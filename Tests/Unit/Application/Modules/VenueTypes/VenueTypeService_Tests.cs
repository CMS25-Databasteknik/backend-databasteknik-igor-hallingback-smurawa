using Backend.Application.Common;
using Backend.Application.Modules.VenueTypes;
using Backend.Application.Modules.VenueTypes.Caching;
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
        var cache = Substitute.For<IVenueTypeCache>();
        cache.GetAllAsync(Arg.Any<Func<CancellationToken, Task<IReadOnlyList<VenueType>>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<IReadOnlyList<VenueType>>>>()(ci.Arg<CancellationToken>()));
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([new VenueType(1, "InPerson"), new VenueType(2, "Online")]);
        var service = new VenueTypeService(cache, repo);

        var result = await service.GetAllVenueTypesAsync();

        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result!.Count);
    }
}

