using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//the bussiness logic,handles user authentication, generates JWT tokens, and defines the API endpoints for login.
//controllers use dtos to talk to front end and models to talk to database, 

//authcontroller manages access
namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(LibraryDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

       [HttpPost("login")]
public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)//deserializes the incoming JSON payload into a LoginDto object, 
                                                //if we didnt use this([from body]): even if the frontend sent the data perfectly, the C# code wouldn't know where to look for it

{
    // Standard: Basic validation check
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            // Standard: Eager load the Role to include it in the JWT claims
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

// Use BCrypt to verify the plain-text input against the stored hash
    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

    if (!isPasswordValid)
    {
        return Unauthorized(new { message = "Invalid credentials" });
    }

    Console.WriteLine("---> SUCCESS: Token generated.");
    var token = GenerateJwtToken(user);
   
   // Return both token and role name to the frontend
    return Ok(new { 
    token = token, 
    role = user.Role?.Name, // This will be "Admin" or "Member"
     userId = user.Id,
     email = user.Email 
    });

}

    // Helper method to create a JWT for the authenticated user.
        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKeyForJWT12345678901234567890!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]//  Standard: Define claims to be embedded in the token
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "MyLibraryApi",
                audience: _configuration["Jwt:Audience"] ?? "MyLibraryApi",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),//token validity: 2 hours(common choice for session tokens)
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
