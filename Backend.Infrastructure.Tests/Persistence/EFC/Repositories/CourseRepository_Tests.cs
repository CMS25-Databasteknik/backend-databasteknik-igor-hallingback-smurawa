using Backend.Domain.Modules.Courses.Models;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backend.Infrastructure.Tests.Persistence.EFC.Repositories;

public class CourseRepository_Tests
{
    private class TestSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                var entries = eventData.Context.ChangeTracker.Entries<CourseEntity>();
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.Concurrency ??= new byte[8];
                        if (entry.Entity.CreatedAtUtc == default)
                            entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                        if (entry.Entity.ModifiedAtUtc == default)
                            entry.Entity.ModifiedAtUtc = DateTime.UtcNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.Concurrency ??= new byte[8];
                        entry.Entity.ModifiedAtUtc = DateTime.UtcNow;
                    }
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

    private CoursesOnlineDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CoursesOnlineDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .AddInterceptors(new TestSaveChangesInterceptor())
            .Options;

        return new CoursesOnlineDbContext(options);
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldCreateCourse()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 5);

        // Act
        var result = await repository.CreateCourseAsync(course, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
        Assert.Equal(course.Title, result.Title);
        Assert.Equal(course.Description, result.Description);
        Assert.Equal(course.DurationInDays, result.DurationInDays);
    }

    [Fact]
    public async Task GetAllCoursesAsync_ShouldReturnAllCourses()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);

        var course1 = new Course(Guid.NewGuid(), "Course 1", "Description 1", 5);
        var course2 = new Course(Guid.NewGuid(), "Course 2", "Description 2", 10);

        await repository.CreateCourseAsync(course1, CancellationToken.None);
        await repository.CreateCourseAsync(course2, CancellationToken.None);

        // Act
        var result = await repository.GetAllCoursesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllCoursesAsync_ShouldReturnEmptyList_WhenNoCoursesExist()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);

        // Act
        var result = await repository.GetAllCoursesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 5);

        await repository.CreateCourseAsync(course, CancellationToken.None);

        // Act
        var result = await repository.GetCourseByIdAsync(course.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Course.Id);
        Assert.Equal(course.Title, result.Course.Title);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ShouldReturnNull_WhenCourseDoesNotExist()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);

        // Act
        var result = await repository.GetCourseByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldUpdateCourse_WhenCourseExists()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);
        var course = new Course(Guid.NewGuid(), "Original Title", "Original Description", 5);

        await repository.CreateCourseAsync(course, CancellationToken.None);

        var updatedCourse = new Course(course.Id, "Updated Title", "Updated Description", 10);

        // Act
        var result = await repository.UpdateCourseAsync(updatedCourse, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedCourse.Id, result.Id);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(10, result.DurationInDays);
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldThrowKeyNotFoundException_WhenCourseDoesNotExist()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 5);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            repository.UpdateCourseAsync(course, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCourseAsync_ShouldDeleteCourse_WhenCourseExists()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 5);

        await repository.CreateCourseAsync(course, CancellationToken.None);

        // Act
        var result = await repository.DeleteCourseAsync(course.Id, CancellationToken.None);

        // Assert
        Assert.True(result);
        var deletedCourse = await repository.GetCourseByIdAsync(course.Id, CancellationToken.None);
        Assert.Null(deletedCourse);
    }

    [Fact]
    public async Task DeleteCourseAsync_ShouldThrowKeyNotFoundException_WhenCourseDoesNotExist()
    {
        // Arrange
        await using var context = CreateInMemoryContext();
        var repository = new CourseRepository(context);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            repository.DeleteCourseAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public void ToModel_ShouldMapEntityToModel()
    {
        // Arrange
        var entity = new CourseEntity
        {
            Id = Guid.NewGuid(),
            Title = "Test Course",
            Description = "Test Description",
            DurationInDays = 5
        };

        // Act
        var result = CourseRepository.ToModel(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Title, result.Title);
        Assert.Equal(entity.Description, result.Description);
        Assert.Equal(entity.DurationInDays, result.DurationInDays);
    }
}
