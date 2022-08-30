using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers;

[Route("api/authentication")]
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class AuthenticationController : Controller
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Not to be used outside of this class
    /// </summary>
    public class AuthenticationRequestBody
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate(AuthenticationRequestBody requestBody)
    {
        // Step 1: validate the username/password
        var user = ValidateUserCredentials(requestBody.Username, requestBody.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        // Step 2: create a token
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"])
        );
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>();
        claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
        claimsForToken.Add(new Claim("given_name", user.FirstName));
        claimsForToken.Add(new Claim("family_name", user.LastName));
        claimsForToken.Add(new Claim("city", user.City));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials
        );

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }

    private CityInfoUser ValidateUserCredentials(string? userName, string? password)
    {
        if (string.Equals(userName, "cjcoffey"))
        {
            return new CityInfoUser(
                1,
                "cjcoffey",
                "CJ",
                "Coffey",
                "Chattanooga"
            );
        }
        return new CityInfoUser(
            2,
            userName ?? "cowboy",
            "Cowboy",
            "Troy",
            "Nashville"
        );
    }

    /// <summary>
    /// Not to be used outside of this class
    /// </summary>
    private class CityInfoUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }

        public CityInfoUser(
            int userId,
            string username,
            string firstName,
            string lastName,
            string city
        )
        {
            UserId = userId;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            City = city;
        }
    }
}