using Backend.Application.Modules.Courses;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.Courses.Models;
using NSubstitute;

namespace Backend.Infrastructure.Tests.Persistence.EFC.Services;

public class CourseService_Tests
{
    [Fact]
    public async Task CreateCourseAsync_Should_Create_Course()
    {
        // Arrange
        var repo = Substitute.For<ICoursesRepository>();
        await repo.CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());

        // Act
        var service = new CourseService(repo);
        var input = new CreateCourseInput("Test Course", "Test Description", 5);

        // Assert

    }
}
