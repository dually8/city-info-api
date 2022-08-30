using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; }
    // public static CitiesDataStore Current { get; } = new CitiesDataStore();
    public CitiesDataStore()
    {
        // dummy data
        Cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "Chattanooga",
                Description = "The big noog",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "Walking Bridge",
                        Description = "Where you walk after getting ice cream"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "Art District",
                        Description = "Lots of neat art things"
                    }
                }
            },
            new CityDto()
            {
                Id = 2,
                Name = "Knoxville",
                Description = "Way too spread out",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 3,
                        Name = "Downtown",
                        Description = "Lots of neat things here. Check out Tomato Head!"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 4,
                        Name = "Old City",
                        Description = "It's really old"
                    }
                }
            },
            new CityDto()
            {
                Id = 3,
                Name = "Nashville",
                Description = "Home of the honky tonk",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "Predator's arena",
                        Description = "Check out the hockey"
                    }
                }
            }
        };
    }
}