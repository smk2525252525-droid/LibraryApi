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
        //private field with underscore
        private readonly LibraryDbContext _context;//camelCase undersocre to signify private variable global to class
        public UsersController(LibraryDbContext context)//variable naming in small case
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {   
            var users = await _context.Users.Include(u => u.Role).ToListAsync();//resource rule: added async suffix to let developers know to addd/use await when calling this method
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id) //Descriptive name for clarity, indicates async operation
        {
            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [AllowAnonymous] // Allow anyone to register
        [HttpPost]
        public async Task<ActionResult<User>> RegisterUserAsync(UserCreateDto userDto) //renamed method for clarity, varibale with camelCase
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

            // Return the created user 
            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.Id }, user);//resource rule: used nameof to avoid hardcoding the method name, which helps prevent errors during refactoring and improves maintainability
        }

        // PUT: api/Users/id
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, UserCreateDto dto)//updated method name for clarity, variable with camelCase
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
        public async Task<IActionResult> UpdateUserRoleAsync(int id, [FromBody] int newRoleId)//deserializes the incoming JSON payload into a LoginDto object, 
                                                //if we didnt use this([from body]): even if the frontend sent the data perfectly, the C# code wouldn't know where to look for it
        {
            var user = await _context.Users.FindAsync(id);//underscore prefix for private variable
            if (user == null) return NotFound();

            // Check if the new role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == newRoleId);
            if (!roleExists) return BadRequest("The specified Role ID does not exist.");

            user.RoleId = newRoleId;//apply changes to the role

            await _context.SaveChangesAsync();

            return Ok(new { message = "User role updated successfully", userId = user.Id, newRoleId = user.RoleId });//structured json response
        }

        // DELETE: api/Users/id
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)//changed method name for clarity, variable with camelCase
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