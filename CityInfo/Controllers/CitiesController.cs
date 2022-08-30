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
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _repository.GetCitiesAsync();
            var cityDtos = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
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