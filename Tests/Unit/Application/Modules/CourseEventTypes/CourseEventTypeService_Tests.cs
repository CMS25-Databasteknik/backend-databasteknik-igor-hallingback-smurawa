using Backend.Application.Modules.CourseEventTypes;
using Backend.Application.Modules.CourseEventTypes.Caching;
using Backend.Application.Modules.CourseEventTypes.Inputs;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.CourseEventTypes;

public class CourseEventTypeService_Tests
{
    private static CourseEventTypeService CreateService(ICourseEventTypeRepository repo, out ICourseEventTypeCache cache)
    {
        cache = Substitute.For<ICourseEventTypeCache>();

        cache.GetAllAsync(Arg.Any<Func<CancellationToken, Task<IReadOnlyList<CourseEventType>>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<IReadOnlyList<CourseEventType>>>>()(ci.Arg<CancellationToken>()));

        cache.GetByIdAsync(Arg.Any<int>(), Arg.Any<Func<CancellationToken, Task<CourseEventType?>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<CourseEventType?>>>()(ci.Arg<CancellationToken>()));

        return new CourseEventTypeService(cache, repo);
    }
    #region CreateCourseEventTypeAsync Tests

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var expectedType = new CourseEventType(1, "Online");

        mockRepo.CreateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(expectedType);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput("Online");

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Online", result.Result.TypeName);
        Assert.Equal("Course event type created successfully.", result.Message);

        await mockRepo.Received(1).CreateCourseEventTypeAsync(
            Arg.Is<CourseEventType>(t => t.TypeName == "Online"),
            Arg.Any<CancellationToken>());
        mockCache.Received(1).ResetEntity(expectedType);
        mockCache.Received(1).SetEntity(expectedType);
    }

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.CreateCourseEventTypeAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event type cannot be null.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
        mockCache.DidNotReceive().ResetEntity(Arg.Any<CourseEventType>());
        mockCache.DidNotReceive().SetEntity(Arg.Any<CourseEventType>());
    }

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput("");

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput("   ");

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().CreateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.CreateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput("Online");

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData("Online")]
    [InlineData("In-Person")]
    [InlineData("Hybrid")]
    [InlineData("Virtual")]
    public async Task CreateCourseEventTypeAsync_Should_Create_Type_With_Various_Valid_Names(string typeName)
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var expectedType = new CourseEventType(1, typeName);

        mockRepo.CreateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(expectedType);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput(typeName);

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(typeName, result.Result.TypeName);
    }

    [Fact]
    public void CourseEventTypeService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        var cache = Substitute.For<ICourseEventTypeCache>();
        Assert.Throws<ArgumentNullException>(() => new CourseEventTypeService(cache, null!));
        Assert.Throws<ArgumentNullException>(() => new CourseEventTypeService(null!, Substitute.For<ICourseEventTypeRepository>()));
    }

    #endregion

    #region GetAllCourseEventTypesAsync Tests

    [Fact]
    public async Task GetAllCourseEventTypesAsync_Should_Return_All_Types_When_Types_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var types = new List<CourseEventType>
        {
            new CourseEventType(1, "Online"),
            new CourseEventType(2, "In-Person"),
            new CourseEventType(3, "Hybrid")
        };

        mockRepo.GetAllCourseEventTypesAsync(Arg.Any<CancellationToken>())
            .Returns(types);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetAllCourseEventTypesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 course event type(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllCourseEventTypesAsync(Arg.Any<CancellationToken>());
        await mockCache.Received(1).GetAllAsync(
            Arg.Any<Func<CancellationToken, Task<IReadOnlyList<CourseEventType>>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCourseEventTypesAsync_Should_Return_Empty_List_When_No_Types_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetAllCourseEventTypesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseEventType>());

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetAllCourseEventTypesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course event types found.", result.Message);
    }

    [Fact]
    public async Task GetAllCourseEventTypesAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetAllCourseEventTypesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseEventType>>(new Exception("Database connection failed")));

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetAllCourseEventTypesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving course event types", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetCourseEventTypeByIdAsync Tests

    [Fact]
    public async Task GetCourseEventTypeByIdAsync_Should_Return_Type_When_Type_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var courseEventType = new CourseEventType(typeId, "Online");

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(courseEventType);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(typeId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(typeId, result.Result.Id);
        Assert.Equal("Online", result.Result.TypeName);
        Assert.Equal("Course event type retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>());
        await mockCache.Received(1).GetByIdAsync(
            typeId,
            Arg.Any<Func<CancellationToken, Task<CourseEventType?>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByIdAsync_Should_Return_NotFound_When_Type_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((CourseEventType)null!);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course event type with ID '{typeId}' not found", result.Message);
    }

    [Fact]
    public async Task GetCourseEventTypeByIdAsync_Should_Return_BadRequest_When_TypeId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetCourseEventTypeByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByIdAsync_Should_Return_BadRequest_When_TypeId_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(-1, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetCourseEventTypeByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType?>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateCourseEventTypeAsync Tests

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");
        var updatedType = new CourseEventType(typeId, "Virtual");

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.UpdateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(updatedType);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(typeId, "Virtual");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(typeId, result.Result.Id);
        Assert.Equal("Virtual", result.Result.TypeName);
        Assert.Equal("Course event type updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateCourseEventTypeAsync(
            Arg.Is<CourseEventType>(t => t.Id == typeId && t.TypeName == "Virtual"),
            Arg.Any<CancellationToken>());
        mockCache.Received(1).ResetEntity(existingType);
        mockCache.Received(1).SetEntity(updatedType);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.UpdateCourseEventTypeAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Course event type cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(0, "Online");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("Course event type with ID '0' not found", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetCourseEventTypeByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));
        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(1, "");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetCourseEventTypeByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));
        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(1, "   ");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_NotFound_When_Type_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((CourseEventType)null!);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(typeId, "Virtual");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Course event type with ID '{typeId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
        mockCache.DidNotReceive().ResetEntity(Arg.Any<CourseEventType>());
        mockCache.DidNotReceive().SetEntity(Arg.Any<CourseEventType>());
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.UpdateCourseEventTypeAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType?>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(typeId, "Virtual");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteCourseEventTypeAsync Tests

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_Success_When_Type_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.IsInUseAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteCourseEventTypeAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        Assert.Equal("Course event type deleted successfully.", result.Message);

        await mockRepo.Received(1).DeleteCourseEventTypeAsync(typeId, Arg.Any<CancellationToken>());
        mockCache.Received(1).ResetEntity(existingType);
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_BadRequest_When_TypeId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseEventTypeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_BadRequest_When_TypeId_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(-1, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseEventTypeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_NotFound_When_Type_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((CourseEventType)null!);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Course event type with ID '{typeId}' not found", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseEventTypeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        mockCache.DidNotReceive().ResetEntity(Arg.Any<CourseEventType>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_Conflict_When_Type_Is_In_Use()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.IsInUseAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Cannot delete course event type with ID '{typeId}' because it is being used by one or more course events", result.Message);

        await mockRepo.DidNotReceive().DeleteCourseEventTypeAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetCourseEventTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.DeleteCourseEventTypeAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion
}







