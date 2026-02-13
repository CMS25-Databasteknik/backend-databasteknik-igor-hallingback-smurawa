using Backend.Application.Extensions;
using Backend.Application.Modules.Courses;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Infrastructure.Extensions;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Persistence.EFC.Seeders;
using Backend.Presentation.API.Models.Course;
using Microsoft.EntityFrameworkCore;

namespace Backend.Presentation.API;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
        builder.Services.AddApplication(builder.Configuration, builder.Environment);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            var useInMemoryDatabase = app.Configuration.GetValue<bool>("DatabaseOptions:UseInMemory");

            if (!useInMemoryDatabase)
            {

                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
                await db.Database.EnsureCreatedAsync();
                await SeedData.SeedDataAsync(db);
            }
        }
        if (!app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            await db.Database.MigrateAsync();
        }

        app.MapOpenApi();
        app.UseHttpsRedirection();
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.MapGet("/api/courses", async (ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.GetAllCoursesAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllCourses");

        app.MapGet("/api/courses/{id:guid}", async (Guid id, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.GetCourseByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseById");

        app.MapPost("/api/courses", async (CreateCourseRequest request, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var input = new CreateCourseInput(request.Title, request.Description, request.DurationInDays);
            var response = await courseService.CreateCourseAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/courses/{response.Result?.Id}", response);
        }).WithName("CreateCourse");

        app.MapPut("/api/courses/{id:guid}", async (Guid id, UpdateCourseRequest request, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseInput(id, request.Title, request.Description, request.DurationInDays);
            var response = await courseService.UpdateCourseAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateCourse");

        app.MapDelete("/api/courses/{id:guid}", async (Guid id, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.DeleteCourseAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteCourse");

        app.Run();
    }
}
