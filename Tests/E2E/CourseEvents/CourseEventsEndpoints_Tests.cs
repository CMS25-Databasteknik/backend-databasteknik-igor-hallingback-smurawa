using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEvents.Outputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Tests.Common.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Backend.Tests.Integration.Infrastructure;

namespace Backend.Tests.E2E.CourseEvents;

public sealed class CourseEventsEndpoints_Tests(CoursesDbOnelineApiFactory factory) : IClassFixture<CoursesDbOnelineApiFactory>
{
    private readonly CoursesDbOnelineApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetAllCourseEvents_ReturnsOk_WithEmptyList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/course-events");
        var payload = await response.Content.ReadFromJsonAsync<CourseEventListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.Empty(payload.Result);
    }

    [Fact]
    public async Task CreateCourseEvent_ThenGetById_ReturnsCreatedEvent()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid courseId;
        int courseEventTypeId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            courseId = (await RepositoryTestDataHelper.CreateCourseAsync(db)).Id;
            courseEventTypeId = (await RepositoryTestDataHelper.CreateCourseEventTypeAsync(db)).Id;
        }

        using var client = _factory.CreateClient();
        var createInput = new CreateCourseEventInput(
            courseId,
            DateTime.UtcNow.AddDays(7),
            499m,
            20,
            courseEventTypeId,
            new VenueType(1, "InPerson"));

        var createResponse = await client.PostAsJsonAsync("/api/course-events", createInput);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var createdEventId = Guid.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/course-events/{createdEventId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<CourseEventDetailsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(createdEventId, getPayload.Result.Id);
        Assert.Equal(courseId, getPayload.Result.CourseId);
        Assert.Equal(courseEventTypeId, getPayload.Result.CourseEventType.Id);
        Assert.Equal(1, getPayload.Result.VenueType.Id);
    }

    [Fact]
    public async Task CreateCourseEvent_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseEventInput(
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(2),
            99m,
            10,
            1,
            new VenueType(1, "InPerson"));

        var response = await client.PostAsJsonAsync("/api/course-events", createInput);
        var payload = await response.Content.ReadFromJsonAsync<CourseEventResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(404, payload.StatusCode);
    }

    [Theory]
    [InlineData("""{ "courseId": "not-a-guid", "eventDate": "2026-03-01T00:00:00Z", "price": 99, "seats": 10, "courseEventTypeId": 1, "venueType": { "id": 1, "name": "InPerson" } }""", "courseId")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "not-a-date", "price": 99, "seats": 10, "courseEventTypeId": 1, "venueType": { "id": 1, "name": "InPerson" } }""", "eventDate")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "2026-03-01T00:00:00Z", "price": "free", "seats": 10, "courseEventTypeId": 1, "venueType": { "id": 1, "name": "InPerson" } }""", "price")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "2026-03-01T00:00:00Z", "price": 99, "seats": 10 }""", "courseEventTypeId")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "2026-03-01T00:00:00Z", "price": 99, "seats": 10, "courseEventTypeId": 1, "venueType": null }""", "venueType")]
    [InlineData("""{}""", "courseId")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", """, "json")]
    public async Task CreateCourseEvent_ReturnsHelpfulError_ForMalformedOrPartialPayload(string jsonBody, string expectedFieldFragment)
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-events", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, expectedFieldFragment);
    }

    [Fact]
    public async Task CreateCourseEvent_ReturnsHelpfulError_ForEmptyBody()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-events", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, "body", "json");
    }

    [Fact]
    public async Task GetCourseEventById_ReturnsBadRequest_ForEmptyGuid()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/course-events/{Guid.Empty}");
        var payload = await response.Content.ReadFromJsonAsync<CourseEventDetailsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task DeleteCourseEvent_ReturnsConflict_WhenEventHasRegistrations()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid eventId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var participant = await RepositoryTestDataHelper.CreateParticipantAsync(db);
            var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10);
            _ = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(
                db,
                participantId: participant.Id,
                courseEventId: courseEvent.Id,
                status: CourseRegistrationStatus.Paid);
            eventId = courseEvent.Id;
        }

        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/course-events/{eventId}");
        var payload = await response.Content.ReadFromJsonAsync<CourseEventDeleteResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(409, payload.StatusCode);
        Assert.False(payload.Result);
    }

    [Theory]
    [InlineData("""{ "courseId": "not-a-guid", "eventDate": "2026-03-01T00:00:00Z", "price": 199, "seats": 10, "courseEventTypeId": 1, "venueType": { "id": 1, "name": "InPerson" } }""", "courseId")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "not-a-date", "price": 199, "seats": 10, "courseEventTypeId": 1, "venueType": { "id": 1, "name": "InPerson" } }""", "eventDate")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "2026-03-01T00:00:00Z", "price": 199, "seats": "many", "courseEventTypeId": 1, "venueType": { "id": 1, "name": "InPerson" } }""", "seats")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "2026-03-01T00:00:00Z", "price": 199 }""", "courseEventType")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", "eventDate": "2026-03-01T00:00:00Z", "price": 199, "seats": 10, "courseEventTypeId": 1, "venueType": null }""", "venueType")]
    [InlineData("""{}""", "course")]
    [InlineData("""{ "courseId": "11111111-1111-1111-1111-111111111111", """, "json")]
    public async Task UpdateCourseEvent_ReturnsHelpfulError_ForMalformedOrPartialPayload(string jsonBody, string expectedFieldFragment)
    {
        await _factory.ResetAndSeedDataAsync();

        Guid courseId;
        Guid courseEventId;
        int courseEventTypeId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            courseId = (await RepositoryTestDataHelper.CreateCourseAsync(db)).Id;
            courseEventTypeId = (await RepositoryTestDataHelper.CreateCourseEventTypeAsync(db)).Id;
            courseEventId = (await RepositoryTestDataHelper.CreateCourseEventAsync(db, courseId: courseId)).Id;
        }

        using var client = _factory.CreateClient();
        var jsonToSend = jsonBody.Replace("11111111-1111-1111-1111-111111111111", courseId.ToString(), StringComparison.OrdinalIgnoreCase)
                                 .Replace("\"courseEventTypeId\": 1", $"\"courseEventTypeId\": {courseEventTypeId}", StringComparison.OrdinalIgnoreCase);

        using var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/api/course-events/{courseEventId}", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, expectedFieldFragment);
    }

    [Fact]
    public async Task UpdateCourseEvent_ReturnsHelpfulError_ForEmptyBody()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid courseEventId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            courseEventId = (await RepositoryTestDataHelper.CreateCourseEventAsync(db)).Id;
        }

        using var client = _factory.CreateClient();
        using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/api/course-events/{courseEventId}", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, "body", "json");
    }

}

