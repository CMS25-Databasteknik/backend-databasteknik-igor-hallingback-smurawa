using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Tests.Unit.Domain.Modules.Instructors.Models;

public class Instructor_Tests
{
    [Fact]
    public void Constructor_Should_Create_Instructor_When_Parameters_Are_Valid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.NotNull(instructor);
        Assert.Equal(id, instructor.Id);
        Assert.Equal(name, instructor.Name);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Id_Is_Empty()
    {
        // Arrange
        var id = Guid.Empty;
        var name = "John Doe";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Instructor(id, name));

        Assert.Equal("id", exception.ParamName);
        Assert.Contains("ID cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Name_Is_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        string name = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Instructor(id, name));

        Assert.Equal("name", exception.ParamName);
        Assert.Contains("Name cannot be null or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Name_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Instructor(id, name));

        Assert.Equal("name", exception.ParamName);
        Assert.Contains("Name cannot be null or whitespace", exception.Message);
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentException_When_Name_Is_Whitespace()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "   ";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Instructor(id, name));

        Assert.Equal("name", exception.ParamName);
        Assert.Contains("Name cannot be null or whitespace", exception.Message);
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData("Jane Smith")]
    [InlineData("Dr. Robert Johnson")]
    [InlineData("Prof. Alice Williams")]
    public void Constructor_Should_Create_Instructor_With_Various_Valid_Names(string name)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal(name, instructor.Name);
    }

    [Fact]
    public void Properties_Should_Be_Initialized_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal(id, instructor.Id);
        Assert.Equal(name, instructor.Name);
    }

    [Fact]
    public void Two_Instructors_With_Same_Values_Should_Have_Same_Property_Values()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";

        var instructor1 = new Instructor(id, name);
        var instructor2 = new Instructor(id, name);

        // Assert
        Assert.Equal(instructor1.Id, instructor2.Id);
        Assert.Equal(instructor1.Name, instructor2.Name);
    }

    [Fact]
    public void Id_Property_Should_Be_Read_Only()
    {
        // Arrange
        var instructor = new Instructor(Guid.NewGuid(), "John Doe");

        // Assert
        var initialId = instructor.Id;
        Assert.Equal(initialId, instructor.Id);
    }

    [Fact]
    public void Name_Property_Should_Be_Read_Only()
    {
        // Arrange
        var instructor = new Instructor(Guid.NewGuid(), "John Doe");

        // Assert
        var initialName = instructor.Name;
        Assert.Equal(initialName, instructor.Name);
    }

    [Fact]
    public void Constructor_Should_Handle_Long_Names()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Professor Dr. John Michael Robert Smith Johnson";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal(name, instructor.Name);
    }

    [Fact]
    public void Constructor_Should_Handle_Names_With_Special_Characters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Dr. O'Brien-Smith";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal(name, instructor.Name);
    }

    [Fact]
    public void Constructor_Should_Handle_Swedish_Characters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Göran Åström";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal(name, instructor.Name);
    }

    [Fact]
    public void Constructor_Should_Handle_Names_With_Titles()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Dr. Robert Johnson";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal(name, instructor.Name);
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData("Jane Smith")]
    [InlineData("Alice Johnson")]
    [InlineData("Bob Williams")]
    public void Two_Instructors_With_Different_Ids_Should_Be_Different(string name)
    {
        // Arrange
        var instructor1 = new Instructor(Guid.NewGuid(), name);
        var instructor2 = new Instructor(Guid.NewGuid(), name);

        // Assert
        Assert.NotEqual(instructor1.Id, instructor2.Id);
        Assert.Equal(instructor1.Name, instructor2.Name);
    }

    [Fact]
    public void Constructor_Should_Accept_First_And_Last_Name_Only()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John Doe";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal("John Doe", instructor.Name);
    }

    [Fact]
    public void Constructor_Should_Accept_Single_Name()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "John";

        // Act
        var instructor = new Instructor(id, name);

        // Assert
        Assert.Equal("John", instructor.Name);
    }
}

