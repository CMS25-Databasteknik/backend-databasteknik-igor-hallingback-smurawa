using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Application.Common;
using Backend.Presentation.API.Models.CourseRegistrationStatus;
using Backend.Tests.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.E2E.CourseRegistrationStatuses;

public sealed class CourseRegistrationStatusesEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private sealed record CourseRegistrationStatusDto(int Id, string Name);

    private readonly CoursesOnlineDbApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetCourseRegistrationStatuses_ReturnsOk_WithSeededList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/course-registration-statuses");
        var payload = await response.Content.ReadFromJsonAsync<ResultBase<IReadOnlyList<CourseRegistrationStatusDto>>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.True(payload.Result.Count >= 4);
    }

    [Fact]
    public async Task GetCourseRegistrationStatuses_ReturnsNewestFirst_ByIdDescending()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var firstCreate = await client.PostAsJsonAsync("/api/course-registration-statuses", new CreateCourseRegistrationStatusRequest
        {
            Name = $"OrderA-{Guid.NewGuid():N}"
        });
        var firstId = int.Parse(firstCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var secondCreate = await client.PostAsJsonAsync("/api/course-registration-statuses", new CreateCourseRegistrationStatusRequest
        {
            Name = $"OrderB-{Guid.NewGuid():N}"
        });
        var secondId = int.Parse(secondCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var response = await client.GetAsync("/api/course-registration-statuses");
        var payload = await response.Content.ReadFromJsonAsync<ResultBase<IReadOnlyList<CourseRegistrationStatusDto>>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload?.Result);
        Assert.Equal(secondId, payload.Result[0].Id);
        Assert.Equal(firstId, payload.Result[1].Id);
    }

    [Fact]
    public async Task CreateCourseRegistrationStatus_ThenGetById_ReturnsCreatedStatus()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createRequest = new CreateCourseRegistrationStatusRequest
        {
            Name = $"Queued-{Guid.NewGuid():N}"
        };

        var createResponse = await client.PostAsJsonAsync("/api/course-registration-statuses", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/course-registration-statuses/{createdId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ResultBase<CourseRegistrationStatusDto>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(createdId, getPayload.Result.Id);
        Assert.Equal(createRequest.Name, getPayload.Result.Name);
    }

    [Fact]
    public async Task CreateCourseRegistrationStatus_ReturnsBadRequest_ForInvalidPayload()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent("{\"name\":123}", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-registration-statuses", content);
        var payload = await response.Content.ReadFromJsonAsync<ResultBase>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
    }

    [Fact]
    public async Task GetCourseRegistrationStatusById_ReturnsBadRequest_ForInvalidId()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/course-registration-statuses/-1");
        var payload = await response.Content.ReadFromJsonAsync<ResultBase>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(ErrorTypes.Validation, payload.ErrorType);
    }

    [Fact]
    public async Task DeleteCourseRegistrationStatus_ReturnsConflict_WhenStatusIsInUse()
    {
        await _factory.ResetAndSeedDataAsync();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var participant = await RepositoryTestDataHelper.CreateParticipantAsync(db);
            var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10);
            _ = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(
                db,
                participantId: participant.Id,
                courseEventId: courseEvent.Id);
        }

        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/api/course-registration-statuses/0");
        var payload = await response.Content.ReadFromJsonAsync<ResultBase<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(ErrorTypes.Conflict, payload.ErrorType);
        Assert.False(payload.Result);
    }

    [Fact]
    public async Task DeleteCourseRegistrationStatus_ReturnsOk_AndRemovesStatus()
    {
        await _factory.ResetAndSeedDataAsync();

        int statusId;
        using (var client = _factory.CreateClient())
        {
            var createRequest = new CreateCourseRegistrationStatusRequest
            {
                Name = $"Temporary-{Guid.NewGuid():N}"
            };

            var createResponse = await client.PostAsJsonAsync("/api/course-registration-statuses", createRequest);
            statusId = int.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        }

        using var verificationClient = _factory.CreateClient();

        var deleteResponse = await verificationClient.DeleteAsync($"/api/course-registration-statuses/{statusId}");
        var deletePayload = await deleteResponse.Content.ReadFromJsonAsync<ResultBase<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletePayload);
        Assert.True(deletePayload.Success);
        Assert.True(deletePayload.Result);

        var getResponse = await verificationClient.GetAsync($"/api/course-registration-statuses/{statusId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ResultBase>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.False(getPayload.Success);
        Assert.Equal(ErrorTypes.NotFound, getPayload.ErrorType);
    }
}
