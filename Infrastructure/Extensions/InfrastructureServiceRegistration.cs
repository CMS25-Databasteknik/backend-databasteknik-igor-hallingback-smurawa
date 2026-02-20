using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrationStatuses.Contracts;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.InPlaceLocations.Contracts;
using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.Instructors.Contracts;
using Backend.Domain.Modules.Locations.Contracts;
using Backend.Domain.Modules.Participants.Contracts;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        var useSqliteForTests = string.Equals(
            Environment.GetEnvironmentVariable("DB_PROVIDER"),
            "Sqlite",
            StringComparison.OrdinalIgnoreCase);

        if (useSqliteForTests)
        {
            services.AddSingleton(_ =>
            {
                var conn = new SqliteConnection("DataSource=:memory:;Cache=Shared");
                conn.Open();
                return conn;
            });

            services.AddDbContext<CoursesOnlineDbContext>((sp, options) =>
            {
                var connection = sp.GetRequiredService<SqliteConnection>();
                options.UseSqlite(connection);
            });
        }
        else
        {
            services.AddDbContext<CoursesOnlineDbContext>(options =>
            {
                var dbConfig = config.GetConnectionString("CoursesOnlineDatabase")
                    ?? throw new InvalidOperationException("Connection string 'CoursesOnlineDatabase' not found.");

                options.UseSqlServer(dbConfig);
            });
        }

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseEventRepository, CourseEventRepository>();
        services.AddScoped<ICourseEventTypeRepository, CourseEventTypeRepository>();
        services.AddScoped<ICourseRegistrationRepository, CourseRegistrationRepository>();
        services.AddScoped<ICourseRegistrationStatusRepository, CourseRegistrationStatusRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IInPlaceLocationRepository, InPlaceLocationRepository>();
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IInstructorRoleRepository, InstructorRoleRepository>();
    }
}
