namespace LibraryApi.Tests;

[TestFixture]
public class LearningTests
{
    [Test]
    public void StringComparison_Test()
    {
        // Arrange
        string name = "Library";

        // Act
        string upperName = name.ToUpper();

        // Assert
        Assert.That(upperName, Is.EqualTo("LIBRARY"));
    }
}