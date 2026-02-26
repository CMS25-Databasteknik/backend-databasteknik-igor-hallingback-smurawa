using Backend.Application.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.InPlaceLocations.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
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
        var payload = await response.Content.ReadFromJsonAsync<InPlaceLocationResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(ResultError.Conflict, payload.Error);
    }
}

