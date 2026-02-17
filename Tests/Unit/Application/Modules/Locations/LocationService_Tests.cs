using Backend.Application.Modules.Locations;
using Backend.Application.Modules.Locations.Inputs;
using Backend.Domain.Modules.Locations.Contracts;
using Backend.Domain.Modules.Locations.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.Locations;

public class LocationService_Tests
{
    #region CreateLocationAsync Tests

    [Fact]
    public async Task CreateLocationAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var expectedLocation = new Location(1, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>())
            .Returns(expectedLocation);

        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("Kungsgatan 12", "111 43", "Stockholm");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Kungsgatan 12", result.Result.StreetName);
        Assert.Equal("111 43", result.Result.PostalCode);
        Assert.Equal("Stockholm", result.Result.City);
        Assert.Equal("Location created successfully.", result.Message);

        await mockRepo.Received(1).CreateLocationAsync(
            Arg.Is<Location>(l => l.StreetName == "Kungsgatan 12" && l.PostalCode == "111 43" && l.City == "Stockholm"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);

        // Act
        var result = await service.CreateLocationAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Location cannot be null.", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_StreetName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("", "111 43", "Stockholm");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_StreetName_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("   ", "111 43", "Stockholm");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_PostalCode_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("Kungsgatan 12", "", "Stockholm");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_PostalCode_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("Kungsgatan 12", "   ", "Stockholm");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_City_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("Kungsgatan 12", "111 43", "");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_BadRequest_When_City_Is_Whitespace()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("Kungsgatan 12", "111 43", "   ");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);

        await mockRepo.DidNotReceive().CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateLocationAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        mockRepo.CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Location>(new Exception("Database error")));

        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput("Kungsgatan 12", "111 43", "Stockholm");

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while creating the location", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Theory]
    [InlineData("Drottninggatan 1", "111 21", "Stockholm")]
    [InlineData("Storgatan 5", "411 38", "Göteborg")]
    [InlineData("Vasagatan 10", "211 20", "Malmö")]
    public async Task CreateLocationAsync_Should_Create_Location_With_Various_Valid_Inputs(
        string streetName, string postalCode, string city)
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var expectedLocation = new Location(1, streetName, postalCode, city);

