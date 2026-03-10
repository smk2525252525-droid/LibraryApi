using LibraryApi.Controllers;
using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LibraryApi.Tests
{
    [TestFixture]
    public class AuthControllerTests
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
        public async Task LoginAsync_ValidCredentials_ReturnsOk()
        {
            // 1. ARRANGE: Create Role and User with HASHED password
            var role = new Role { Id = 1, Name = "Admin" };
            _context.Roles.Add(role);

            var password = "password123";
            var user = new User { 
                Email = "test@test.com", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), 
                Name = "Tester",
                RoleId = 1
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

              // Mock IConfiguration for JWT token generation
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Key"]).Returns("your-test-secret-key-that-is-long-enough");
            mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfig.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");


            // until the GenerateToken step, which we can skip or mock.
            var controller = new AuthController(_context, mockConfig.Object);
            var loginDto = new LoginDto { Email = "test@test.com", Password = password };

            // 2. ACT
            var result =controller.Login(loginDto) as OkObjectResult;

            // 3. ASSERT
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StatusCode, Is.EqualTo(200));
        }

        [TearDown]
        public void Cleanup() { _context.Dispose(); }
    }
}