using Backend.Application.Interfaces;
using Backend.Domain.Models.Course;
using Backend.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Backend.Presentation.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        //builder.Services.AddDbContext<CoursesOnlineDbContext>(options => options.UseSqlServer(
        //builder.Configuration.GetConnectionString("CoursesOnlineDatabase"),
        //    sql => sql.MigrationsAssembly("Backend.Infrastructure")
        //));

        builder.Services.AddOpenApi();
        //builder.Services.AddScoped<ICoursesRepository, CourseRepository>();
        //builder.Services.AddScoped<ICourseService, CourseService>();

        builder.Services.AddCors();

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
