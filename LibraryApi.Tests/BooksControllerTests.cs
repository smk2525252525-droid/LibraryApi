
//Get all books
using LibraryApi.Controllers;
using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryApi.Tests
{
    [TestFixture]
    public class BooksControllerTests
    {
        private LibraryDbContext _context;
        private BooksController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryDbContext(options);
            _controller = new BooksController(_context);
        }

        // TEST CASE 1: Successful Retrieval
        [Test]
        public async Task GetAllBooksAsync_WhenBooksExist_ReturnsAllBooks()
        {
            // Arrange
              var category = new Category { Id = 1, Name = "Test Category" };
            _context.Categories.Add(category);
            
            _context.Books.Add(new Book { Id = 1, Title = "C# Basics", Author = "John Smith", CategoryId = 1 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBooks();

            // Assert

            // Check that the result is not null and contains the expected number of books
            Assert.That(result.Value, Is.Not.Null);



            Assert.That(result.Value.Count(), Is.EqualTo(1));
            Assert.That(result.Value.First().Title, Is.EqualTo("C# Basics"));
        }

    
        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}