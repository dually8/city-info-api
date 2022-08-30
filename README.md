# City Info API

From pluralsight course [ASP.NET Core 6 Web API Fundamentals](https://app.pluralsight.com/library/courses/asp-dot-net-core-6-web-api-fundamentals/table-of-contents)

## Requirements

- [.NET 6.0.303](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
  - _Might work with later versions, but this was the one I had installed at the time I was doing the course._

## Getting Started

Since this is a .NET 6 application, you should be able to use any IDE you like (or even vim if that's your fancy). You _will_ need the .NET 6 SDK though. You can find that [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). Afterwards, you should have the dotnet cli in your path. From there, you can open up the shell of your choosing and running the following:

1. `dotnet restore`
2. `dotnet run`

This should install all of the NuGet dependencies and run the application. You can navigate to https://localhost:7169/swagger/index.html to see and use the Swagger UI for the API.

## Architecture

This application uses a SqlLite database (`CityInfo.db`) to hold all of the information. This can be swapped out with any other database, but this was the easiest one to work with, and the one that was chosen by the course author.

- `CityInfo.API/Controllers` 
  - Web API controllers
- `CityInfo.API/DbContexts` 
  - Database context(s)
- `CityInfo.API/Entities` 
  - Database models
- `CityInfo.API/Migrations` 
  - Database migrations, auto generated from [ef core](https://docs.microsoft.com/en-us/ef/core/get-started/overview/install)
    - e.g. You add a DB model property (say, add a "TagLine" string to the City entity), you can run `dotnet ef migrations add AddTagLine` then `dotnet ef database update` to both auto generate a migration and update the database.
- `CityInfo.API/Models` 
  - Data transfer objects (DTOs) for use in the Controllers
- `CityInfo.API/Profiles` 
  - AutoMapper configurations
- `CityInfo.API/Services` 
    - Contains middleware between the API and database

## Further Reading

- [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-6.0)
- [Your first Web API](https://docs.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-6.0&tabs=visual-studio-code)
- [Options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0)
  - **TODO:** Maybe look into this for the config stuff? I don't like reusing hard-coded strings. (e.g. `builder.Configuration["Authentication:Issuer"]`)