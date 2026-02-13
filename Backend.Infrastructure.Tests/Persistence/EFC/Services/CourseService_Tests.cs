using Backend.Application.Modules.Courses;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Domain.Modules.CourseEvents.Models;
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
            .Returns(Task.FromException<Course>(new Exception("Database error")));

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

    #region GetAllCoursesAsync Tests

    [Fact]
    public async Task GetAllCoursesAsync_Should_Return_All_Courses_When_Courses_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courses = new List<Course>
        {
            new Course(Guid.NewGuid(), "Course 1", "Description 1", 10),
            new Course(Guid.NewGuid(), "Course 2", "Description 2", 20),
            new Course(Guid.NewGuid(), "Course 3", "Description 3", 15)
        };

        mockRepo.GetAllCoursesAsync(Arg.Any<CancellationToken>())
            .Returns(courses);

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetAllCoursesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 course(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllCoursesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCoursesAsync_Should_Return_Empty_List_When_No_Courses_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        mockRepo.GetAllCoursesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Course>());

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetAllCoursesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No courses found.", result.Message);
    }

    [Fact]
    public async Task GetAllCoursesAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        mockRepo.GetAllCoursesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<Course>>(new Exception("Database connection failed")));

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetAllCoursesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving courses", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetCourseByIdAsync Tests

    [Fact]
    public async Task GetCourseByIdAsync_Should_Return_Course_When_Course_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var course = new Course(courseId, "Test Course", "Test Description", 10);
        var courseWithEvents = new CourseWithEvents(course, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetCourseByIdAsync(courseId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(courseId, result.Result.Course.Id);
        Assert.Equal("Test Course", result.Result.Course.Title);
        Assert.Equal("Course retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseByIdAsync_Should_Return_NotFound_When_Course_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((CourseWithEvents)null!);

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetCourseByIdAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course with ID '{courseId}' not found", result.Message);
    }

    [Fact]
    public async Task GetCourseByIdAsync_Should_Return_BadRequest_When_CourseId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetCourseByIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseWithEvents?>(new Exception("Database error")));

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.GetCourseByIdAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the course", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateCourseAsync Tests

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Old Title", "Old Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());
        var updatedCourse = new Course(courseId, "Updated Title", "Updated Description", 10);

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.UpdateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(updatedCourse);

        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(courseId, "Updated Title", "Updated Description", 10);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Updated Title", result.Result.Title);
        Assert.Equal("Updated Description", result.Result.Description);
        Assert.Equal(10, result.Result.DurationInDays);
        Assert.Equal("Course updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateCourseAsync(
            Arg.Is<Course>(c => c.Id == courseId && c.Title == "Updated Title" && c.Description == "Updated Description" && c.DurationInDays == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);

        // Act
        var result = await service.UpdateCourseAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_BadRequest_When_CourseId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(Guid.Empty, "Test Title", "Test Description", 5);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_BadRequest_When_Title_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(Guid.NewGuid(), "", "Test Description", 5);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course title cannot be empty or whitespace.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_BadRequest_When_Description_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(Guid.NewGuid(), "Test Title", "", 5);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course description cannot be empty or whitespace.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_BadRequest_When_DurationInDays_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(Guid.NewGuid(), "Test Title", "Test Description", 0);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course duration must be greater than zero.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_NotFound_When_Course_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((CourseWithEvents)null!);

        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(courseId, "Test Title", "Test Description", 5);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course with ID '{courseId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_Conflict_When_Concurrency_Exception_Occurs()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Old Title", "Old Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.UpdateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Course?>(new InvalidOperationException("Course was modified by another user")));

        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(courseId, "Updated Title", "Updated Description", 10);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("The course was modified by another user. Please refresh and try again.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Old Title", "Old Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.UpdateCourseAsync(Arg.Any<Course>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Course?>(new Exception("Database error")));

        var service = new CourseService(mockRepo);
        var input = new UpdateCourseInput(courseId, "Updated Title", "Updated Description", 10);

        // Act
        var result = await service.UpdateCourseAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the course", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteCourseAsync Tests

    [Fact]
    public async Task DeleteCourseAsync_Should_Return_Success_When_Course_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Test Course", "Test Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.DeleteCourseAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.DeleteCourseAsync(courseId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        Assert.Equal("Course deleted successfully.", result.Message);

        await mockRepo.Received(1).DeleteCourseAsync(courseId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseAsync_Should_Return_BadRequest_When_CourseId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var service = new CourseService(mockRepo);

        // Act
        var result = await service.DeleteCourseAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Course ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseAsync_Should_Return_NotFound_When_Course_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns((CourseWithEvents)null!);

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.DeleteCourseAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Course with ID '{courseId}' not found", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseAsync_Should_Return_Conflict_When_Course_Has_Associated_Events()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Test Course", "Test Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.DeleteCourseAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new InvalidOperationException("Cannot delete course with associated course events")));

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.DeleteCourseAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("Cannot delete course because it has associated course events", result.Message);
    }

    [Fact]
    public async Task DeleteCourseAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Test Course", "Test Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.DeleteCourseAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.DeleteCourseAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the course", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Fact]
    public async Task DeleteCourseAsync_Should_Return_InternalServerError_When_Delete_Returns_False()
    {
        // Arrange
        var mockRepo = Substitute.For<ICoursesRepository>();
        var courseId = Guid.NewGuid();
        var existingCourse = new Course(courseId, "Test Course", "Test Description", 5);
        var courseWithEvents = new CourseWithEvents(existingCourse, new List<CourseEvent>());

        mockRepo.GetCourseByIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(courseWithEvents);

        mockRepo.DeleteCourseAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = new CourseService(mockRepo);

        // Act
        var result = await service.DeleteCourseAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Failed to delete course.", result.Message);
    }

    #endregion
}
