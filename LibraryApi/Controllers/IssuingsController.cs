using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class IssuingsController : ControllerBase
{
    private readonly LibraryDbContext _context;
    public IssuingsController(LibraryDbContext context) { _context = context; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Issuing>>> GetAllIssuingsAsync() => 
        await _context.Issuings.Include(i => i.User).Include(i => i.Book).ToListAsync();

    [HttpPost]
public async Task<ActionResult<Issuing>> CreateIssuingAsync(IssuingDto dto)
{
    
    var issuing = new Issuing 
    { 
        UserId = dto.UserId, 
        BookId = dto.BookId, 
        IssuedAt = DateTime.Now 
    };

    _context.Issuings.Add(issuing);
    await _context.SaveChangesAsync();
    return Ok(issuing);
}

    // Logic for returning a book, sets tiemstamp for when the book was returned in database
    [HttpPut("return/{id}")]
    public async Task<IActionResult> ReturnBookAsync(int id)
    {
        var issuing = await _context.Issuings.FindAsync(id);
        if (issuing == null) return NotFound(new { message = "Issuing record not found." });
        
        issuing.ReturnedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Book returned successfully" });
    }
}

