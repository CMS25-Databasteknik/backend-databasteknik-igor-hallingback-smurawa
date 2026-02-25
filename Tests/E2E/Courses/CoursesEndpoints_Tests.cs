using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Microsoft.Extensions.DependencyInjection;
using Tests.Integration.Infrastructure;

namespace Tests.E2E.Courses;

public sealed class CoursesEndpoints_Tests(CoursesDbOnelineApiFactory factory) : IClassFixture<CoursesDbOnelineApiFactory>
{
    private readonly CoursesDbOnelineApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetCourses_ReturnsOk_WithEmptyList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/courses");
        var payload = await response.Content.ReadFromJsonAsync<CourseListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.Empty(payload.Result);
    }

    [Fact]
    public async Task GetCourseById_ReturnsBadRequest_ForEmptyGuid()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/courses/{Guid.Empty}");
        var payload = await response.Content.ReadFromJsonAsync<CourseWithEventsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task GetCourseById_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/courses/{Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<CourseWithEventsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(404, payload.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_ReturnsBadRequest_ForWhitespaceTitle()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var input = new CreateCourseInput("   ", "Valid description", 5);
        var response = await client.PostAsJsonAsync("/api/courses", input);
        var payload = await response.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_ReturnsBadRequest_ForInvalidDuration()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var input = new CreateCourseInput("Valid title", "Valid description", 0);
        var response = await client.PostAsJsonAsync("/api/courses", input);
        var payload = await response.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task UpdateCourse_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var missingCourseId = Guid.NewGuid();
        var input = new UpdateCourseInput(missingCourseId, "Updated title", "Updated description", 2);
        var response = await client.PutAsJsonAsync($"/api/courses/{missingCourseId}", input);
        var payload = await response.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(404, payload.StatusCode);
    }

    [Fact]
    public async Task UpdateCourse_ReturnsBadRequest_ForInvalidDuration()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseInput("Initial title", "Initial description", 3);
        var createResponse = await client.PostAsJsonAsync("/api/courses", createInput);
        var createdPayload = await createResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);
        Assert.NotNull(createdPayload?.Result);

        var courseId = createdPayload.Result.Id;
        var updateInput = new UpdateCourseInput(courseId, "Updated title", "Updated description", 0);
        var updateResponse = await client.PutAsJsonAsync($"/api/courses/{courseId}", updateInput);
        var updatePayload = await updateResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
        Assert.NotNull(updatePayload);
        Assert.False(updatePayload.Success);
        Assert.Equal(400, updatePayload.StatusCode);
    }

    [Fact]
    public async Task DeleteCourse_ReturnsBadRequest_ForEmptyGuid()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/api/courses/{Guid.Empty}");
        var payload = await response.Content.ReadFromJsonAsync<CourseDeleteResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task DeleteCourse_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync($"/api/courses/{Guid.NewGuid()}");
        var payload = await response.Content.ReadFromJsonAsync<CourseDeleteResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(404, payload.StatusCode);
    }

    [Fact]
    public async Task DeleteCourse_ReturnsConflict_WhenCourseHasAssociatedEvents()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid courseId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var course = await RepositoryTestDataHelper.CreateCourseAsync(db);
            courseId = course.Id;
            _ = await RepositoryTestDataHelper.CreateCourseEventAsync(db, courseId: courseId);
        }

        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/courses/{courseId}");
        var payload = await response.Content.ReadFromJsonAsync<CourseDeleteResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(409, payload.StatusCode);
    }
}
