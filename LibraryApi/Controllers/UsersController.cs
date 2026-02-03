using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        public UsersController(LibraryDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [AllowAnonymous] // Allow anyone to register
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserCreateDto userDto)
        {
            // Map DTO to Entity
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
               PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),  // Mapping "Password" from DTO to "PasswordHash" in DB
                RoleId = 2 // Force default role to "Member" for self-registration
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Return the created user (The [JsonIgnore] will ensure PasswordHash is hidden in this response)
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserCreateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.RoleId = dto.RoleId;

            // Only update password if a new one is provided in the DTO and not replace with null in db
             if (!string.IsNullOrEmpty(dto.Password))
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Users/change-role/5
        [Authorize(Roles = "Admin")]
        [HttpPatch("change-role/{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] int newRoleId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Check if the new role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == newRoleId);
            if (!roleExists) return BadRequest("The specified Role ID does not exist.");

            user.RoleId = newRoleId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User role updated successfully", userId = user.Id, newRoleId = user.RoleId });
        }

        // DELETE: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Include Issuings to check for active loans
            var user = await _context.Users.Include(u => u.Issuings).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Logic Check: Don't delete user if they haven't returned books
            if (user.Issuings.Any(i => i.ReturnedAt == null))
            {
                return BadRequest("Cannot delete user. They have active book issuings that are not yet returned.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}