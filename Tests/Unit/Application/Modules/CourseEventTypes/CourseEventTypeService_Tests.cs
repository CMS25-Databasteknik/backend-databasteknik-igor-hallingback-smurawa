using Backend.Application.Common;
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
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<IReadOnlyList<CourseEventType>>>>()(ci.Arg<CancellationToken>())!);

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

        mockRepo.AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(expectedType);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput("Online");

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal("Online", result.Result.TypeName);
        Assert.Equal("Course event type created successfully.", result.Message);

        await mockRepo.Received(1).AddAsync(
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Course event type cannot be null.", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);

        await mockRepo.DidNotReceive().AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput("Online");

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Fact]
    public async Task CreateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Already_Exists()
    {
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetCourseEventTypeByTypeNameAsync("Online", Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));

        var service = CreateService(mockRepo, out var mockCache);

        var result = await service.CreateCourseEventTypeAsync(new CreateCourseEventTypeInput("Online"), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Equal("A typename with the same name already exists.", result.Message);
        await mockRepo.DidNotReceive().AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
        mockCache.DidNotReceive().ResetEntity(Arg.Any<CourseEventType>());
        mockCache.DidNotReceive().SetEntity(Arg.Any<CourseEventType>());
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

        mockRepo.AddAsync(Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(expectedType);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new CreateCourseEventTypeInput(typeName);

        // Act
        var result = await service.CreateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
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

        mockRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(types);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetAllCourseEventTypesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 course event type(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        await mockCache.Received(1).GetAllAsync(
            Arg.Any<Func<CancellationToken, Task<IReadOnlyList<CourseEventType>>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCourseEventTypesAsync_Should_Return_Empty_List_When_No_Types_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseEventType>());

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetAllCourseEventTypesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course event types found.", result.Message);
    }

    [Fact]
    public async Task GetAllCourseEventTypesAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseEventType>>(new Exception("Database connection failed")));

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetAllCourseEventTypesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Contains("An error occurred while retrieving course event types", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetCourseEventTypeByTypeNameAsync Tests

    [Fact]
    public async Task GetCourseEventTypeByTypeNameAsync_Should_Return_Type_When_Type_Exists()
    {
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeName = "Online";
        var courseEventType = new CourseEventType(1, typeName);

        mockRepo.GetCourseEventTypeByTypeNameAsync(typeName, Arg.Any<CancellationToken>())
            .Returns(courseEventType);

        var service = CreateService(mockRepo, out var mockCache);

        var result = await service.GetCourseEventTypeByTypeNameAsync(typeName, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(typeName, result.Result.TypeName);
        Assert.Equal("Course event type retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetCourseEventTypeByTypeNameAsync(typeName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByTypeNameAsync_Should_Return_BadRequest_When_TypeName_Is_Empty()
    {
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var service = CreateService(mockRepo, out var mockCache);

        var result = await service.GetCourseEventTypeByTypeNameAsync(" ", CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Equal("Course event type name is required.", result.Message);

        await mockRepo.DidNotReceive().GetCourseEventTypeByTypeNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByTypeNameAsync_Should_Return_NotFound_When_Type_Does_Not_Exist()
    {
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeName = "Unknown";
        mockRepo.GetCourseEventTypeByTypeNameAsync(typeName, Arg.Any<CancellationToken>())
            .Returns((CourseEventType?)null);

        var service = CreateService(mockRepo, out var mockCache);

        var result = await service.GetCourseEventTypeByTypeNameAsync(typeName, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Equal($"Course event type with name '{typeName}' not found.", result.Message);
        await mockRepo.Received(1).GetCourseEventTypeByTypeNameAsync(typeName, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByTypeNameAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetCourseEventTypeByTypeNameAsync("Online", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType?>(new Exception("Database error")));

        var service = CreateService(mockRepo, out _);

        var result = await service.GetCourseEventTypeByTypeNameAsync("Online", CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Contains("An error occurred while retrieving the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
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

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(courseEventType);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(typeId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(typeId, result.Result.Id);
        Assert.Equal("Online", result.Result.TypeName);
        Assert.Equal("Course event type retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetByIdAsync(typeId, Arg.Any<CancellationToken>());
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

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((CourseEventType)null!);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCourseEventTypeByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType?>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.GetCourseEventTypeByIdAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
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

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.UpdateAsync(Arg.Any<int>(), Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(updatedType);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(typeId, "Virtual");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(typeId, result.Result.Id);
        Assert.Equal("Virtual", result.Result.TypeName);
        Assert.Equal("Course event type updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateAsync(
            Arg.Is<int>(id => id == typeId),
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Equal("Course event type cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateAsync(Arg.Any<int>(), Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
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
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("Course event type with ID '0' not found", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));
        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(1, "");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_BadRequest_When_TypeName_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        mockRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));
        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(1, "   ");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Null(result.Result);
        Assert.Contains("Type name cannot be empty or whitespace.", result.Message);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_Should_Return_NotFound_When_Type_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((CourseEventType)null!);

        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(typeId, "Virtual");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Null(result.Result);
        Assert.Contains($"Course event type with ID '{typeId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateAsync(Arg.Any<int>(), Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>());
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

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.UpdateAsync(Arg.Any<int>(), Arg.Any<CourseEventType>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEventType?>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);
        var input = new UpdateCourseEventTypeInput(typeId, "Virtual");

        // Act
        var result = await service.UpdateCourseEventTypeAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
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

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.IsInUseAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.RemoveAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.True(result.Result);
        Assert.Equal("Course event type deleted successfully.", result.Message);

        await mockRepo.Received(1).RemoveAsync(typeId, Arg.Any<CancellationToken>());
        mockCache.Received(1).ResetEntity(existingType);
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_InternalServerError_When_Delete_Returns_False()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.IsInUseAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.RemoveAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.False(result.Result);
        Assert.Equal("Failed to delete course event type.", result.Message);
        mockCache.DidNotReceive().ResetEntity(Arg.Any<CourseEventType>());
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.False(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
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
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.False(result.Result);
        Assert.Equal("Course event type ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_NotFound_When_Type_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((CourseEventType)null!);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.False(result.Result);
        Assert.Contains($"Course event type with ID '{typeId}' not found", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        mockCache.DidNotReceive().ResetEntity(Arg.Any<CourseEventType>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_Conflict_When_Type_Is_In_Use()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.IsInUseAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Conflict, result.Error);
        Assert.False(result.Result);
        Assert.Contains($"Cannot delete course event type with ID '{typeId}' because it is being used by one or more course events", result.Message);

        await mockRepo.DidNotReceive().RemoveAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ICourseEventTypeRepository>();
        var typeId = 1;
        var existingType = new CourseEventType(typeId, "Online");

        mockRepo.GetByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(existingType);

        mockRepo.RemoveAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = CreateService(mockRepo, out var mockCache);

        // Act
        var result = await service.DeleteCourseEventTypeAsync(typeId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the course event type", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion
}

