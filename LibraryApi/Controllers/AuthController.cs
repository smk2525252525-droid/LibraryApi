using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
public IActionResult Login([FromBody] LoginDto loginDto)
{
    // LOG 1: Check what arrived
    Console.WriteLine($"---> Login Request Received for: {loginDto.Email}");

    var user = _context.Users.Include(u => u.Role)
                             .FirstOrDefault(u => u.Email == loginDto.Email);

    if (user == null)
    {
        Console.WriteLine("---> ERROR: User not found in database.");
        return Unauthorized(new { message = "Invalid credentials" });
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
     userId = user.Id 
    });

}

        private string GenerateJwtToken(User user)
        {
            var jwtSecret = _configuration["Jwt:Secret"] ?? "SuperSecretKeyForJWT12345678901234567890!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
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
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
