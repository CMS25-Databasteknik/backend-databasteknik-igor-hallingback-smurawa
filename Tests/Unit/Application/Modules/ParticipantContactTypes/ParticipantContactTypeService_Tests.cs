using Backend.Application.Modules.ParticipantContactTypes;
using Backend.Domain.Modules.ParticipantContactTypes.Contracts;
using Backend.Domain.Modules.ParticipantContactTypes.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.ParticipantContactTypes;

public class ParticipantContactTypeService_Tests
{
    [Fact]
    public async Task GetAll_Should_Return_Items()
    {
        var repo = Substitute.For<IParticipantContactTypeRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([new ParticipantContactType(1, "Primary"), new ParticipantContactType(2, "Billing")]);
        var service = new ParticipantContactTypeService(repo);

        var result = await service.GetAllParticipantContactTypesAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result!.Count);
    }
}
