using LibraryApi.Controllers;
using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace LibraryApi.Tests
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private LibraryDbContext _context;
        private CategoriesController _controller;

        [SetUp]
        public void Setup()
        {
            // 1. ARRANGE: Set up the In-Memory Database
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryDbContext(options);
            _controller = new CategoriesController(_context);
        }

        [Test]
        public async Task CreateCategoryAsync_ValidDto_SavesToDatabaseAndReturnsCreated()
        {
            // 1. ARRANGE: Create the data we want to send
            var categoryDto = new CategoryDto { Name = "Technology" };

            // 2. ACT: Call the POST method
            var result = await _controller.CreateCategory(categoryDto);

            // 3. ASSERT: 
            // Check if the result is 'CreatedAtActionResult' (Status 201)
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));

            // Verify the data exists in our fake database
            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Technology");
            Assert.That(categoryInDb, Is.Not.Null);
            Assert.That(categoryInDb!.Name, Is.EqualTo("Technology"));
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}