        mockRepo.CreateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>())
            .Returns(expectedLocation);

        var service = new LocationService(mockRepo);
        var input = new CreateLocationInput(streetName, postalCode, city);

        // Act
        var result = await service.CreateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(streetName, result.Result.StreetName);
        Assert.Equal(postalCode, result.Result.PostalCode);
        Assert.Equal(city, result.Result.City);
    }

    [Fact]
    public void LocationService_Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LocationService(null!));
    }

    #endregion

    #region GetAllLocationsAsync Tests

    [Fact]
    public async Task GetAllLocationsAsync_Should_Return_All_Locations_When_Locations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locations = new List<Location>
        {
            new Location(1, "Kungsgatan 12", "111 43", "Stockholm"),
            new Location(2, "Storgatan 5", "411 38", "Göteborg"),
            new Location(3, "Vasagatan 10", "211 20", "Malmö")
        };

        mockRepo.GetAllLocationsAsync(Arg.Any<CancellationToken>())
            .Returns(locations);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetAllLocationsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
        Assert.Equal("Retrieved 3 location(s) successfully.", result.Message);

        await mockRepo.Received(1).GetAllLocationsAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllLocationsAsync_Should_Return_Empty_List_When_No_Locations_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        mockRepo.GetAllLocationsAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Location>());

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetAllLocationsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No locations found.", result.Message);
    }

    [Fact]
    public async Task GetAllLocationsAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        mockRepo.GetAllLocationsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<Location>>(new Exception("Database connection failed")));

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetAllLocationsAsync(CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("An error occurred while retrieving locations", result.Message);
        Assert.Contains("Database connection failed", result.Message);
    }

    #endregion

    #region GetLocationByIdAsync Tests

    [Fact]
    public async Task GetLocationByIdAsync_Should_Return_Location_When_Location_Exists()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var location = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(location);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetLocationByIdAsync(locationId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(locationId, result.Result.Id);
        Assert.Equal("Kungsgatan 12", result.Result.StreetName);
        Assert.Equal("Location retrieved successfully.", result.Message);

        await mockRepo.Received(1).GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetLocationByIdAsync_Should_Return_NotFound_When_Location_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns((Location)null!);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetLocationByIdAsync(locationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Location with ID '{locationId}' not found", result.Message);
    }

    [Fact]
    public async Task GetLocationByIdAsync_Should_Return_BadRequest_When_LocationId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetLocationByIdAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Location ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetLocationByIdAsync_Should_Return_BadRequest_When_LocationId_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetLocationByIdAsync(-1, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Location ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetLocationByIdAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Location?>(new Exception("Database error")));

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.GetLocationByIdAsync(locationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while retrieving the location", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region UpdateLocationAsync Tests

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_Success_When_Valid_Input()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var existingLocation = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");
        var updatedLocation = new Location(locationId, "Kungsgatan 15", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(existingLocation);

        mockRepo.UpdateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>())
            .Returns(updatedLocation);

        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(locationId, "Kungsgatan 15", "111 43", "Stockholm");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Kungsgatan 15", result.Result.StreetName);
        Assert.Equal("Location updated successfully.", result.Message);

        await mockRepo.Received(1).UpdateLocationAsync(
            Arg.Is<Location>(l => l.Id == locationId && l.StreetName == "Kungsgatan 15"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_BadRequest_When_Input_Is_Null()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);

        // Act
        var result = await service.UpdateLocationAsync(null!, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Location cannot be null.", result.Message);

        await mockRepo.DidNotReceive().UpdateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_BadRequest_When_LocationId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(0, "Kungsgatan 12", "111 43", "Stockholm");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Equal("Location ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_BadRequest_When_StreetName_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        mockRepo.GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new Location(1, "Old Street", "111 11", "City"));
        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(1, "", "111 43", "Stockholm");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_BadRequest_When_PostalCode_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        mockRepo.GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new Location(1, "Old Street", "111 11", "City"));
        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(1, "Kungsgatan 12", "", "Stockholm");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_BadRequest_When_City_Is_Empty()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        mockRepo.GetLocationByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new Location(1, "Old Street", "111 11", "City"));
        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(1, "Kungsgatan 12", "111 43", "");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("cannot be empty or whitespace", result.Message);
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_NotFound_When_Location_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns((Location)null!);

        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains($"Location with ID '{locationId}' not found", result.Message);

        await mockRepo.DidNotReceive().UpdateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateLocationAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var existingLocation = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(existingLocation);

        mockRepo.UpdateLocationAsync(Arg.Any<Location>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Location?>(new Exception("Database error")));

        var service = new LocationService(mockRepo);
        var input = new UpdateLocationInput(locationId, "Kungsgatan 15", "111 43", "Stockholm");

        // Act
        var result = await service.UpdateLocationAsync(input, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Null(result.Result);
        Assert.Contains("An error occurred while updating the location", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    #endregion

    #region DeleteLocationAsync Tests

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_Success_When_Location_Is_Deleted()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var existingLocation = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(existingLocation);

        mockRepo.HasInPlaceLocationsAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteLocationAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(locationId, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        Assert.Equal("Location deleted successfully.", result.Message);

        await mockRepo.Received(1).DeleteLocationAsync(locationId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_BadRequest_When_LocationId_Is_Zero()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Location ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().DeleteLocationAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_BadRequest_When_LocationId_Is_Negative()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(-1, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Location ID must be greater than zero.", result.Message);

        await mockRepo.DidNotReceive().DeleteLocationAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_NotFound_When_Location_Does_Not_Exist()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns((Location)null!);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(locationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains($"Location with ID '{locationId}' not found", result.Message);

        await mockRepo.DidNotReceive().DeleteLocationAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_Conflict_When_Location_Has_InPlaceLocations()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var existingLocation = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(existingLocation);

        mockRepo.HasInPlaceLocationsAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(true);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(locationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("Cannot delete location", result.Message);
        Assert.Contains("has in-place locations", result.Message);

        await mockRepo.DidNotReceive().DeleteLocationAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_InternalServerError_When_Repository_Throws_Exception()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var existingLocation = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(existingLocation);

        mockRepo.HasInPlaceLocationsAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteLocationAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<bool>(new Exception("Database error")));

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(locationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Contains("An error occurred while deleting the location", result.Message);
        Assert.Contains("Database error", result.Message);
    }

    [Fact]
    public async Task DeleteLocationAsync_Should_Return_InternalServerError_When_Delete_Returns_False()
    {
        // Arrange
        var mockRepo = Substitute.For<ILocationRepository>();
        var locationId = 1;
        var existingLocation = new Location(locationId, "Kungsgatan 12", "111 43", "Stockholm");

        mockRepo.GetLocationByIdAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(existingLocation);

        mockRepo.HasInPlaceLocationsAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(false);

        mockRepo.DeleteLocationAsync(locationId, Arg.Any<CancellationToken>())
            .Returns(false);

        var service = new LocationService(mockRepo);

        // Act
        var result = await service.DeleteLocationAsync(locationId, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.False(result.Result);
        Assert.Equal("Failed to delete location.", result.Message);
    }

    #endregion
}

