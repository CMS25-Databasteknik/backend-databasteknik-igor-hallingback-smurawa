using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.E2E;

public sealed class CoursesApi_E2E_Tests(CoursesDbOnelineApiFactory factory) : IClassFixture<CoursesDbOnelineApiFactory>
{
    private readonly CoursesDbOnelineApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetAllCourses_Should_Return_Empty_After_Reset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/courses");
        var payload = await response.Content.ReadFromJsonAsync<CourseListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.Equal(200, payload.StatusCode);
        Assert.NotNull(payload.Result);
        Assert.Empty(payload.Result);
    }

    [Fact]
    public async Task CreateCourse_Then_GetById_Should_Return_Created_Course()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseInput("E2E Course", "E2E Description", 5);
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
    public async Task UpdateCourse_Should_Persist_Changes()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseInput("Initial Course", "Initial Description", 3);
        var createResponse = await client.PostAsJsonAsync("/api/courses", createInput);
        var createPayload = await createResponse.Content.ReadFromJsonAsync<CourseResult>(_jsonOptions);
        Assert.NotNull(createPayload?.Result);

        var courseId = createPayload.Result.Id;
        var updateInput = new UpdateCourseInput(courseId, "Updated Course", "Updated Description", 10);
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
    public async Task DeleteCourse_Should_Remove_Course_From_Database()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createInput = new CreateCourseInput("Delete Me", "Delete Me Description", 2);
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
