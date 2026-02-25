using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.CourseRegistrations.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Models.CourseRegistration;
using Microsoft.Extensions.DependencyInjection;
using Backend.Tests.Integration.Infrastructure;

namespace Backend.Tests.E2E.CourseRegistrations;

public sealed class CourseRegistrationsEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private readonly CoursesOnlineDbApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetAllCourseRegistrations_ReturnsOk_WithEmptyList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/course-registrations");
        var payload = await response.Content.ReadFromJsonAsync<CourseRegistrationListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.Empty(payload.Result);
    }

    [Fact]
    public async Task CreateCourseRegistration_ThenGetById_ReturnsCreatedRegistration()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid participantId;
        Guid courseEventId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            participantId = (await RepositoryTestDataHelper.CreateParticipantAsync(db)).Id;
            courseEventId = (await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10)).Id;
        }

        using var client = _factory.CreateClient();
        var createRequest = new CreateCourseRegistrationRequest
        {
            ParticipantId = participantId,
            CourseEventId = courseEventId,
            StatusId = 1,
            PaymentMethodId = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/course-registrations", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);
        var createdRegistrationId = Guid.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/course-registrations/{createdRegistrationId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<CourseRegistrationDetailsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(createdRegistrationId, getPayload.Result.Id);
        Assert.Equal(participantId, getPayload.Result.Participant.Id);
        Assert.Equal(courseEventId, getPayload.Result.CourseEvent.Id);
    }

    [Fact]
    public async Task CreateCourseRegistration_ReturnsBadRequest_ForNegativeStatusId()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createRequest = new CreateCourseRegistrationRequest
        {
            ParticipantId = Guid.NewGuid(),
            CourseEventId = Guid.NewGuid(),
            StatusId = -1,
            PaymentMethodId = 1
        };

        var response = await client.PostAsJsonAsync("/api/course-registrations", createRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCourseRegistration_ReturnsNotFound_WhenParticipantDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid courseEventId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            courseEventId = (await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10)).Id;
        }

        using var client = _factory.CreateClient();
        var createRequest = new CreateCourseRegistrationRequest
        {
            ParticipantId = Guid.NewGuid(),
            CourseEventId = courseEventId,
            StatusId = 1,
            PaymentMethodId = 1
        };

        var response = await client.PostAsJsonAsync("/api/course-registrations", createRequest);
        var payload = await response.Content.ReadFromJsonAsync<CourseRegistrationResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(404, payload.StatusCode);
    }

    [Fact]
    public async Task GetCourseRegistrationById_ReturnsBadRequest_ForEmptyGuid()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/course-registrations/{Guid.Empty}");
        var payload = await response.Content.ReadFromJsonAsync<CourseRegistrationDetailsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task DeleteCourseRegistration_ReturnsNotFound_WhenRegistrationDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/api/course-registrations/{Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<CourseRegistrationDeleteResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(404, payload.StatusCode);
    }
}

