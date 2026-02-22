using Backend.Domain.Modules.InstructorRoles.Models;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Tests.Unit.Domain.Modules.Instructors.Models;

public class Instructor_Tests
{
    [Fact]
    public void Constructor_Should_Create_Instructor_When_Parameters_Are_Valid()
    {
        var id = Guid.NewGuid();
        var name = "John Doe";
        var role = new InstructorRole(1, "Lead");

        var instructor = new Instructor(id, name, role);

        Assert.Equal(id, instructor.Id);
        Assert.Equal(name, instructor.Name);
        Assert.Equal(role.Id, instructor.InstructorRoleId);
        Assert.Equal(role, instructor.Role);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Id_Is_Empty()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Instructor(Guid.Empty, "Jane", new InstructorRole(1, "Lead")));
        Assert.Equal("id", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_Name_Is_Invalid(string name)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Instructor(Guid.NewGuid(), name!, new InstructorRole(1, "Lead")));
        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Role_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new Instructor(Guid.NewGuid(), "Jane", null!));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Role_Id_Is_Zero()
    {
        var role = new InstructorRole(0, "Lead");

        var ex = Assert.Throws<ArgumentException>(() => new Instructor(Guid.NewGuid(), "Jane", role));
        Assert.Equal("role", ex.ParamName);
    }

    [Fact]
    public void Constructor_Should_Trim_Name()
    {
        var id = Guid.NewGuid();
        var role = new InstructorRole(1, "Lead");

        var instructor = new Instructor(id, "  Jane  ", role);

        Assert.Equal("Jane", instructor.Name);
    }
}
