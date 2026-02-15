using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Extensions;

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
        services.AddScoped<ICourseEventsRepository, CourseEventRepository>();
        services.AddScoped<ICourseEventTypesRepository, CourseEventTypeRepository>();
    }
}
