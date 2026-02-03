using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly LibraryDbContext _context;
    public CategoriesController(LibraryDbContext context) { _context = context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories() => await _context.Categories.ToListAsync();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Category>> PostCategory(CategoryDto dto)
    {
        var category = new Category { Name = dto.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
    }

    // DELETE: api/Categories/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
public async Task<IActionResult> DeleteCategory(int id)
{
    var category = await _context.Categories.Include(c => c.Books).FirstOrDefaultAsync(c => c.Id == id);
    if (category == null) return NotFound();

    if (category.Books.Any()) {
        return BadRequest("Cannot delete category because it contains books. Move the books first.");
    }

    _context.Categories.Remove(category);
    await _context.SaveChangesAsync();
    return NoContent();
}
}