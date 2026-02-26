using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.CourseEvents.Outputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Models.CourseEvent;
using Backend.Presentation.API.Models;
using Microsoft.Extensions.DependencyInjection;
using Backend.Tests.Integration.Infrastructure;

namespace Backend.Tests.E2E.CourseEvents;

public sealed class CourseEventsEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private readonly CoursesOnlineDbApiFactory _factory = factory;
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
    public async Task GetAllCourseEvents_ReturnsNewestFirst_ByCreatedAtDescending()
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
        var firstCreate = await client.PostAsJsonAsync("/api/course-events", new CreateCourseEventRequest
        {
            CourseId = courseId,
            EventDate = DateTime.UtcNow.AddDays(10),
            Price = 100m,
            Seats = 10,
            CourseEventTypeId = courseEventTypeId,
            VenueTypeId = 1
        });
        var firstId = Guid.Parse(firstCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        var secondCreate = await client.PostAsJsonAsync("/api/course-events", new CreateCourseEventRequest
        {
            CourseId = courseId,
            EventDate = DateTime.UtcNow.AddDays(11),
            Price = 200m,
            Seats = 20,
            CourseEventTypeId = courseEventTypeId,
            VenueTypeId = 1
        });
        var secondId = Guid.Parse(secondCreate.Headers.Location!.OriginalString.Split('/')[^1]);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var firstEntity = await db.CourseEvents.FindAsync(firstId);
            var secondEntity = await db.CourseEvents.FindAsync(secondId);
            Assert.NotNull(firstEntity);
            Assert.NotNull(secondEntity);
            firstEntity!.CreatedAtUtc = DateTime.UtcNow.AddMinutes(-2);
            secondEntity!.CreatedAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        var response = await client.GetAsync("/api/course-events");
        var payload = await response.Content.ReadFromJsonAsync<JsonDocument>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        var ids = payload.RootElement
            .GetProperty("result")
            .EnumerateArray()
            .Select(item => item.GetProperty("id").GetGuid())
            .ToList();
        var firstIndex = ids.FindIndex(x => x == firstId);
        var secondIndex = ids.FindIndex(x => x == secondId);

        Assert.True(firstIndex >= 0);
        Assert.True(secondIndex >= 0);
        Assert.True(secondIndex < firstIndex);
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
        var createRequest = new CreateCourseEventRequest
        {
            CourseId = courseId,
            EventDate = DateTime.UtcNow.AddDays(7),
            Price = 499m,
            Seats = 20,
            CourseEventTypeId = courseEventTypeId,
            VenueTypeId = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/course-events", createRequest);

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

        var createRequest = new CreateCourseEventRequest
        {
            CourseId = Guid.NewGuid(),
            EventDate = DateTime.UtcNow.AddDays(2),
            Price = 99m,
            Seats = 10,
            CourseEventTypeId = 1,
            VenueTypeId = 1
        };

        var response = await client.PostAsJsonAsync("/api/course-events", createRequest);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("not_found", payload.Code);
    }

    [Fact]
    public async Task GetCourseEventById_ReturnsBadRequest_ForEmptyGuid()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/course-events/{Guid.Empty}");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("validation_error", payload.Code);
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
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("conflict", payload.Code);
        Assert.False(payload.Result);
    }
}

