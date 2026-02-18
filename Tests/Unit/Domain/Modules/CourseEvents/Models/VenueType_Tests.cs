using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Tests.Unit.Domain.Modules.CourseEvents.Models;

public class VenueType_Tests
{
    [Theory]
    [InlineData(VenueType.InPerson)]
    [InlineData(VenueType.Online)]
    [InlineData(VenueType.Hybrid)]
    public void Enum_Should_Contain_Expected_Values(VenueType venueType)
    {
        Assert.True(Enum.IsDefined(typeof(VenueType), venueType));
    }

    [Fact]
    public void Enum_Should_Reject_Invalid_Value()
    {
        Assert.False(Enum.IsDefined(typeof(VenueType), (VenueType)999));
    }
}

