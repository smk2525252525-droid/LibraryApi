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
    [Authorize]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _context;
    public BooksController(LibraryDbContext context) { _context = context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks() => 
        await _context.Books.Include(b => b.Category).ToListAsync();

        

    [HttpPost]
    [Authorize(Roles = "Admin")]
public async Task<ActionResult<Book>> PostBook(BookCreateDto dto)
{
    var book = new Book 
    { 
        Title = dto.Title, 
        Author = dto.Author,
        CategoryId = dto.CategoryId,
        Status = dto.Status
    };

    _context.Books.Add(book);
    await _context.SaveChangesAsync();
    return Ok(book);
}

    // PUT: api/Books/5
// Used for updating the ENTIRE book object
[Authorize(Roles = "Admin")]
[HttpPut("{id}")]
public async Task<IActionResult> PutBook(int id, BookCreateDto dto)
{
    var book = await _context.Books.FindAsync(id);
    if (book == null) return NotFound();

    book.Title = dto.Title;
    book.Author = dto.Author;
    book.CategoryId = dto.CategoryId;
    book.Status = dto.Status;

    try {
        await _context.SaveChangesAsync();
    } catch (DbUpdateConcurrencyException) {
        if (!_context.Books.Any(e => e.Id == id)) return NotFound();
        else throw;
    }

    return NoContent(); // 204 No Content is standard for successful PUT
}

// PATCH: api/Books/status/5
// Used for partial updates (just changing the status)
[HttpPatch("status/{id}")]
public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
{
    var book = await _context.Books.FindAsync(id);
    if (book == null) return NotFound();

    book.Status = newStatus;
    await _context.SaveChangesAsync();

    return Ok(new { message = "Status updated", status = book.Status });
}

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
}
