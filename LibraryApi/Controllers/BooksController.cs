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
    public BooksController(LibraryDbContext context) { _context = context; }//constructor injection for db context, underscore prefix for private variable

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks() => 
        await _context.Books.Include(b => b.Category).ToListAsync();

           // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
            
            if (book == null) return NotFound(new { message = "Book not found." });
            return Ok(book);
        }



    [HttpPost]
    [Authorize(Roles = "Admin")]
public async Task<ActionResult<Book>> CreateBookAsync(BookCreateDto dto)
{
    var book = new Book 
    { 
        Title = dto.Title, 
        Author = dto.Author,
        CategoryId = dto.CategoryId,
        Status = dto.Status ?? "Available" // Default to "Available" if not provided
    };

    _context.Books.Add(book);
    await _context.SaveChangesAsync();
      // Using nameof() ensures that if the method name changes, the compiler catches it
    return CreatedAtAction(nameof(GetBookByIdAsync), new { id = book.Id }, book);
}

 // PUT: api/Books/5
// Used for updating the ENTIRE book object
[Authorize(Roles = "Admin")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateBookAsync(int id, BookCreateDto dto)
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
public async Task<IActionResult> UpdateBookStatusAsync(int id, [FromBody] string newStatus)
{
    var book = await _context.Books.FindAsync(id);
    if (book == null) return NotFound();

    book.Status = newStatus;
    await _context.SaveChangesAsync();

    return Ok(new { message = "Status updated", status = book.Status });
}

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
}
