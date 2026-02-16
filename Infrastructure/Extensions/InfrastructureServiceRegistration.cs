using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.InPlaceLocations.Contracts;
using Backend.Domain.Modules.Locations.Contracts;
using Backend.Domain.Modules.Participants.Contracts;
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

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseEventRepository, CourseEventRepository>();
        services.AddScoped<ICourseEventTypeRepository, CourseEventTypeRepository>();
        services.AddScoped<ICourseRegistrationRepository, CourseRegistrationRepository>();
        services.AddScoped<IParticipantRepository, ParticipantRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IInPlaceLocationRepository, InPlaceLocationRepository>();
    }
}
