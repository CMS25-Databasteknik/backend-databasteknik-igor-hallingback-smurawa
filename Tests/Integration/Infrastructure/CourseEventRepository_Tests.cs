using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CourseEventRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateCourseEventAsync_ShouldPersist_And_BeReadableByIdAndCourseId()
    {
        await using var context = fixture.CreateDbContext();
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        var type = await RepositoryTestDataHelper.CreateCourseEventTypeAsync(context);
        var repo = new CourseEventRepository(context);

        var input = new CourseEvent(Guid.NewGuid(), course.Id, DateTime.UtcNow.AddDays(1), 100m, 10, type.Id, VenueType.InPerson);
        var created = await repo.AddAsync(input, CancellationToken.None);
        var byId = await repo.GetByIdAsync(created.Id, CancellationToken.None);
        var byCourse = await repo.GetCourseEventsByCourseIdAsync(course.Id, CancellationToken.None);

        Assert.NotNull(byId);
        Assert.Contains(byCourse, x => x.Id == created.Id);
        Assert.Equal(input.Id, created.Id);
        Assert.Equal(input.Id, byId!.Id);
        Assert.Equal(input.CourseId, byId.CourseId);
        Assert.Equal(input.CourseEventTypeId, byId.CourseEventTypeId);
        Assert.Equal(input.Seats, byId.Seats);
        Assert.Equal(input.Price, byId.Price);
        Assert.Equal(input.VenueType, byId.VenueType);
        Assert.Equal(input.CourseId, created.CourseId);
        Assert.Equal(input.CourseEventTypeId, created.CourseEventTypeId);
        Assert.Equal(input.Seats, created.Seats);
        Assert.Equal(input.Price, created.Price);
        Assert.Equal(input.VenueType, created.VenueType);

        var persisted = await context.CourseEvents
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(input.Id, persisted.Id);
        Assert.Equal(input.CourseId, persisted.CourseId);
        Assert.Equal(input.CourseEventTypeId, persisted.CourseEventTypeId);
        Assert.Equal(input.Seats, persisted.Seats);
        Assert.Equal(input.Price, persisted.Price);
        Assert.Equal((int)input.VenueType, persisted.VenueTypeId);
    }

    [Fact]
    public async Task GetAllCourseEventsAsync_ShouldContainCreatedEvent()
    {
        await using var context = fixture.CreateDbContext();
        var created = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var repo = new CourseEventRepository(context);

        var all = await repo.GetAllAsync(CancellationToken.None);

        Assert.Contains(all, x => x.Id == created.Id);
    }

    [Fact]
    public async Task UpdateCourseEventAsync_ShouldPersistChanges()
    {
        await using var context = fixture.CreateDbContext();
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var repo = new CourseEventRepository(context);

        var updated = await repo.UpdateAsync(
            courseEvent.Id,
            new CourseEvent(
                courseEvent.Id,
                courseEvent.CourseId,
                courseEvent.EventDate.AddDays(2),
                123m,
                15,
                courseEvent.CourseEventTypeId,
                VenueType.Hybrid),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(123m, updated!.Price);
        Assert.Equal(VenueType.Hybrid, updated.VenueType);

        var persisted = await context.CourseEvents
            .AsNoTracking()
            .SingleAsync(x => x.Id == courseEvent.Id, CancellationToken.None);

        Assert.Equal(123m, persisted.Price);
        Assert.Equal(15, persisted.Seats);
        Assert.Equal((int)VenueType.Hybrid, persisted.VenueTypeId);
    }

    [Fact]
    public async Task HasRegistrationsAsync_ShouldReturnTrueWhenRegistrationsExist()
    {
        await using var context = fixture.CreateDbContext();
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context, courseEventId: courseEvent.Id);
        var repo = new CourseEventRepository(context);

        var hasRegistrations = await repo.HasRegistrationsAsync(courseEvent.Id, CancellationToken.None);

        Assert.True(hasRegistrations);
    }

    [Fact]
    public async Task DeleteCourseEventAsync_ShouldRemoveEvent()
    {
        await using var context = fixture.CreateDbContext();
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var repo = new CourseEventRepository(context);

        var deleted = await repo.RemoveAsync(courseEvent.Id, CancellationToken.None);
        var loaded = await repo.GetByIdAsync(courseEvent.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(loaded);
    }
}
