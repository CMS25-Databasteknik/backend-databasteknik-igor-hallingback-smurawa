using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Application.Modules.Instructors.Outputs;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Models;
using Backend.Presentation.API.Models.Instructor;
using Backend.Tests.Integration.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Tests.E2E.Instructors;

public sealed class InstructorsEndpoints_Tests(CoursesOnlineDbApiFactory factory) : IClassFixture<CoursesOnlineDbApiFactory>
{
    private readonly CoursesOnlineDbApiFactory _factory = factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetAllInstructors_ReturnsOk_WithEmptyList_AfterReset()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/instructors");
        var payload = await response.Content.ReadFromJsonAsync<InstructorListResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.True(payload.Success);
        Assert.NotNull(payload.Result);
        Assert.Empty(payload.Result);
    }

    [Fact]
    public async Task CreateInstructor_ThenGetById_ReturnsCreatedInstructor()
    {
        await _factory.ResetAndSeedDataAsync();

        int instructorRoleId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            instructorRoleId = (await RepositoryTestDataHelper.CreateInstructorRoleAsync(db)).Id;
        }

        using var client = _factory.CreateClient();
        var createRequest = new CreateInstructorRequest
        {
            Name = "Donald Knuth",
            InstructorRoleId = instructorRoleId
        };

        var createResponse = await client.PostAsJsonAsync("/api/instructors", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdInstructorId = Guid.Parse(createResponse.Headers.Location!.OriginalString.Split('/')[^1]);
        var getResponse = await client.GetAsync($"/api/instructors/{createdInstructorId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<InstructorDetailsResult>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.True(getPayload.Success);
        Assert.NotNull(getPayload.Result);
        Assert.Equal(createdInstructorId, getPayload.Result.Id);
        Assert.Equal("Donald Knuth", getPayload.Result.Name);
        Assert.Equal(instructorRoleId, getPayload.Result.InstructorRole.Id);
    }

    [Fact]
    public async Task CreateInstructor_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var createRequest = new CreateInstructorRequest
        {
            Name = "Invalid Role Instructor",
            InstructorRoleId = int.MaxValue
        };

        var response = await client.PostAsJsonAsync("/api/instructors", createRequest);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("not_found", payload.Code);
    }

    [Fact]
    public async Task GetInstructorById_ReturnsBadRequest_ForEmptyGuid()
    {
        await _factory.ResetAndSeedDataAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/instructors/{Guid.Empty}");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("validation_error", payload.Code);
    }

    [Fact]
    public async Task DeleteInstructor_ReturnsConflict_WhenInstructorHasCourseEvents()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid instructorId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            var instructor = await RepositoryTestDataHelper.CreateInstructorAsync(db);
            var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(db, seats: 10);
            await RepositoryTestDataHelper.LinkInstructorToCourseEventAsync(db, instructor.Id, courseEvent.Id);
            instructorId = instructor.Id;
        }

        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/instructors/{instructorId}");
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(payload);
        Assert.False(payload.Success);
        Assert.Equal("conflict", payload.Code);
        Assert.False(payload.Result);
    }

    [Fact]
    public async Task DeleteInstructor_ReturnsOk_AndRemovesInstructor()
    {
        await _factory.ResetAndSeedDataAsync();

        Guid instructorId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            instructorId = (await RepositoryTestDataHelper.CreateInstructorAsync(db)).Id;
        }

        using var client = _factory.CreateClient();

        var deleteResponse = await client.DeleteAsync($"/api/instructors/{instructorId}");
        var deletePayload = await deleteResponse.Content.ReadFromJsonAsync<ApiResponse<bool>>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletePayload);
        Assert.True(deletePayload.Success);
        Assert.True(deletePayload.Result);

        var getResponse = await client.GetAsync($"/api/instructors/{instructorId}");
        var getPayload = await getResponse.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.NotNull(getPayload);
        Assert.False(getPayload.Success);
        Assert.Equal("not_found", getPayload.Code);
    }
}
