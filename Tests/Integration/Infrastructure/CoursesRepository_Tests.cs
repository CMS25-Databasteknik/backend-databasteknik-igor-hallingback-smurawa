using Backend.Domain.Modules.Courses.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CoursesRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateCourseAsync_ShouldAddCourseToDatabase_And_Return_Course()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRepository(context);
        var input = new Course(Guid.NewGuid(), $"Test Course {Guid.NewGuid():N}", "A course for testing", 5);

        var course = await repo.AddAsync(input, CancellationToken.None);

        Assert.NotNull(course);
        Assert.Equal(input.Id, course.Id);
        Assert.Equal(input.Title, course.Title);
        Assert.Equal(input.Description, course.Description);
        Assert.Equal(input.DurationInDays, course.DurationInDays);

        var persisted = await context.Courses
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == input.Id, CancellationToken.None);

        Assert.NotNull(persisted);
        Assert.Equal(input.Id, persisted!.Id);
        Assert.Equal(input.Title, persisted!.Title);
        Assert.Equal(input.Description, persisted.Description);
        Assert.Equal(input.DurationInDays, persisted.DurationInDays);
    }

    [Fact]
    public async Task GetAllCoursesAsync_ShouldReturnCreatedCourses()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRepository(context);

        await repo.AddAsync(new Course(Guid.NewGuid(), $"C-{Guid.NewGuid():N}", "D1", 1), CancellationToken.None);
        await repo.AddAsync(new Course(Guid.NewGuid(), $"C-{Guid.NewGuid():N}", "D2", 2), CancellationToken.None);

        var courses = await repo.GetAllAsync(CancellationToken.None);

        Assert.True(courses.Count >= 2);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourseWithEvents()
    {
        await using var context = fixture.CreateDbContext();
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        await RepositoryTestDataHelper.CreateCourseEventAsync(context, course.Id);
        var repo = new CourseRepository(context);

        var loaded = await repo.GetByIdWithEventsAsync(course.Id, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(course.Id, loaded!.Course.Id);
        Assert.NotEmpty(loaded.Events);
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldPersistChanges()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRepository(context);
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);

        var updated = await repo.UpdateAsync(
            course.Id,
            new Course(course.Id, "Updated Title", "Updated Description", 10),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated!.Title);

        var persisted = await context.Courses
            .AsNoTracking()
            .SingleAsync(x => x.Id == course.Id, CancellationToken.None);

        Assert.Equal("Updated Title", persisted.Title);
        Assert.Equal("Updated Description", persisted.Description);
        Assert.Equal(10, persisted.DurationInDays);
    }

    [Fact]
    public async Task HasCourseEventsAsync_ShouldReturnTrueWhenEventExists()
    {
        await using var context = fixture.CreateDbContext();
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);
        await RepositoryTestDataHelper.CreateCourseEventAsync(context, course.Id);
        var repo = new CourseRepository(context);

        var hasEvents = await repo.HasCourseEventsAsync(course.Id, CancellationToken.None);

        Assert.True(hasEvents);
    }

    [Fact]
    public async Task DeleteCourseAsync_ShouldRemoveCourse()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseRepository(context);
        var course = await RepositoryTestDataHelper.CreateCourseAsync(context);

        var deleted = await repo.RemoveAsync(course.Id, CancellationToken.None);
        var loaded = await repo.GetByIdWithEventsAsync(course.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(loaded);
    }
}
