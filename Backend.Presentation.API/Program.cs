using Backend.Application.Extensions;
using Backend.Application.Modules.Courses;
using Backend.Domain.Models.Course;
using Backend.Infrastructure.Extensions;

namespace Backend.Presentation.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration, builder.Environment);

        var app = builder.Build();

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
                return Results.BadRequest(response.Message);

            return Results.Ok(response.Result);
        })
        .WithName("GetAllCourses");

        app.MapPost("/api/courses", async (CreateCourseDto createCourseDto, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.CreateCourseAsync(createCourseDto, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response.Message);

            return Results.Created($"/api/courses/{response.Result?.Id}", response.Result);
        })
        .WithName("CreateCourse");

        app.Run();
    }
}
