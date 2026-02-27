using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.InPlaceLocations.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Application.Common;
using Backend.Presentation.API.Models.InPlaceLocation;
using Backend.Tests.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.E2E.InPlaceLocations;

public sealed class InPlaceLocationsEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private readonly CoursesOnlineDbApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetAllInPlaceLocations_ReturnsOk_WithEmptyList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/in-place-locations");
        var payload = await response.Content.ReadFromJsonAsync<InPlaceLocationListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.Empty(payload.Result);
    }

    [Fact]
    public async Task GetAllInPlaceLocations_ReturnsNewestFirst_ByIdDescending()
    {
        await _factory.ResetAndSeedDataAsync();

        int locationId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            locationId = (await RepositoryTestDataHelper.CreateLocationAsync(db)).Id;
        }

        using var client = _factory.CreateClient();
        var firstCreate = await client.PostAsJsonAsync("/api/in-place-locations", new CreateInPlaceLocationRequest
        {
            LocationId = locationId,
            RoomNumber = 301,
            Seats = 20
        });
        var firstId = int.Parse(firstCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var secondCreate = await client.PostAsJsonAsync("/api/in-place-locations", new CreateInPlaceLocationRequest
        {
            LocationId = locationId,
            RoomNumber = 302,
            Seats = 30
        });
        var secondId = int.Parse(secondCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var response = await client.GetAsync("/api/in-place-locations");
        var payload = await response.Content.ReadFromJsonAsync<InPlaceLocationListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Result);
        Assert.Equal(secondId, payload.Result[0].Id);
        Assert.Equal(firstId, payload.Result[1].Id);
    }

    [Fact]
    public async Task CreateInPlaceLocation_ThenGetById_ReturnsCreatedInPlaceLocation()
    {
        await _factory.ResetAndSeedDataAsync();

        int locationId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            locationId = (await RepositoryTestDataHelper.CreateLocationAsync(db)).Id;
        }

        using var client = _factory.CreateClient();
        var createRequest = new CreateInPlaceLocationRequest
        {
            LocationId = locationId,
            RoomNumber = 101,
            Seats = 25
        };

        var createResponse = await client.PostAsJsonAsync("/api/in-place-locations", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdInPlaceLocationId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/in-place-locations/{createdInPlaceLocationId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<InPlaceLocationResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(createdInPlaceLocationId, getPayload.Result.Id);
        Assert.Equal(locationId, getPayload.Result.LocationId);
        Assert.Equal(101, getPayload.Result.RoomNumber);
        Assert.Equal(25, getPayload.Result.Seats);
    }

    [Fact]
    public async Task GetInPlaceLocationById_ReturnsBadRequest_ForInvalidId()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/in-place-locations/0");
        var payload = await response.Content.ReadFromJsonAsync<ResultBase>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(ErrorTypes.Validation, payload.ErrorType);
    }

    [Fact]
    public async Task UpdateInPlaceLocation_ReturnsConflict_WhenLocationReferenceIsInvalid()
    {
        await _factory.ResetAndSeedDataAsync();

        int inPlaceLocationId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var inPlaceLocation = await RepositoryTestDataHelper.CreateInPlaceLocationAsync(db);
            inPlaceLocationId = inPlaceLocation.Id;
        }

        using var client = _factory.CreateClient();
        var request = new UpdateInPlaceLocationRequest
        {
            LocationId = int.MaxValue,
            RoomNumber = 101,
            Seats = 25
        };

        var response = await client.PutAsJsonAsync($"/api/in-place-locations/{inPlaceLocationId}", request);
        var payload = await response.Content.ReadFromJsonAsync<ResultBase>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(ErrorTypes.Conflict, payload.ErrorType);
    }

    [Fact]
    public async Task DeleteInPlaceLocation_ReturnsOk_AndRemovesInPlaceLocation()
    {
        await _factory.ResetAndSeedDataAsync();

        int inPlaceLocationId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            inPlaceLocationId = (await RepositoryTestDataHelper.CreateInPlaceLocationAsync(db)).Id;
        }

        using var client = _factory.CreateClient();

        var deleteResponse = await client.DeleteAsync($"/api/in-place-locations/{inPlaceLocationId}");
        var deletePayload = await deleteResponse.Content.ReadFromJsonAsync<ResultBase<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletePayload);
        Assert.True(deletePayload.Success);
        Assert.True(deletePayload.Result);

        var getResponse = await client.GetAsync($"/api/in-place-locations/{inPlaceLocationId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ResultBase>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.False(getPayload.Success);
        Assert.Equal(ErrorTypes.NotFound, getPayload.ErrorType);
    }
}
