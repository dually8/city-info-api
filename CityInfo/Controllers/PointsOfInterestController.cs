using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : Controller
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _repository;
    private readonly IMapper _mapper;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        ICityInfoRepository repository,
        IMapper mapper
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        if (!await _repository.DoesCityExistAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest");
            return NotFound();
        }

        var poiEntity = await _repository.GetPointsOfInterestForCityAsync(cityId);
        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(poiEntity));
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        if (!await _repository.DoesCityExistAsync(cityId))
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest");
            return NotFound();
        }

        var poiEntity = await _repository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

        if (poiEntity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(poiEntity));
    }

    // [HttpPost]
    // public ActionResult<PointOfInterestDto> CreatePointOfInterest(
    //     int cityId,
    //     PointOfInterestForCreationDto pointOfInterest
    // )
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
    //     if (city == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     // for demo purposes
    //     // ~ THERE'S GOT TO BE A BETTER WAY ~
    //     var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
    //         c => c.PointsOfInterest
    //     ).Max(p => p.Id);
    //
    //     var newPointOfInterest = new PointOfInterestDto()
    //     {
    //         Id = ++maxPointOfInterestId,
    //         Name = pointOfInterest.Name,
    //         Description = pointOfInterest.Description
    //     };
    //
    //     city.PointsOfInterest.Add(newPointOfInterest);
    //     return CreatedAtRoute(
    //         "GetPointOfInterest",
    //         new
    //         {
    //             cityId = cityId,
    //             pointOfInterestId = newPointOfInterest.Id
    //         },
    //         newPointOfInterest
    //     );
    // }
    //
    // [HttpPut("{pointOfInterestId}")]
    // public ActionResult UpdatePointOfInterest(
    //     int cityId,
    //     int pointOfInterestId,
    //     PointOfInterestForUpdateDto pointOfInterest
    // )
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
    //     if (city == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
    //     if (pointOfInterestFromStore == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     pointOfInterestFromStore.Name = pointOfInterest.Name;
    //     pointOfInterestFromStore.Description = pointOfInterest.Description;
    //
    //     return NoContent();
    // }
    //
    // [HttpPatch("{pointOfInterestId}")]
    // public ActionResult PartiallyUpdatePointOfInterest(
    //     int cityId,
    //     int pointOfInterestId,
    //     JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    // )
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
    //     if (city == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
    //     if (pointOfInterestFromStore == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var pointOfInterestPatch = new PointOfInterestForUpdateDto()
    //     {
    //         Name = pointOfInterestFromStore.Name,
    //         Description = pointOfInterestFromStore.Description
    //     };
    //     patchDocument.ApplyTo(pointOfInterestPatch, ModelState);
    //
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     if (!TryValidateModel(pointOfInterestPatch))
    //     {
    //         return BadRequest(ModelState);
    //     }
    //
    //     pointOfInterestFromStore.Name = pointOfInterestPatch.Name;
    //     pointOfInterestFromStore.Description = pointOfInterestPatch.Description;
    //
    //     return NoContent();
    // }
    //
    // [HttpDelete("{pointOfInterestId}")]
    // public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
    //     if (city == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
    //     if (pointOfInterestFromStore == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     city.PointsOfInterest.Remove(pointOfInterestFromStore);
    //     _mailService.Send(
    //         "Point of interest deleted",
    //         $"Point of interest {pointOfInterestFromStore.Name} with Id {pointOfInterestFromStore.Id} was deleted."
    //     );
    //     return NoContent();
    // }
}