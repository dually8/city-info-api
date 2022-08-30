using AutoMapper;

namespace CityInfo.API.Profiles;

public class PointOfInterest : Profile
{
    public PointOfInterest()
    {
        CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
    }
}