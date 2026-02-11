using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")] // Only Admins can manage roles
public class RolesController : ControllerBase
{
    private readonly LibraryDbContext _context;
    public RolesController(LibraryDbContext context) { _context = context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRolesAsync() => await _context.Roles.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Role>>  CreateRoleAsync(RoleDto dto)
    {
        var role = new Role { Name = dto.Name };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRolesAsync), new { id = role.Id }, role);
    }

    // PUT: api/Roles/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRole(int id, RoleDto dto)
    {
    var role = await _context.Roles.FindAsync(id);
    if (role == null) return NotFound();

    role.Name = dto.Name;
    await _context.SaveChangesAsync();

    return NoContent();
    }
}
}