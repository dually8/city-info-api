using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts;

public class CityInfoContext : DbContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;

    public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("Chattanooga")
            {
                Id = 1,
                Description = "The big noog"
            },
            new City("Knoxville")
            {
                Id = 2,
                Description = "Way too spread out"
            },
            new City("Nashville")
            {
                Id = 3,
                Description = "Home of the honky tonk"
            }
        );

        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Walking Bridge")
            {
                Id = 1,
                CityId = 1,
                Description = "Where you walk after getting ice cream"
            },
            new PointOfInterest("Art District")
            {
                Id = 2,
                CityId = 1,
                Description = "Lots of neat art things"
            },
            new PointOfInterest("Downtown")
            {
                Id = 3,
                CityId = 2,
                Description = "Lots of neat things here. Check out Tomato Head!"
            },
            new PointOfInterest("Old City")
            {
                Id = 4,
                CityId = 2,
                Description = "It's really old"
            },
            new PointOfInterest("Predator's arena")
            {
                Id = 5,
                CityId = 3,
                Description = "Check out the hockey"
            }
        );
        base.OnModelCreating(modelBuilder);
    }
}