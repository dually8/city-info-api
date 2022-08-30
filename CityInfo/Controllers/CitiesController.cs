using System.Text.Json;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _repository;
        private readonly IMapper _mapper;
        private const int maxCitiesPageSize = 20;

        public CitiesController(
            ICityInfoRepository repository,
            IMapper mapper
        )
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Route: api/cities
        /// </summary>
        /// <returns>List of cities</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            // The attribute and name is unnecessary here
            // but this shows how you can use it.
            // Name = "XXX" tells us what the query param will be
            // e.g. api/cities?XXX=something
            [FromQuery(Name = "name")] string? name,
            string? searchQuery,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (cityEntities, paginationMetadata) = await _repository.GetCitiesAsync(
                name,
                searchQuery,
                pageNumber,
                pageSize
            );
            var cityDtos = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
            
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(cityDtos);
        }

        /// <summary>
        /// Route: api/cities/{id}[?includePointsOfInterest=true]
        /// </summary>
        /// <param name="id">Id of the city</param>
        /// <param name="includePointsOfInterest">Do or do not include points of interest</param>
        /// <returns>CityWithoutPointsOfInterestDto</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(
            int id,
            bool includePointsOfInterest = false
        )
        {
            var cityEntity = await _repository.GetCityAsync(id, includePointsOfInterest);
            if (cityEntity == null)
            {
                return NotFound();
            }

            return includePointsOfInterest
                ? Ok(_mapper.Map<CityDto>(cityEntity))
                : Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityEntity));
        }
    }
}