using LibraryApi.Controllers;
using LibraryApi.Data;
using LibraryApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace LibraryApi.Tests
{
    [TestFixture]
    public class UsersControllerTests
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
        public async Task RegisterUserAsync_SetsDefaultRoleToMember()
        {
            // 1. ARRANGE
            var controller = new UsersController(_context);
            var newUser = new UserCreateDto { 
                Name = "New Member", 
                Email = "member@test.com", 
                Password = "password123" 
            };

            // 2. ACT
            await controller.RegisterUser(newUser);

            // 3. ASSERT: Check if User was saved with RoleId = 2
            var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "member@test.com");
            
            Assert.That(savedUser, Is.Not.Null);
            Assert.That(savedUser!.RoleId, Is.EqualTo(2));
        }

        [TearDown]
        public void Cleanup() { _context.Dispose(); }
    }
}