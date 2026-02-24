using Backend.Application.Modules.VenueTypes;
using Backend.Domain.Modules.VenueTypes.Contracts;
using Backend.Domain.Modules.VenueTypes.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.VenueTypes;

public class VenueTypeService_Tests
{
    [Fact]
    public async Task GetAll_Should_Return_Items()
    {
        var repo = Substitute.For<IVenueTypeRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([new VenueType(1, "InPerson"), new VenueType(2, "Online")]);
        var service = new VenueTypeService(repo);

        var result = await service.GetAllVenueTypesAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result!.Count);
    }
}
