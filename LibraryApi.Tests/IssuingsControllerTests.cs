using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryApi.Tests
{
    [TestFixture]
    public class IssuingsControllerTests
    {
        private LibraryDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new LibraryDbContext(options);
        }

        [Test]
        public async Task ReturnBookAsync_WhenCalled_UpdatesTimestamp()
        {
            // 1. ARRANGE: Create an issuing record with NULL return date
            var issuing = new Issuing { 
                Id = 1, 
                BookId = 1, 
                UserId = 1, 
                IssuedAt = DateTime.Now,
                ReturnedAt = null // Ensure it's currently null
            };
            _context.Issuings.Add(issuing);
            await _context.SaveChangesAsync();

            var controller = new IssuingsController(_context);

            // 2. ACT
            await controller.ReturnBook(1);

            // 3. ASSERT: Fetch again and check if ReturnedAt has a value
            var updatedRecord = await _context.Issuings.FindAsync(1);
            Assert.That(updatedRecord!.ReturnedAt, Is.Not.Null);
        }

        [TearDown]
        public void Cleanup() { _context.Dispose(); }
    }
}