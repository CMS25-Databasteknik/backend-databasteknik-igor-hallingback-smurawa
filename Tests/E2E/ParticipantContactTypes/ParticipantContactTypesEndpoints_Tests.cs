using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Models;
using Backend.Presentation.API.Models.ParticipantContactType;
using Backend.Tests.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.E2E.ParticipantContactTypes;

public sealed class ParticipantContactTypesEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private sealed record ParticipantContactTypeDto(int Id, string Name);

    private readonly CoursesOnlineDbApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetParticipantContactTypes_ReturnsOk_WithListPayload_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/participant-contact-types");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ParticipantContactTypeDto>>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
    }

    [Fact]
    public async Task GetParticipantContactTypes_ReturnsNewestFirst_ByIdDescending()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var firstCreate = await client.PostAsJsonAsync("/api/participant-contact-types", new CreateParticipantContactTypeRequest
        {
            Name = $"OrderA-{Guid.NewGuid():N}"
        });
        var firstId = int.Parse(firstCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var secondCreate = await client.PostAsJsonAsync("/api/participant-contact-types", new CreateParticipantContactTypeRequest
        {
            Name = $"OrderB-{Guid.NewGuid():N}"
        });
        var secondId = int.Parse(secondCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var response = await client.GetAsync("/api/participant-contact-types");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ParticipantContactTypeDto>>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Result);
        Assert.Equal(secondId, payload.Result[0].Id);
        Assert.Equal(firstId, payload.Result[1].Id);
    }

    [Fact]
    public async Task CreateParticipantContactType_ThenGetById_ReturnsCreatedContactType()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createRequest = new CreateParticipantContactTypeRequest
        {
            Name = $"Work-{Guid.NewGuid():N}"
        };

        var createResponse = await client.PostAsJsonAsync("/api/participant-contact-types", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/participant-contact-types/{createdId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ApiResponse<ParticipantContactTypeDto>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload?.Result);
        Assert.True(getPayload.Success);
        Assert.Equal(createdId, getPayload.Result.Id);
        Assert.Equal(createRequest.Name, getPayload.Result.Name);
    }

    [Fact]
    public async Task CreateParticipantContactType_ReturnsBadRequest_ForInvalidPayload()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent("{\"name\":123}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/participant-contact-types", content);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("validation_error", payload.Code);
    }

    [Fact]
    public async Task GetParticipantContactTypeById_ReturnsBadRequest_ForInvalidId()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/participant-contact-types/0");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("validation_error", payload.Code);
    }

    [Fact]
    public async Task DeleteParticipantContactType_ReturnsConflict_WhenContactTypeIsInUse()
    {
        await _factory.ResetAndSeedDataAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            _ = await RepositoryTestDataHelper.CreateParticipantAsync(db);
        }

        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/api/participant-contact-types/1");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("conflict", payload.Code);
        Assert.False(payload.Result);
    }

    [Fact]
    public async Task DeleteParticipantContactType_ReturnsOk_AndRemovesContactType()
    {
        await _factory.ResetAndSeedDataAsync();

        int contactTypeId;
        using (var client = _factory.CreateClient())
        {
            var createRequest = new CreateParticipantContactTypeRequest
            {
                Name = $"TempContact-{Guid.NewGuid():N}"
            };

            var createResponse = await client.PostAsJsonAsync("/api/participant-contact-types", createRequest);
            contactTypeId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        }

        using var verificationClient = _factory.CreateClient();

        var deleteResponse = await verificationClient.DeleteAsync($"/api/participant-contact-types/{contactTypeId}");
        var deletePayload = await deleteResponse.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletePayload);
        Assert.True(deletePayload.Success);
        Assert.True(deletePayload.Result);

        var getResponse = await verificationClient.GetAsync($"/api/participant-contact-types/{contactTypeId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.False(getPayload.Success);
        Assert.Equal("not_found", getPayload.Code);
    }
}
