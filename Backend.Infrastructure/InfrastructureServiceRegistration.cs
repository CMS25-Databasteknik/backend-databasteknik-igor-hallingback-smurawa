using Backend.Application.Interfaces;
using Backend.Application.Modules.Courses;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<CoursesOnlineDbContext>(options =>
        {
            var dbConfig = config.GetConnectionString("CoursesOnlineDatabase") ?? throw new InvalidOperationException("Connection string 'CoursesOnlineDatabase' not found.");

            options.UseSqlServer(dbConfig);
        });

        services.AddScoped<ICoursesRepository, CourseRepository>();
        services.AddScoped<ICourseService, CourseService>();
    }
}
