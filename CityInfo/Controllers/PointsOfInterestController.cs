﻿using CityInfo.API.Models;
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
    private readonly CitiesDataStore _citiesDataStore;

    public PointsOfInterestController(
        ILogger<PointsOfInterestController> logger,
        IMailService mailService,
        CitiesDataStore citiesDataStore
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"Exception when gettings points of interest for city with id {cityId}.",
                e
            );
            return StatusCode(500, "A problem happened while handling your request.");
        }
    }

    [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterest == null)
        {
            return NotFound();
        }

        return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId,
        PointOfInterestForCreationDto pointOfInterest
    )
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        // for demo purposes
        // ~ THERE'S GOT TO BE A BETTER WAY ~
        var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(
            c => c.PointsOfInterest
        ).Max(p => p.Id);

        var newPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointsOfInterest.Add(newPointOfInterest);
        return CreatedAtRoute(
            "GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = newPointOfInterest.Id
            },
            newPointOfInterest
        );
    }

    [HttpPut("{pointOfInterestId}")]
    public ActionResult UpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        PointOfInterestForUpdateDto pointOfInterest
    )
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId}")]
    public ActionResult PartiallyUpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    )
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }

        var pointOfInterestPatch = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description
        };
        patchDocument.ApplyTo(pointOfInterestPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestPatch))
        {
            return BadRequest(ModelState);
        }

        pointOfInterestFromStore.Name = pointOfInterestPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestPatch.Description;

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }

        city.PointsOfInterest.Remove(pointOfInterestFromStore);
        _mailService.Send(
            "Point of interest deleted",
            $"Point of interest {pointOfInterestFromStore.Name} with Id {pointOfInterestFromStore.Id} was deleted."
        );
        return NoContent();
    }
}