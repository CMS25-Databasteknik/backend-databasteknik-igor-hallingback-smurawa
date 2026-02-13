using Backend.Application.Modules.Courses;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.Courses.Models;
using NSubstitute;

namespace Backend.Infrastructure.Tests.Persistence.EFC.Services;

public class CourseService_Tests
{
    [Fact]
    public async Task CreateCourseAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var expectedCourse = new Course(Guid.NewGuid(), "Test Course", "Test Description", 5);

        mockRepo.CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(expectedCourse);

        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("Test Course", "Test Description", 5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Test Course", result.Result.Title);
        Assert.Equal("Test Description", result.Result.Description);
        Assert.Equal(5, result.Result.DurationInDays);
        Assert.Equal("Course created successfully.", result.Message);

        await mockRepo.Received(1).CreateCourseAsync(
            Arg.Is<Course>(c => c.Title == "Test Course" && c.Description == "Test Description" && c.DurationInDays == 5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);

        // Act
        var result = await service.CreateCourseAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course cannot be null.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_Title_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("", "Test Description", 5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course title cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_Title_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("   ", "Test Description", 5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course title cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_Description_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("Test Course", "", 5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course description cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_Description_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("Test Course", "   ", 5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course description cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_DurationInDays_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("Test Course", "Test Description", 0);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course duration must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_BadRequest_When_DurationInDays_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("Test Course", "Test Description", -5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course duration must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        mockRepo.CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns<Course>(_ => throw new Exception("Database error"));

        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput("Test Course", "Test Description", 5);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the course", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData("C# Basics", "Learn C#", 10)]
    [InlineData("Advanced ASP.NET", "Master ASP.NET Core", 20)]
    [InlineData("Entity Framework", "Database access with EF", 15)]
    public async Task CreateCourseAsync_Should_Create_Course_With_Various_Valid_Inputs(
        string title, string description, int duration)
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var expectedCourse = new Course(Guid.NewGuid(), title, description, duration);

        mockRepo.CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(expectedCourse);

        var service = new CourseService(mockRepo);
        var input = new CreateCourseInput(title, description, duration);

        // Act
        var result = await service.CreateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(title, result.Result.Title);
        Assert.Equal(description, result.Result.Description);
        Assert.Equal(duration, result.Result.DurationInDays);
    }

    [Fact]
    public async Task CreateCourseAsync_Should_Generate_Unique_Guid_For_Each_Course()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var capturedGuids = new List<Guid>();

        mockRepo.CreateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var course = callInfo.Arg<Course>();
                capturedGuids.Add(course.Id);
                return course;
            });

        var service = new CourseService(mockRepo);
        var input1 = new CreateCourseInput("Course 1", "Description 1", 5);
        var input2 = new CreateCourseInput("Course 2", "Description 2", 10);

        // Act
        await service.CreateCourseAsync(input1, CancellationToken.None);
        await service.CreateCourseAsync(input2, CancellationToken.None);

        // Assert
        Assert.Equal(2, capturedGuids.Count);
        Assert.NotEqual(capturedGuids[0], capturedGuids[1]);
        Assert.NotEqual(Guid.Empty, capturedGuids[0]);
        Assert.NotEqual(Guid.Empty, capturedGuids[1]);
    }

    [Fact]
    public void CourseService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CourseService(null!));
    }
}
