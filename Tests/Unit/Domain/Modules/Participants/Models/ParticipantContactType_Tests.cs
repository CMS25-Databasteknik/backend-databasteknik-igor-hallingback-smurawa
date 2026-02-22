using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Tests.Unit.Domain.Modules.Participants.Models;

public class ParticipantContactType_Tests
{
    [Theory]
    [InlineData(ParticipantContactType.Primary)]
    [InlineData(ParticipantContactType.Billing)]
    [InlineData(ParticipantContactType.Emergency)]
    public void Enum_Should_Define_All_Known_Values(ParticipantContactType contactType)
    {
        Assert.True(Enum.IsDefined(typeof(ParticipantContactType), contactType));
    }

    [Fact]
    public void Enum_Should_Reject_Invalid_Value()
    {
        Assert.False(Enum.IsDefined(typeof(ParticipantContactType), (ParticipantContactType)999));
    }
}
