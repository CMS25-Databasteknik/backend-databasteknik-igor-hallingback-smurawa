using Backend.Application.Modules.CourseEvents;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEvents.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.CourseEvents;

public class CourseEventService_Tests
{
    #region CreateCourseEventAsync Tests

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow.AddDays(30);
        var expectedEvent = new CourseEvent(Guid.NewGuid(), courseId, eventDate, 999.99m, 25, 1);

        mockRepo.CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(expectedEvent);

        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(courseId, eventDate, 999.99m, 25, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(courseId, result.Result.CourseId);
        Assert.Equal(eventDate, result.Result.EventDate);
        Assert.Equal(999.99m, result.Result.Price);
        Assert.Equal(25, result.Result.Seats);
        Assert.Equal(1, result.Result.CourseEventTypeId);
        Assert.Equal("Course event created successfully.", result.Message);

        await mockRepo.Received(1).CreateCourseEventAsync(
            Arg.Is<CourseEvent>(e => e.CourseId == courseId && e.EventDate == eventDate && e.Price == 999.99m && e.Seats == 25 && e.CourseEventTypeId == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.CreateCourseEventAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event cannot be null.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_CourseId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.Empty, DateTime.UtcNow.AddDays(30), 999.99m, 25, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_EventDate_Is_Default()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), default, 999.99m, 25, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Event date must be specified.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_Price_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(30), -100m, 25, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Price cannot be negative.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_Seats_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 0, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Seats must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_Seats_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, -10, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Seats must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_CourseEventTypeId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, 0);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_BadRequest_When_CourseEventTypeId_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, -1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        mockRepo.CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEvent>(new Exception("Database error")));

        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the course event", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(500.50, 20)]
    [InlineData(2500.99, 50)]
    public async Task CreateCourseEventAsync_Should_Create_Event_With_Various_Valid_Prices_And_Seats(
        decimal price, int seats)
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow.AddDays(30);
        var expectedEvent = new CourseEvent(Guid.NewGuid(), courseId, eventDate, price, seats, 1);

        mockRepo.CreateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(expectedEvent);

        var service = new CourseEventService(mockRepo);
        var input = new CreateCourseEventInput(courseId, eventDate, price, seats, 1);

