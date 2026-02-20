using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CourseRegistrationStatusRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateCourseRegistrationStatusAsync_ShouldPersist_And_BeReadableByName()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRegistrationStatusRepository(context);
        var name = $"Status-{Guid.NewGuid():N}";

        var created = await repo.CreateCourseRegistrationStatusAsync(new CourseRegistrationStatus(name), CancellationToken.None);
        var byName = await repo.GetCourseRegistrationStatusByNameAsync(name, CancellationToken.None);

        Assert.NotNull(byName);
        Assert.Equal(created.Id, byName!.Id);
        Assert.Equal(name, created.Name);
        Assert.Equal(name, byName.Name);

        var persisted = await context.CourseRegistrationStatuses
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(created.Id, persisted.Id);
        Assert.Equal(name, persisted.Name);
    }

    [Fact]
    public async Task GetAllCourseRegistrationStatusesAsync_ShouldContainSeededStatuses()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRegistrationStatusRepository(context);

        var all = await repo.GetAllCourseRegistrationStatusesAsync(CancellationToken.None);

        Assert.Contains(all, x => x.Id == 0 && x.Name == "Pending");
        Assert.Contains(all, x => x.Id == 1 && x.Name == "Paid");
    }

    [Fact]
    public async Task GetCourseRegistrationStatusByIdAsync_ShouldReturnStatus()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRegistrationStatusRepository(context);

        var loaded = await repo.GetCourseRegistrationStatusByIdAsync(0, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal("Pending", loaded!.Name);
    }

    [Fact]
    public async Task UpdateCourseRegistrationStatusAsync_ShouldPersistChanges()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRegistrationStatusRepository(context);
        var created = await repo.CreateCourseRegistrationStatusAsync(new CourseRegistrationStatus($"Status-{Guid.NewGuid():N}"), CancellationToken.None);

        var updated = await repo.UpdateCourseRegistrationStatusAsync(new CourseRegistrationStatus(created.Id, "Renamed"), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("Renamed", updated!.Name);

        var persisted = await context.CourseRegistrationStatuses
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal("Renamed", persisted.Name);
    }

    [Fact]
    public async Task IsInUseAsync_ShouldReturnTrueWhenReferencedByRegistration()
    {
        await using var context = fixture.CreateDbContext();
        await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context, status: CourseRegistrationStatus.Pending);
        var repo = new CourseRegistrationStatusRepository(context);

        var inUse = await repo.IsInUseAsync(0, CancellationToken.None);

        Assert.True(inUse);
    }

    [Fact]
    public async Task DeleteCourseRegistrationStatusAsync_ShouldRemoveStatus()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRegistrationStatusRepository(context);
        var created = await repo.CreateCourseRegistrationStatusAsync(new CourseRegistrationStatus($"Status-{Guid.NewGuid():N}"), CancellationToken.None);

        var deleted = await repo.DeleteCourseRegistrationStatusAsync(created.Id, CancellationToken.None);
        var loaded = await repo.GetCourseRegistrationStatusByIdAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(loaded);
    }
}
