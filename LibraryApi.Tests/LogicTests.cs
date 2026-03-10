using LibraryApi.Helpers;

namespace LibraryApi.Tests
{
    [TestFixture]
    public class LogicTests
    {
        private LibraryLogic _logic;

        [SetUp]
        public void Setup()
        {
            // Arrange: Create the object we want to test
            _logic = new LibraryLogic();
        }

        // Password Strength Test

        [Test]
        public void IsPasswordStrong_ValidPassword_ReturnsTrue()
        {
            // Act
            bool result = _logic.IsPasswordStrong("123456"); // Exactly 6 chars

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsPasswordStrong_ShortPassword_ReturnsFalse()
        {
            // Act
            bool result = _logic.IsPasswordStrong("123"); // Only 3 chars

            // Assert
            Assert.That(result, Is.False);
        }

        //  Overdue Logic 

        [Test]
        public void IsOverdue_When15DaysPassed_ReturnsTrue()
        {
            // Arrange
            var issued = new DateTime(2023, 10, 01);
            var now = new DateTime(2023, 10, 16); // 15 days meaning 1 dayoverdue

            // Act
            var result = _logic.IsOverdue(issued, now);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}