using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.CourseRegistrations.Outputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Tests.Common.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Backend.Tests.Integration.Infrastructure;

namespace Backend.Tests.E2E.CourseRegistrations;

public sealed class CourseRegistrationsEndpoints_Tests(CoursesDbOnelineApiFactory factory) : IClassFixture<CoursesDbOnelineApiFactory>
{
    private readonly CoursesDbOnelineApiFactory _factory = factory;
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
        var createInput = new CreateCourseRegistrationInput(
            participantId,
            courseEventId,
            CourseRegistrationStatus.Paid,
            new PaymentMethod(1, "Card"));

        var createResponse = await client.PostAsJsonAsync("/api/course-registrations", ToApiPayload(createInput));

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

        var createRequest = new
        {
            participantId = Guid.NewGuid(),
            courseEventId = Guid.NewGuid(),
            statusId = -1,
            paymentMethod = new { id = 1, name = "Card" }
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
        var createInput = new CreateCourseRegistrationInput(
            Guid.NewGuid(),
            courseEventId,
            CourseRegistrationStatus.Paid,
            new PaymentMethod(1, "Card"));

        var response = await client.PostAsJsonAsync("/api/course-registrations", ToApiPayload(createInput));
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

    [Fact]
    public async Task CreateCourseRegistration_ReturnsHelpfulError_ForMalformedJson()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var malformedJson = """
            { "participantId": "not-a-guid", "courseEventId":
            """;

        using var content = new StringContent(malformedJson, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-registrations", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, "json", "participantId", "courseEventId");
    }

    [Theory]
    [InlineData("""{}""", "participantId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222" }""", "paymentMethod")]
    [InlineData("""{ "participantId": "not-a-guid", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": 1, "paymentMethod": { "id": 1, "name": "Card" } }""", "participantId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": "paid", "paymentMethod": { "id": 1, "name": "Card" } }""", "statusId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": 1, "paymentMethod": null }""", "paymentMethod")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": 1, "paymentMethod": { "id": "one", "name": "Card" } }""", "paymentMethod")]
    public async Task CreateCourseRegistration_ReturnsHelpfulError_ForMissingPartialOrInvalidFields(string jsonBody, string expectedFieldFragment)
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-registrations", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, expectedFieldFragment);
    }

    [Fact]
    public async Task CreateCourseRegistration_ReturnsHelpfulError_ForPartialPayload()
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
        var partialJson = $$"""
            {
              "participantId": "{{participantId}}",
              "courseEventId": "{{courseEventId}}"
            }
            """;

        using var content = new StringContent(partialJson, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-registrations", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, "statusId", "paymentMethod");
    }

    [Fact]
    public async Task CreateCourseRegistration_ReturnsHelpfulError_ForEmptyBody()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/course-registrations", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, "body", "json");
    }

    [Theory]
    [InlineData("""{}""", "participantId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222" }""", "paymentMethod")]
    [InlineData("""{ "participantId": "not-a-guid", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": 1, "paymentMethod": { "id": 1, "name": "Card" } }""", "participantId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "not-a-guid", "statusId": 1, "paymentMethod": { "id": 1, "name": "Card" } }""", "courseEventId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": "paid", "paymentMethod": { "id": 1, "name": "Card" } }""", "statusId")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": 1, "paymentMethod": null }""", "paymentMethod")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", "courseEventId": "22222222-2222-2222-2222-222222222222", "statusId": 1, "paymentMethod": { "id": "one", "name": "Card" } }""", "paymentMethod")]
    [InlineData("""{ "participantId": "11111111-1111-1111-1111-111111111111", """, "json")]
    public async Task UpdateCourseRegistration_ReturnsHelpfulError_ForMalformedOrPartialPayload(string jsonBody, string expectedFieldFragment)
    {
        await _factory.ResetAndSeedDataAsync();

        Guid participantId;
        Guid courseEventId;
        Guid registrationId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            participantId = (await RepositoryTestDataHelper.CreateParticipantAsync(db)).Id;
            courseEventId = (await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10)).Id;
            registrationId = (await RepositoryTestDataHelper.CreateCourseRegistrationAsync(db, participantId: participantId, courseEventId: courseEventId)).Id;
        }

        using var client = _factory.CreateClient();
        var jsonToSend = jsonBody.Replace("11111111-1111-1111-1111-111111111111", participantId.ToString(), StringComparison.OrdinalIgnoreCase)
                                 .Replace("22222222-2222-2222-2222-222222222222", courseEventId.ToString(), StringComparison.OrdinalIgnoreCase);

        using var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/api/course-registrations/{registrationId}", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, expectedFieldFragment);
    }

    [Fact]
    public async Task UpdateCourseRegistration_ReturnsHelpfulError_ForEmptyBody()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid registrationId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var participantId = (await RepositoryTestDataHelper.CreateParticipantAsync(db)).Id;
            var courseEventId = (await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10)).Id;
            registrationId = (await RepositoryTestDataHelper.CreateCourseRegistrationAsync(db, participantId: participantId, courseEventId: courseEventId)).Id;
        }

        using var client = _factory.CreateClient();
        using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/api/course-registrations/{registrationId}", content);

        await ClientErrorAssertions.MentionsFieldAsync(response, "body", "json");
    }

    private static object ToApiPayload(CreateCourseRegistrationInput input) => new
    {
        participantId = input.ParticipantId,
        courseEventId = input.CourseEventId,
        statusId = input.Status.Id,
        paymentMethod = new
        {
            id = input.PaymentMethod.Id,
            name = input.PaymentMethod.Name
        }
    };

}

