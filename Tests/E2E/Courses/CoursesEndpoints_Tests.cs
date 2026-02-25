using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Models.Course;
using Microsoft.Extensions.DependencyInjection;
using Backend.Tests.Integration.Infrastructure;

namespace Backend.Tests.E2E.Courses;

public sealed class CoursesEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private readonly CoursesOnlineDbApiFactory _factory = factory;
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

        var input = new CreateCourseRequest
        {
            Title = "   ",
            Description = "Valid description",
            DurationInDays = 5
        };
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

        var input = new CreateCourseRequest
        {
            Title = "Valid title",
            Description = "Valid description",
            DurationInDays = 0
        };
        var response = await client.PostAsJsonAsync("/api/courses", input);
        var payload = await response.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal(400, payload.StatusCode);
    }

    [Fact]
    public async Task CreateCourse_ReturnsBadRequest_ForInvalidJsonType()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var json = """
                   {
                       "title": 424324,
                       "description": "A description",
                       "durationInDays": 20
                   }
                   """;

        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/courses", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCourse_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var missingCourseId = Guid.NewGuid();
        var input = new UpdateCourseRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            DurationInDays = 2
        };
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

        var createInput = new CreateCourseRequest
        {
            Title = "Initial title",
            Description = "Initial description",
            DurationInDays = 3
        };
        var createResponse = await client.PostAsJsonAsync("/api/courses", createInput);
        var createdPayload = await createResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);
        Assert.NotNull(createdPayload?.Result);

        var courseId = createdPayload.Result.Id;
        var updateInput = new UpdateCourseRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            DurationInDays = 0
        };
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

    [Fact]
    public async Task CreateCourse_ThenGetById_ReturnsCreatedCourse()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseRequest
        {
            Title = "E2E Course",
            Description = "E2E Description",
            DurationInDays = 5
        };
        var createResponse = await client.PostAsJsonAsync("/api/courses", createInput);
        var createPayload = await createResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createPayload);
        Assert.True(createPayload.Success);
        Assert.NotNull(createPayload.Result);

        var courseId = createPayload.Result.Id;
        var getResponse = await client.GetAsync($"/api/courses/{courseId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<CourseWithEventsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(courseId, getPayload.Result.Course.Id);
        Assert.Equal("E2E Course", getPayload.Result.Course.Title);
        Assert.Equal("E2E Description", getPayload.Result.Course.Description);
        Assert.Equal(5, getPayload.Result.Course.DurationInDays);
    }

    [Fact]
    public async Task UpdateCourse_ReturnsOk_AndPersistsChanges()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseRequest
        {
            Title = "Initial Course",
            Description = "Initial Description",
            DurationInDays = 3
        };
        var createResponse = await client.PostAsJsonAsync("/api/courses", createInput);
        var createPayload = await createResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);
        Assert.NotNull(createPayload?.Result);

        var courseId = createPayload.Result.Id;
        var updateInput = new UpdateCourseRequest
        {
            Title = "Updated Course",
            Description = "Updated Description",
            DurationInDays = 10
        };
        var updateResponse = await client.PutAsJsonAsync($"/api/courses/{courseId}", updateInput);
        var updatePayload = await updateResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updatePayload);
        Assert.True(updatePayload.Success);
        Assert.NotNull(updatePayload.Result);
        Assert.Equal("Updated Course", updatePayload.Result.Title);
        Assert.Equal("Updated Description", updatePayload.Result.Description);
        Assert.Equal(10, updatePayload.Result.DurationInDays);
    }

    [Fact]
    public async Task DeleteCourse_ReturnsOk_AndRemovesCourseFromDatabase()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseRequest
        {
            Title = "Delete Me",
            Description = "Delete Me Description",
            DurationInDays = 2
        };
        var createResponse = await client.PostAsJsonAsync("/api/courses", createInput);
        var createPayload = await createResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);
        Assert.NotNull(createPayload?.Result);

        var courseId = createPayload.Result.Id;
        var deleteResponse = await client.DeleteAsync($"/api/courses/{courseId}");
        var deletePayload = await deleteResponse.Content.ReadFromJsonAsync<CourseDeleteResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletePayload);
        Assert.True(deletePayload.Success);
        Assert.True(deletePayload.Result);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
        var existing = await db.Courses.FindAsync(courseId);
        Assert.Null(existing);
    }
}

