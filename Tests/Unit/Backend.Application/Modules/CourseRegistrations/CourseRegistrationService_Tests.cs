using Backend.Application.Modules.CourseRegistrations;
using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Backend.Application.Modules.CourseRegistrations;

public class CourseRegistrationService_Tests
{
    #region CreateCourseRegistrationAsync Tests

    [Fact]
    public async Task CreateCourseRegistrationAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var expectedRegistration = new CourseRegistration(Guid.NewGuid(), participantId, courseEventId, DateTime.UtcNow, false);

        mockRepo.CreateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>())
            .Returns(expectedRegistration);

        var service = new CourseRegistrationService(mockRepo);
        var input = new CreateCourseRegistrationInput(participantId, courseEventId, false);

        // Act
        var result = await service.CreateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(participantId, result.Result.ParticipantId);
        Assert.Equal(courseEventId, result.Result.CourseEventId);
        Assert.False(result.Result.IsPaid);
        Assert.Equal("Course registration created successfully.", result.Message);

        await mockRepo.Received(1).CreateCourseRegistrationAsync(
            Arg.Is<CourseRegistration>(cr => cr.ParticipantId == participantId && cr.CourseEventId == courseEventId && !cr.IsPaid),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseRegistrationAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.CreateCourseRegistrationAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course registration cannot be null.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseRegistrationAsync_Should_Return_BadRequest_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);
        var input = new CreateCourseRegistrationInput(Guid.Empty, Guid.NewGuid(), false);

        // Act
        var result = await service.CreateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Participant ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseRegistrationAsync_Should_Return_BadRequest_When_CourseEventId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);
        var input = new CreateCourseRegistrationInput(Guid.NewGuid(), Guid.Empty, false);

        // Act
        var result = await service.CreateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseRegistrationAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        mockRepo.CreateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseRegistration>(new Exception("Database error")));

        var service = new CourseRegistrationService(mockRepo);
        var input = new CreateCourseRegistrationInput(Guid.NewGuid(), Guid.NewGuid(), false);

        // Act
        var result = await service.CreateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the course registration", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateCourseRegistrationAsync_Should_Create_Registration_With_Various_Payment_Status(bool isPaid)
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var expectedRegistration = new CourseRegistration(Guid.NewGuid(), participantId, courseEventId, DateTime.UtcNow, isPaid);

        mockRepo.CreateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>())
            .Returns(expectedRegistration);

        var service = new CourseRegistrationService(mockRepo);
        var input = new CreateCourseRegistrationInput(participantId, courseEventId, isPaid);

        // Act
        var result = await service.CreateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(isPaid, result.Result.IsPaid);
    }

    [Fact]
    public void CourseRegistrationService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CourseRegistrationService(null!));
    }

    #endregion

    #region GetAllCourseRegistrationsAsync Tests

    [Fact]
    public async Task GetAllCourseRegistrationsAsync_Should_Return_All_Registrations_When_Registrations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrations = new List<CourseRegistration>
        {
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false),
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, true),
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false)
        };

        mockRepo.GetAllCourseRegistrationsAsync(Arg.Any<CancellationToken>())
            .Returns(registrations);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetAllCourseRegistrationsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 course registration(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllCourseRegistrationsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCourseRegistrationsAsync_Should_Return_Empty_List_When_No_Registrations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        mockRepo.GetAllCourseRegistrationsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseRegistration>());

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetAllCourseRegistrationsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course registrations found.", result.Message);
    }

    [Fact]
    public async Task GetAllCourseRegistrationsAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        mockRepo.GetAllCourseRegistrationsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseRegistration>>(new Exception("Database connection failed")));

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetAllCourseRegistrationsAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving course registrations", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetCourseRegistrationByIdAsync Tests

    [Fact]
    public async Task GetCourseRegistrationByIdAsync_Should_Return_Registration_When_Registration_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var registration = new CourseRegistration(registrationId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, true);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(registration);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationByIdAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(registrationId, result.Result.Id);
        Assert.Equal("Course registration retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseRegistrationByIdAsync_Should_Return_NotFound_When_Registration_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns((CourseRegistration)null!);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationByIdAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course registration with ID '{registrationId}' not found", result.Message);
    }

    [Fact]
    public async Task GetCourseRegistrationByIdAsync_Should_Return_BadRequest_When_RegistrationId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationByIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course registration ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseRegistrationByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseRegistrationByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseRegistration?>(new Exception("Database error")));

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationByIdAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the course registration", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region GetCourseRegistrationsByParticipantIdAsync Tests

    [Fact]
    public async Task GetCourseRegistrationsByParticipantIdAsync_Should_Return_Registrations_When_Registrations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var participantId = Guid.NewGuid();
        var registrations = new List<CourseRegistration>
        {
            new CourseRegistration(Guid.NewGuid(), participantId, Guid.NewGuid(), DateTime.UtcNow, false),
            new CourseRegistration(Guid.NewGuid(), participantId, Guid.NewGuid(), DateTime.UtcNow, true)
        };

        mockRepo.GetCourseRegistrationsByParticipantIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(registrations);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByParticipantIdAsync(participantId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count());
        Assert.Equal("Retrieved 2 course registration(s) for the participant successfully.", result.Message);

        await mockRepo.Received(1).GetCourseRegistrationsByParticipantIdAsync(participantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseRegistrationsByParticipantIdAsync_Should_Return_Empty_List_When_No_Registrations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var participantId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationsByParticipantIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(new List<CourseRegistration>());

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByParticipantIdAsync(participantId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course registrations found for this participant.", result.Message);
    }

    [Fact]
    public async Task GetCourseRegistrationsByParticipantIdAsync_Should_Return_BadRequest_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByParticipantIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Participant ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseRegistrationsByParticipantIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseRegistrationsByParticipantIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var participantId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationsByParticipantIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseRegistration>>(new Exception("Database error")));

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByParticipantIdAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving course registrations", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region GetCourseRegistrationsByCourseEventIdAsync Tests

    [Fact]
    public async Task GetCourseRegistrationsByCourseEventIdAsync_Should_Return_Registrations_When_Registrations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var courseEventId = Guid.NewGuid();
        var registrations = new List<CourseRegistration>
        {
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), courseEventId, DateTime.UtcNow, false),
            new CourseRegistration(Guid.NewGuid(), Guid.NewGuid(), courseEventId, DateTime.UtcNow, true)
        };

        mockRepo.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, Arg.Any<CancellationToken>())
            .Returns(registrations);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count());
        Assert.Equal("Retrieved 2 course registration(s) for the course event successfully.", result.Message);

        await mockRepo.Received(1).GetCourseRegistrationsByCourseEventIdAsync(courseEventId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseRegistrationsByCourseEventIdAsync_Should_Return_Empty_List_When_No_Registrations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var courseEventId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, Arg.Any<CancellationToken>())
            .Returns(new List<CourseRegistration>());

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course registrations found for this course event.", result.Message);
    }

    [Fact]
    public async Task GetCourseRegistrationsByCourseEventIdAsync_Should_Return_BadRequest_When_CourseEventId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByCourseEventIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Course event ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseRegistrationsByCourseEventIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseRegistrationsByCourseEventIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var courseEventId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseRegistration>>(new Exception("Database error")));

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving course registrations", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateCourseRegistrationAsync Tests

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var courseEventId = Guid.NewGuid();
        var existingRegistration = new CourseRegistration(registrationId, participantId, courseEventId, DateTime.UtcNow, false);
        var updatedRegistration = new CourseRegistration(registrationId, participantId, courseEventId, DateTime.UtcNow, true);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(existingRegistration);

        mockRepo.UpdateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>())
            .Returns(updatedRegistration);

        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(registrationId, participantId, courseEventId, true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.True(result.Result.IsPaid);
        Assert.Equal("Course registration updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateCourseRegistrationAsync(
            Arg.Is<CourseRegistration>(cr => cr.Id == registrationId && cr.IsPaid),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course registration cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_BadRequest_When_RegistrationId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course registration ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetCourseRegistrationByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_BadRequest_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Participant ID cannot be empty.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_BadRequest_When_CourseEventId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event ID cannot be empty.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_NotFound_When_Registration_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns((CourseRegistration)null!);

        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(registrationId, Guid.NewGuid(), Guid.NewGuid(), true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course registration with ID '{registrationId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_Conflict_When_Concurrency_Exception_Occurs()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var existingRegistration = new CourseRegistration(registrationId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(existingRegistration);

        mockRepo.UpdateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseRegistration?>(new InvalidOperationException("Course registration was modified by another user")));

        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(registrationId, Guid.NewGuid(), Guid.NewGuid(), true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("The course registration was modified by another user. Please refresh and try again.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var existingRegistration = new CourseRegistration(registrationId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(existingRegistration);

        mockRepo.UpdateCourseRegistrationAsync(Arg.Any<CourseRegistration>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseRegistration?>(new Exception("Database error")));

        var service = new CourseRegistrationService(mockRepo);
        var input = new UpdateCourseRegistrationInput(registrationId, Guid.NewGuid(), Guid.NewGuid(), true);

        // Act
        var result = await service.UpdateCourseRegistrationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the course registration", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteCourseRegistrationAsync Tests

    [Fact]
    public async Task DeleteCourseRegistrationAsync_Should_Return_Success_When_Registration_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var existingRegistration = new CourseRegistration(registrationId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(existingRegistration);

        mockRepo.DeleteCourseRegistrationAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.DeleteCourseRegistrationAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        Assert.Equal("Course registration deleted successfully.", result.Message);

        await mockRepo.Received(1).DeleteCourseRegistrationAsync(registrationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseRegistrationAsync_Should_Return_BadRequest_When_RegistrationId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.DeleteCourseRegistrationAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Course registration ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseRegistrationAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseRegistrationAsync_Should_Return_NotFound_When_Registration_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns((CourseRegistration)null!);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.DeleteCourseRegistrationAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Course registration with ID '{registrationId}' not found", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseRegistrationAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseRegistrationAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var existingRegistration = new CourseRegistration(registrationId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(existingRegistration);

        mockRepo.DeleteCourseRegistrationAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.DeleteCourseRegistrationAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the course registration", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Fact]
    public async Task DeleteCourseRegistrationAsync_Should_Return_InternalServerError_When_Delete_Returns_False()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseRegistrationRepository>();
        var registrationId = Guid.NewGuid();
        var existingRegistration = new CourseRegistration(registrationId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, false);

        mockRepo.GetCourseRegistrationByIdAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(existingRegistration);

        mockRepo.DeleteCourseRegistrationAsync(registrationId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = new CourseRegistrationService(mockRepo);

        // Act
        var result = await service.DeleteCourseRegistrationAsync(registrationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Failed to delete course registration.", result.Message);
    }

    #endregion
}
