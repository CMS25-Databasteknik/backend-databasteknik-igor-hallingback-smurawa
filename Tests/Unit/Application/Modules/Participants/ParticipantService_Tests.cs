using Backend.Application.Common;
using Backend.Application.Modules.Participants;
using Backend.Application.Modules.Participants.Inputs;
using Backend.Domain.Modules.ParticipantContactTypes.Contracts;
using Backend.Domain.Modules.ParticipantContactTypes.Models;
using Backend.Domain.Modules.Participants.Contracts;
using Backend.Domain.Modules.Participants.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.Participants;

public class ParticipantService_Tests
{
    private static ParticipantService CreateService(
        IParticipantRepository? participantRepository = null,
        IParticipantContactTypeRepository? contactTypeRepository = null)
    {
        var repo = participantRepository ?? Substitute.For<IParticipantRepository>();
        var contactRepo = contactTypeRepository ?? Substitute.For<IParticipantContactTypeRepository>();

        contactRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var id = ci.Arg<int>();
                return id <= 0 ? null : new ParticipantContactType(id, id == 2 ? "Billing" : "Primary");
            });

        return new ParticipantService(repo, contactRepo);
    }

    #region CreateParticipantAsync Tests

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var expectedParticipant = new Participant(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(expectedParticipant);

        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "Doe", "john.doe@example.com", "+46701234567", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal("John", result.Result.FirstName);
        Assert.Equal("Doe", result.Result.LastName);
        Assert.Equal("john.doe@example.com", result.Result.Email);
        Assert.Equal("+46701234567", result.Result.PhoneNumber);
        Assert.Equal("Participant created successfully.", result.Message);

        await mockRepo.Received(1).AddAsync(
            Arg.Is<Participant>(p => p.FirstName == "John" && p.LastName == "Doe" && p.Email == "john.doe@example.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);

        // Act
        var result = await service.CreateParticipantAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Participant cannot be null.", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_FirstName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput(string.Empty, "Doe", "john.doe@example.com", "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_FirstName_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("   ", "Doe", "john.doe@example.com", "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_LastName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", string.Empty, "john.doe@example.com", "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_LastName_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "   ", "john.doe@example.com", "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_Email_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "Doe", string.Empty, "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_Email_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "Doe", "   ", "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_PhoneNumber_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "Doe", "john.doe@example.com", string.Empty, 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_BadRequest_When_PhoneNumber_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "Doe", "john.doe@example.com", "   ", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateParticipantAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Participant>(new Exception("Database error")));

        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput("John", "Doe", "john.doe@example.com", "1234567890", 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the participant", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData("Alice", "Smith", "alice.smith@example.com", "+46709876543")]
    [InlineData("Bob", "Johnson", "bob.johnson@example.com", "+46701111111")]
    [InlineData("Charlie", "Brown", "charlie.brown@example.com", "+46702222222")]
    public async Task CreateParticipantAsync_Should_Create_Participant_With_Various_Valid_Inputs(
        string firstName, string lastName, string email, string phoneNumber)
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var expectedParticipant = new Participant(Guid.NewGuid(), firstName, lastName, email, phoneNumber);

        mockRepo.AddAsync(Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(expectedParticipant);

        var service = CreateService(mockRepo);
        var input = new CreateParticipantInput(firstName, lastName, email, phoneNumber, 1);

        // Act
        var result = await service.CreateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(firstName, result.Result.FirstName);
        Assert.Equal(lastName, result.Result.LastName);
        Assert.Equal(email, result.Result.Email);
        Assert.Equal(phoneNumber, result.Result.PhoneNumber);
    }

    [Fact]
    public void ParticipantService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ParticipantService(null!, Substitute.For<IParticipantContactTypeRepository>()));
    }

    #endregion

    #region GetAllParticipantsAsync Tests

    [Fact]
    public async Task GetAllParticipantsAsync_Should_Return_All_Participants_When_Participants_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participants = new List<Participant>
        {
            new Participant(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "+46701234567"),
            new Participant(Guid.NewGuid(), "Jane", "Smith", "jane.smith@example.com", "+46709876543"),
            new Participant(Guid.NewGuid(), "Bob", "Johnson", "bob.johnson@example.com", "+46701111111")
        };

        mockRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(participants);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetAllParticipantsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 participant(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllParticipantsAsync_Should_Return_Empty_List_When_No_Participants_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Participant>());

        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetAllParticipantsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No participants found.", result.Message);
    }

    [Fact]
    public async Task GetAllParticipantsAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<Participant>>(new Exception("Database connection failed")));

        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetAllParticipantsAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Contains("An error occurred while retrieving participants", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetParticipantByIdAsync Tests

    [Fact]
    public async Task GetParticipantByIdAsync_Should_Return_Participant_When_Participant_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var participant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(participant);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetParticipantByIdAsync(participantId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(participantId, result.Result.Id);
        Assert.Equal("John", result.Result.FirstName);
        Assert.Equal("Participant retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetByIdAsync(participantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetParticipantByIdAsync_Should_Return_NotFound_When_Participant_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns((Participant)null!);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetParticipantByIdAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Null(result.Result);
        Assert.Contains($"Participant with ID '{participantId}' not found", result.Message);
    }

    [Fact]
    public async Task GetParticipantByIdAsync_Should_Return_BadRequest_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetParticipantByIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Participant ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetParticipantByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Participant?>(new Exception("Database error")));

        var service = CreateService(mockRepo);

        // Act
        var result = await service.GetParticipantByIdAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the participant", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateParticipantAsync Tests

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");
        var updatedParticipant = new Participant(participantId, "John", "Smith", "john.smith@example.com", "+46709876543");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(updatedParticipant);

        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(participantId, "John", "Smith", "john.smith@example.com", "+46709876543", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal("Smith", result.Result.LastName);
        Assert.Equal("john.smith@example.com", result.Result.Email);
        Assert.Equal("Participant updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateAsync(
            Arg.Is<Guid>(id => id == participantId),
            Arg.Is<Participant>(p => p.Id == participantId && p.LastName == "Smith"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);

        // Act
        var result = await service.UpdateParticipantAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Participant cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateAsync(Arg.Any<Guid>(), Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_BadRequest_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(Guid.Empty, "John", "Doe", "john.doe@example.com", "+46701234567", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Participant ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_BadRequest_When_FirstName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Participant(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "+46701234567"));
        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(Guid.NewGuid(), "", "Doe", "john.doe@example.com", "+46701234567", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_BadRequest_When_LastName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Participant(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "+46701234567"));
        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(Guid.NewGuid(), "John", "", "john.doe@example.com", "+46701234567", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_BadRequest_When_Email_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Participant(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "+46701234567"));
        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(Guid.NewGuid(), "John", "Doe", "", "+46701234567", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_BadRequest_When_PhoneNumber_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        mockRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Participant(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "+46701234567"));
        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_NotFound_When_Participant_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns((Participant)null!);

        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(participantId, "John", "Doe", "john.doe@example.com", "+46701234567", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Null(result.Result);
        Assert.Contains($"Participant with ID '{participantId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateAsync(Arg.Any<Guid>(), Arg.Any<Participant>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_Conflict_When_Concurrency_Exception_Occurs()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Participant?>(new InvalidOperationException("Participant was modified by another user")));

        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(participantId, "John", "Smith", "john.smith@example.com", "+46709876543", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Conflict, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("The participant was modified by another user. Please refresh and try again.", result.Message);
    }

    [Fact]
    public async Task UpdateParticipantAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Participant>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Participant?>(new Exception("Database error")));

        var service = CreateService(mockRepo);
        var input = new UpdateParticipantInput(participantId, "John", "Smith", "john.smith@example.com", "+46709876543", 1);

        // Act
        var result = await service.UpdateParticipantAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the participant", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteParticipantAsync Tests

    [Fact]
    public async Task DeleteParticipantAsync_Should_Return_Success_When_Participant_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.HasRegistrationsAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.RemoveAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.DeleteParticipantAsync(participantId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.True(result.Result);
        Assert.Equal("Participant deleted successfully.", result.Message);

        await mockRepo.Received(1).RemoveAsync(participantId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteParticipantAsync_Should_Return_BadRequest_When_ParticipantId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var service = CreateService(mockRepo);

        // Act
        var result = await service.DeleteParticipantAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.False(result.Result);
        Assert.Equal("Participant ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteParticipantAsync_Should_Return_NotFound_When_Participant_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns((Participant)null!);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.DeleteParticipantAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.False(result.Result);
        Assert.Contains($"Participant with ID '{participantId}' not found", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteParticipantAsync_Should_Return_Conflict_When_Participant_Has_Registrations()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.HasRegistrationsAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.DeleteParticipantAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Conflict, result.Error);
        Assert.False(result.Result);
        Assert.Contains("Cannot delete participant", result.Message);
        Assert.Contains("they have course registrations", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteParticipantAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.HasRegistrationsAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.RemoveAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = CreateService(mockRepo);

        // Act
        var result = await service.DeleteParticipantAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the participant", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Fact]
    public async Task DeleteParticipantAsync_Should_Return_InternalServerError_When_Delete_Returns_False()
    {
        // Arrange
        var mockRepo = Substitute.For<IParticipantRepository>();
        var participantId = Guid.NewGuid();
        var existingParticipant = new Participant(participantId, "John", "Doe", "john.doe@example.com", "+46701234567");

        mockRepo.GetByIdAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(existingParticipant);

        mockRepo.HasRegistrationsAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.RemoveAsync(participantId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = CreateService(mockRepo);

        // Act
        var result = await service.DeleteParticipantAsync(participantId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.False(result.Result);
        Assert.Equal("Failed to delete participant.", result.Message);
    }

    #endregion
}

