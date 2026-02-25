using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CourseEventRepository_Tests(SqliteInMemoryFixture fixture)
{
    private sealed class TestableCourseEventRepository(CoursesOnlineDbContext context)
        : CourseEventRepository(context)
    {
        public CourseEvent MapToModel(CourseEventEntity entity) => base.ToModel(entity);
    }

    [Fact]
    public async Task CreateCourseEventAsync_ShouldPersist_And_BeReadableByIdAndCourseId()
    {
        await using var context = fixture.CreateDbContext();
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        var type = await RepositoryTestDataHelper.CreateCourseEventTypeAsync(context);
        var repo = new CourseEventRepository(context);

        var input = new CourseEvent(Guid.NewGuid(), course.Id, DateTime.UtcNow.AddDays(1), 100m, 10, type.Id, new VenueType(1, "InPerson"));
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
        Assert.Equal(input.VenueType.Id, persisted.VenueTypeId);
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
                new VenueType(3, "Hybrid")),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(123m, updated!.Price);
        Assert.Equal(new VenueType(3, "Hybrid"), updated.VenueType);

        var persisted = await context.CourseEvents
            .AsNoTracking()
            .SingleAsync(x => x.Id == courseEvent.Id, CancellationToken.None);

        Assert.Equal(123m, persisted.Price);
        Assert.Equal(15, persisted.Seats);
        Assert.Equal(new VenueType(3, "Hybrid").Id, persisted.VenueTypeId);
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

    [Fact]
    public async Task GetCourseEventByIdAsync_ShouldIncludeJoinedCourseEventType()
    {
        await using var context = fixture.CreateDbContext();
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        var type = await new CourseEventTypeRepository(context)
            .AddAsync(new Backend.Domain.Modules.CourseEventTypes.Models.CourseEventType("Workshop"), CancellationToken.None);
        var repo = new CourseEventRepository(context);

        var created = await repo.AddAsync(
            new CourseEvent(
                Guid.NewGuid(),
                course.Id,
                DateTime.UtcNow.AddDays(2),
                249m,
                12,
                type.Id,
                new VenueType(2, "Online")),
            CancellationToken.None);

        var loaded = await repo.GetByIdAsync(created.Id, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(type.Id, loaded!.CourseEventType.Id);
        Assert.Equal("Workshop", loaded.CourseEventType.TypeName);
        Assert.Equal(new VenueType(2, "Online").Id, loaded.VenueType.Id);
    }

    [Fact]
    public async Task GetCourseEventByIdAsync_ShouldReturnNull_WhenCourseEventDoesNotExist()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseEventRepository(context);

        var loaded = await repo.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(loaded);
    }

    [Fact]
    public async Task UpdateCourseEventAsync_ShouldThrow_WhenCourseEventDoesNotExist()
    {
        await using var context = fixture.CreateDbContext();
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        var type = await RepositoryTestDataHelper.CreateCourseEventTypeAsync(context);
        var repo = new CourseEventRepository(context);
        var missingId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            repo.UpdateAsync(
                missingId,
                new CourseEvent(
                    missingId,
                    course.Id,
                    DateTime.UtcNow.AddDays(1),
                    149m,
                    20,
                    type.Id,
                    new VenueType(1, "InPerson")),
                CancellationToken.None));
    }

    [Fact]
    public async Task CreateCourseEventAsync_ShouldThrow_WhenForeignKeysAreInvalid()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseEventRepository(context);

        var input = new CourseEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            199m,
            10,
            999_999,
            new VenueType(1, "InPerson"));

        await Assert.ThrowsAsync<DbUpdateException>(() => repo.AddAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ToModel_ShouldThrow_WhenCourseEventTypeIsNotLoaded()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new TestableCourseEventRepository(context);
        var entity = new CourseEventEntity
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            EventDate = DateTime.UtcNow.AddDays(1),
            Price = 100m,
            Seats = 10,
            CourseEventTypeId = 1,
            VenueTypeId = 1,
            VenueType = new VenueTypeEntity { Id = 1, Name = "InPerson" },
            CourseEventType = null!
        };

        var ex = Assert.Throws<InvalidOperationException>(() => repo.MapToModel(entity));
        Assert.Equal("Course event type must be loaded from database.", ex.Message);
    }

    [Fact]
    public async Task ToModel_ShouldThrow_WhenVenueTypeIsNotLoaded()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new TestableCourseEventRepository(context);
        var entity = new CourseEventEntity
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            EventDate = DateTime.UtcNow.AddDays(1),
            Price = 100m,
            Seats = 10,
            CourseEventTypeId = 1,
            VenueTypeId = 1,
            CourseEventType = new CourseEventTypeEntity { Id = 1, TypeName = "Online" },
            VenueType = null!
        };

        var ex = Assert.Throws<InvalidOperationException>(() => repo.MapToModel(entity));
        Assert.Equal("Venue type must be loaded from database.", ex.Message);
    }
}

