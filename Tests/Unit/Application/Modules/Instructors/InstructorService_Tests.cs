using Backend.Application.Modules.Instructors;
using Backend.Application.Modules.Instructors.Inputs;
using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.InstructorRoles.Models;
using Backend.Domain.Modules.Instructors.Contracts;
using Backend.Domain.Modules.Instructors.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.Instructors;

public class InstructorService_Tests
{
    private static IInstructorRoleRepository CreateRoleRepo()
    {
        var repo = Substitute.For<IInstructorRoleRepository>();
        repo.GetInstructorRoleByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(ci => new InstructorRole(ci.Arg<int>(), $"Role-{ci.Arg<int>()}"));
        return repo;
    }

    #region CreateInstructorAsync Tests

    [Fact]
    public async Task CreateInstructorAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var mockRoleRepo = Substitute.For<IInstructorRoleRepository>();
        mockRoleRepo.GetInstructorRoleByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new InstructorRole(1, "Lead"));

        var expectedInstructor = new Instructor(Guid.NewGuid(), "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.CreateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>())
            .Returns(expectedInstructor);

        var service = new InstructorService(mockRepo, mockRoleRepo);
        var input = new CreateInstructorInput("Dr. John Doe");

        // Act
        var result = await service.CreateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Dr. John Doe", result.Result.Name);
        Assert.Equal("Instructor created successfully.", result.Message);

        await mockRepo.Received(1).CreateInstructorAsync(
            Arg.Is<Instructor>(i => i.Name == "Dr. John Doe"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateInstructorAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.CreateInstructorAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Instructor cannot be null.", result.Message);

        await mockRepo.DidNotReceive().CreateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateInstructorAsync_Should_Return_BadRequest_When_Name_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new CreateInstructorInput("");

        // Act
        var result = await service.CreateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateInstructorAsync_Should_Return_BadRequest_When_Name_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new CreateInstructorInput("   ");

        // Act
        var result = await service.CreateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateInstructorAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        mockRepo.CreateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Instructor>(new Exception("Database error")));

        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new CreateInstructorInput("Dr. John Doe");

        // Act
        var result = await service.CreateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the instructor", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData("Dr. Jane Smith")]
    [InlineData("Prof. Robert Johnson")]
    [InlineData("Alice Williams")]
    [InlineData("Dr. O'Brien-Smith")]
    public async Task CreateInstructorAsync_Should_Create_Instructor_With_Various_Valid_Names(string name)
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var expectedInstructor = new Instructor(Guid.NewGuid(), name, new InstructorRole(1, "Lead"));

        mockRepo.CreateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>())
            .Returns(expectedInstructor);

        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new CreateInstructorInput(name);

        // Act
        var result = await service.CreateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(name, result.Result.Name);
    }

    [Fact]
    public void InstructorService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        var roleRepo = CreateRoleRepo();
        Assert.Throws<ArgumentNullException>(() => new InstructorService(null!, roleRepo));
        var instructorRepo = Substitute.For<IInstructorRepository>();
        Assert.Throws<ArgumentNullException>(() => new InstructorService(instructorRepo, null!));
    }

    #endregion

    #region GetAllInstructorsAsync Tests

    [Fact]
    public async Task GetAllInstructorsAsync_Should_Return_All_Instructors_When_Instructors_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructors = new List<Instructor>
        {
            new Instructor(Guid.NewGuid(), "Dr. John Doe", new InstructorRole(1, "Lead")),
            new Instructor(Guid.NewGuid(), "Prof. Jane Smith", new InstructorRole(1, "Lead")),
            new Instructor(Guid.NewGuid(), "Dr. Robert Johnson", new InstructorRole(1, "Lead"))
        };

        mockRepo.GetAllInstructorsAsync(Arg.Any<CancellationToken>())
            .Returns(instructors);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetAllInstructorsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 instructor(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllInstructorsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllInstructorsAsync_Should_Return_Empty_List_When_No_Instructors_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        mockRepo.GetAllInstructorsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Instructor>());

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetAllInstructorsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No instructors found.", result.Message);
    }

    [Fact]
    public async Task GetAllInstructorsAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        mockRepo.GetAllInstructorsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<Instructor>>(new Exception("Database connection failed")));

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetAllInstructorsAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving instructors", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetInstructorByIdAsync Tests

    [Fact]
    public async Task GetInstructorByIdAsync_Should_Return_Instructor_When_Instructor_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var instructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(instructor);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetInstructorByIdAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(instructorId, result.Result.Id);
        Assert.Equal("Dr. John Doe", result.Result.Name);
        Assert.Equal("Instructor retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetInstructorByIdAsync_Should_Return_NotFound_When_Instructor_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns((Instructor)null!);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetInstructorByIdAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Instructor with ID '{instructorId}' not found", result.Message);
    }

    [Fact]
    public async Task GetInstructorByIdAsync_Should_Return_BadRequest_When_InstructorId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetInstructorByIdAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Instructor ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().GetInstructorByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetInstructorByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Instructor?>(new Exception("Database error")));

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.GetInstructorByIdAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the instructor", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateInstructorAsync Tests

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var existingInstructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));
        var updatedInstructor = new Instructor(instructorId, "Prof. John Doe", new InstructorRole(2, "Assistant"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(existingInstructor);

        mockRepo.UpdateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>())
            .Returns(updatedInstructor);

        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new UpdateInstructorInput(instructorId, "Prof. John Doe");

        // Act
        var result = await service.UpdateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Prof. John Doe", result.Result.Name);
        Assert.Equal("Instructor updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateInstructorAsync(
            Arg.Is<Instructor>(i => i.Id == instructorId && i.Name == "Prof. John Doe"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.UpdateInstructorAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Instructor cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_BadRequest_When_InstructorId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new UpdateInstructorInput(Guid.Empty, "Dr. John Doe");

        // Act
        var result = await service.UpdateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty", result.Message);

        await mockRepo.DidNotReceive().GetInstructorByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_BadRequest_When_Name_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        mockRepo.GetInstructorByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Instructor(Guid.NewGuid(), "Existing", new InstructorRole(1, "Lead")));
        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new UpdateInstructorInput(Guid.NewGuid(), "");

        // Act
        var result = await service.UpdateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_BadRequest_When_Name_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        mockRepo.GetInstructorByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Instructor(Guid.NewGuid(), "Existing", new InstructorRole(1, "Lead")));
        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new UpdateInstructorInput(Guid.NewGuid(), "   ");

        // Act
        var result = await service.UpdateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_NotFound_When_Instructor_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns((Instructor)null!);

        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new UpdateInstructorInput(instructorId, "Dr. John Doe");

        // Act
        var result = await service.UpdateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Instructor with ID '{instructorId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateInstructorAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var existingInstructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(existingInstructor);

        mockRepo.UpdateInstructorAsync(Arg.Any<Instructor>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Instructor?>(new Exception("Database error")));

        var service = new InstructorService(mockRepo, CreateRoleRepo());
        var input = new UpdateInstructorInput(instructorId, "Prof. John Doe");

        // Act
        var result = await service.UpdateInstructorAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the instructor", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteInstructorAsync Tests

    [Fact]
    public async Task DeleteInstructorAsync_Should_Return_Success_When_Instructor_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var existingInstructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(existingInstructor);

        mockRepo.HasCourseEventsAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteInstructorAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.DeleteInstructorAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        Assert.Equal("Instructor deleted successfully.", result.Message);

        await mockRepo.Received(1).DeleteInstructorAsync(instructorId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteInstructorAsync_Should_Return_BadRequest_When_InstructorId_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.DeleteInstructorAsync(Guid.Empty, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Instructor ID cannot be empty.", result.Message);

        await mockRepo.DidNotReceive().DeleteInstructorAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteInstructorAsync_Should_Return_NotFound_When_Instructor_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns((Instructor)null!);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.DeleteInstructorAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Instructor with ID '{instructorId}' not found", result.Message);

        await mockRepo.DidNotReceive().DeleteInstructorAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteInstructorAsync_Should_Return_Conflict_When_Instructor_Has_CourseEvents()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var existingInstructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(existingInstructor);

        mockRepo.HasCourseEventsAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.DeleteInstructorAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("Cannot delete instructor", result.Message);
        Assert.Contains("assigned to course events", result.Message);

        await mockRepo.DidNotReceive().DeleteInstructorAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteInstructorAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var existingInstructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(existingInstructor);

        mockRepo.HasCourseEventsAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteInstructorAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.DeleteInstructorAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the instructor", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Fact]
    public async Task DeleteInstructorAsync_Should_Return_InternalServerError_When_Delete_Returns_False()
    {
        // Arrange
        var mockRepo = Substitute.For<IInstructorRepository>();
        var instructorId = Guid.NewGuid();
        var existingInstructor = new Instructor(instructorId, "Dr. John Doe", new InstructorRole(1, "Lead"));

        mockRepo.GetInstructorByIdAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(existingInstructor);

        mockRepo.HasCourseEventsAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteInstructorAsync(instructorId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = new InstructorService(mockRepo, CreateRoleRepo());

        // Act
        var result = await service.DeleteInstructorAsync(instructorId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Failed to delete instructor.", result.Message);
    }

    #endregion
}





