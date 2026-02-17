using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Tests.Unit.Domain.Modules.Instructors.Models;

public class Instructor_Tests
{
    [Fact]
    public void Constructor_Should_Create_Instructor_When_Parameters_Are_Valid()
    {
        var id = Guid.NewGuid();
        var name = "John Doe";
        var roleId = 1;

        var instructor = new Instructor(id, name, roleId);

        Assert.Equal(id, instructor.Id);
        Assert.Equal(name, instructor.Name);
        Assert.Equal(roleId, instructor.InstructorRoleId);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Id_Is_Empty()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Instructor(Guid.Empty, "Jane", 1));
        Assert.Equal("id", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_Name_Is_Invalid(string name)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Instructor(Guid.NewGuid(), name!, 1));
        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Role_Is_Less_Than_One()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Instructor(Guid.NewGuid(), "Jane", 0));
        Assert.Equal("instructorRoleId", ex.ParamName);
    }
}
