using Backend.Domain.Modules.Courses.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CoursesRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateCourseAsync_ShouldAddCourseToDatabase_And_Return_Course()
    {
        // Arrange
        await using var context = fixture.CreateDbContext();
        await ClearCoursesAsync(context);

        var repo = new CourseRepository(context);
        var input = new Course(Guid.NewGuid(), "Test Course", "A course for testing", 5);

        // Act
        var course = await repo.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(course);
        Assert.Equal(input.Id, course.Id);
        Assert.Equal(input.Title, course.Title);
        Assert.Equal(input.Description, course.Description);
        Assert.Equal(input.DurationInDays, course.DurationInDays);
    }

    private static async Task ClearCoursesAsync(CoursesOnlineDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Courses;");
    }
}
