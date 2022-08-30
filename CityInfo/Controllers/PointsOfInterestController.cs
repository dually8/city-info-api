using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
[ApiController]
// [Authorize(Policy = "MustBeFromChattanooga")] // See Authorization region in Program.cs
[ApiVersion("2.0")]
public class PointsOfInterestController : Controller
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        ICityInfoRepository cityInfoRepository,
        IMapper mapper
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        // Can remove this block. Just shows how you can utilize the JWT token claims
        var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
        if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
        {
            return Forbid();
        }
        
        if (!await _cityInfoRepository.DoesCityExistAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest");
            return NotFound();
        }

        var poiEntity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(poiEntity));
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.DoesCityExistAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest");
            return NotFound();
        }

        var poiEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

        if (poiEntity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(poiEntity));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        int cityId,
        PointOfInterestForCreationDto pointOfInterest
    )
    {
        if (!await _cityInfoRepository.DoesCityExistAsync(cityId))
        {
            return NotFound();
        }

        var newPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);
        await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, newPointOfInterest);
        await _cityInfoRepository.SaveChangesAsync();

        var createdPointOfInterest = _mapper.Map<Models.PointOfInterestDto>(newPointOfInterest);
        
        return CreatedAtRoute(
            "GetPointOfInterest",
            new
            {
                cityId,
                pointOfInterestId = createdPointOfInterest.Id
            },
            createdPointOfInterest
        );
    }
    
    [HttpPut("{pointOfInterestId}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest
    )
    {
        if (!await _cityInfoRepository.DoesCityExistAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        // Maps incoming data to existing entity
        _mapper.Map(pointOfInterest, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPatch("{pointOfInterestId}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    )
    {
        if (!await _cityInfoRepository.DoesCityExistAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
    
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
    
        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
    
        return NoContent();
    }
    
    [HttpDelete("{pointOfInterestId}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.DoesCityExistAsync(cityId))
        {
            return NotFound();
        }

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        if (pointOfInterestEntity == null)
        {
            return NotFound();
        }
    
        _cityInfoRepository.DeletePointOfInterestForCity(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        
        _mailService.Send(
            "Point of interest deleted",
            $"Point of interest {pointOfInterestEntity.Name} with Id {pointOfInterestEntity.Id} was deleted."
        );
        return NoContent();
    }
}