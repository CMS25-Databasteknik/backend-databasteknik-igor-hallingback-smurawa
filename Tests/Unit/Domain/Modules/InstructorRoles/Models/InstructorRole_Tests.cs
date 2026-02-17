using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Tests.Unit.Domain.Modules.InstructorRoles.Models;

public class InstructorRole_Tests
{
    [Fact]
    public void Constructor_Should_Create_Role_When_Valid()
    {
        var role = new InstructorRole(1, "Lead");

        Assert.Equal(1, role.Id);
        Assert.Equal("Lead", role.RoleName);
    }

    [Fact]
    public void Constructor_Should_Trim_Name()
    {
        var role = new InstructorRole(2, "  Assistant  ");

        Assert.Equal("Assistant", role.RoleName);
    }

    [Theory]
    [InlineData(-1)]
    public void Constructor_Should_Throw_When_Id_Invalid(int id)
    {
        var ex = Assert.Throws<ArgumentException>(() => new InstructorRole(id, "Lead"));
        Assert.Equal("id", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_Name_Invalid(string name)
    {
        var ex = Assert.Throws<ArgumentException>(() => new InstructorRole(1, name!));
        Assert.Equal("roleName", ex.ParamName);
    }
}

