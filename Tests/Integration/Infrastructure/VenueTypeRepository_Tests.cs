using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class VenueTypeRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateVenueTypeAsync_ShouldPersist_And_BeReadableByIdAndName()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new VenueTypeRepository(context);
        var name = $"Venue-{Guid.NewGuid():N}";

        var created = await repo.AddAsync(new VenueType(1, name), CancellationToken.None);
        var byId = await repo.GetByIdAsync(created.Id, CancellationToken.None);
        var byName = await repo.GetByNameAsync(name, CancellationToken.None);

        Assert.NotNull(byId);
        Assert.NotNull(byName);
        Assert.Equal(name, byId!.Name);
        Assert.Equal(created.Id, byName!.Id);

        var persisted = await context.VenueTypes
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(name, persisted.Name);
    }

    [Fact]
    public async Task IsInUseAsync_ShouldReturnTrue_WhenReferencedByCourseEvent()
    {
        await using var context = fixture.CreateDbContext();
        var venueTypeRepo = new VenueTypeRepository(context);
        var venueType = await venueTypeRepo.AddAsync(
            new VenueType(1, $"Venue-{Guid.NewGuid():N}"),
            CancellationToken.None);
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        var type = await RepositoryTestDataHelper.CreateCourseEventTypeAsync(context);
        var eventRepo = new CourseEventRepository(context);

        await eventRepo.AddAsync(
            new CourseEvent(
                Guid.NewGuid(),
                course.Id,
                DateTime.UtcNow.AddDays(1),
                100m,
                10,
                type.Id,
                new VenueType(venueType.Id, venueType.Name)),
            CancellationToken.None);

        var inUse = await venueTypeRepo.IsInUseAsync(venueType.Id, CancellationToken.None);

        Assert.True(inUse);
    }
}