        // Act
        var result = await service.CreateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(price, result.Result.Price);
        Assert.Equal(seats, result.Result.Seats);
    }

    [Fact]
    public void CourseEventService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CourseEventService(null!));
    }

    #endregion

    #region GetAllCourseEventsAsync Tests

    [Fact]
    public async Task GetAllCourseEventsAsync_Should_Return_All_Events_When_Events_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();
        var events = new List<CourseEvent>
        {
            new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(10), 999.99m, 20, 1),
            new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(20), 1299.99m, 30, 2),
            new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(30), 1599.99m, 40, 3)
        };

        mockRepo.GetAllCourseEventsAsync(Arg.Any<CancellationToken>())
            .Returns(events);

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetAllCourseEventsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 course event(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllCourseEventsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCourseEventsAsync_Should_Return_Empty_List_When_No_Events_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        mockRepo.GetAllCourseEventsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseEvent>());

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetAllCourseEventsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course events found.", result.Message);
    }

    [Fact]
    public async Task GetAllCourseEventsAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        mockRepo.GetAllCourseEventsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseEvent>>(new Exception("Database connection failed")));

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetAllCourseEventsAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving course events", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetCourseEventByIdAsync Tests

    [Fact]
    public async Task GetCourseEventByIdAsync_Should_Return_Event_When_Event_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var eventDate = DateTime.UtcNow.AddDays(30);
        var courseEvent = new CourseEvent(eventId, courseId, eventDate, 999.99m, 25, 1);

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(courseEvent);

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventByIdAsync(eventId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(eventId, result.Result.Id);
        Assert.Equal(courseId, result.Result.CourseId);
        Assert.Equal("Course event retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventByIdAsync_Should_Return_NotFound_When_Event_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns((CourseEvent)null!);

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventByIdAsync(eventId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course event with ID '{eventId}' not found", result.Message);
    }

    [Fact]
    public async Task GetCourseEventByIdAsync_Should_Return_BadRequest_When_EventId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventByIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseEventByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEvent?>(new Exception("Database error")));

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventByIdAsync(eventId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the course event", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region GetCourseEventsByCourseIdAsync Tests

    [Fact]
    public async Task GetCourseEventsByCourseIdAsync_Should_Return_Events_When_Events_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();
        var events = new List<CourseEvent>
        {
            new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(10), 999.99m, 20, 1),
            new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(20), 1299.99m, 30, 2)
        };

        mockRepo.GetCourseEventsByCourseIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(events);

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventsByCourseIdAsync(courseId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count());
        Assert.Equal("Retrieved 2 course event(s) for the course successfully.", result.Message);

        await mockRepo.Received(1).GetCourseEventsByCourseIdAsync(courseId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventsByCourseIdAsync_Should_Return_Empty_List_When_No_Events_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();

        mockRepo.GetCourseEventsByCourseIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(new List<CourseEvent>());

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventsByCourseIdAsync(courseId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course events found for this course.", result.Message);
    }

    [Fact]
    public async Task GetCourseEventsByCourseIdAsync_Should_Return_BadRequest_When_CourseId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventsByCourseIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Course ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseEventsByCourseIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventsByCourseIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();

        mockRepo.GetCourseEventsByCourseIdAsync(courseId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseEvent>>(new Exception("Database error")));

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.GetCourseEventsByCourseIdAsync(courseId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving course events", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateCourseEventAsync Tests

    [Fact]
    public async Task UpdateCourseEventAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var oldDate = DateTime.UtcNow.AddDays(30);
        var newDate = DateTime.UtcNow.AddDays(40);
        var existingEvent = new CourseEvent(eventId, courseId, oldDate, 999.99m, 20, 1);
        var updatedEvent = new CourseEvent(eventId, courseId, newDate, 1299.99m, 30, 2);

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(existingEvent);

        mockRepo.UpdateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(updatedEvent);

        var service = new CourseEventService(mockRepo);
        var input = new UpdateCourseEventInput(eventId, courseId, newDate, 1299.99m, 30, 2);

        // Act
        var result = await service.UpdateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(eventId, result.Result.Id);
        Assert.Equal(courseId, result.Result.CourseId);
        Assert.Equal(newDate, result.Result.EventDate);
        Assert.Equal(1299.99m, result.Result.Price);
        Assert.Equal(30, result.Result.Seats);
        Assert.Equal(2, result.Result.CourseEventTypeId);
        Assert.Equal("Course event updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateCourseEventAsync(
            Arg.Is<CourseEvent>(e => e.Id == eventId && e.CourseId == courseId && e.EventDate == newDate && e.Price == 1299.99m && e.Seats == 30 && e.CourseEventTypeId == 2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseEventAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.UpdateCourseEventAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseEventAsync_Should_Return_BadRequest_When_EventId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);
        var input = new UpdateCourseEventInput(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, 1);

        // Act
        var result = await service.UpdateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseEventByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseEventAsync_Should_Return_NotFound_When_Event_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns((CourseEvent)null!);

        var service = new CourseEventService(mockRepo);
        var input = new UpdateCourseEventInput(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, 1);

        // Act
        var result = await service.UpdateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course event with ID '{eventId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseEventAsync_Should_Return_Conflict_When_Concurrency_Exception_Occurs()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var existingEvent = new CourseEvent(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 20, 1);

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(existingEvent);

        mockRepo.UpdateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEvent?>(new InvalidOperationException("Course event was modified by another user")));

        var service = new CourseEventService(mockRepo);
        var input = new UpdateCourseEventInput(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(40), 1299.99m, 30, 2);

        // Act
        var result = await service.UpdateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("The course event was modified by another user. Please refresh and try again.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var existingEvent = new CourseEvent(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 20, 1);

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(existingEvent);

        mockRepo.UpdateCourseEventAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEvent?>(new Exception("Database error")));

        var service = new CourseEventService(mockRepo);
        var input = new UpdateCourseEventInput(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(40), 1299.99m, 30, 2);

        // Act
        var result = await service.UpdateCourseEventAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the course event", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteCourseEventAsync Tests

    [Fact]
    public async Task DeleteCourseEventAsync_Should_Return_Success_When_Event_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var existingEvent = new CourseEvent(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, 1);

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(existingEvent);

        mockRepo.DeleteCourseEventAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.DeleteCourseEventAsync(eventId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        Assert.Equal("Course event deleted successfully.", result.Message);

        await mockRepo.Received(1).DeleteCourseEventAsync(eventId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventAsync_Should_Return_BadRequest_When_EventId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.DeleteCourseEventAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Course event ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseEventAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventAsync_Should_Return_NotFound_When_Event_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns((CourseEvent)null!);

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.DeleteCourseEventAsync(eventId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Course event with ID '{eventId}' not found", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseEventAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var existingEvent = new CourseEvent(eventId, Guid.NewGuid(), DateTime.UtcNow.AddDays(30), 999.99m, 25, 1);

        mockRepo.GetCourseEventByIdAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(existingEvent);

        mockRepo.DeleteCourseEventAsync(eventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = new CourseEventService(mockRepo);

        // Act
        var result = await service.DeleteCourseEventAsync(eventId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the course event", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion
}


