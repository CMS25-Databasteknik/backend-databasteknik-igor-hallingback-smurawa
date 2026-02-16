using Backend.Application.Modules.Courses;
using Backend.Application.Modules.CourseEvents;
using Backend.Application.Modules.CourseEventTypes;
using Backend.Application.Modules.CourseRegistrations;
using Backend.Application.Modules.Participants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.Application.Extensions;

public static class ApplicationServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICourseEventService, CourseEventService>();
        services.AddScoped<ICourseEventTypeService, CourseEventTypeService>();
        services.AddScoped<ICourseRegistrationService, CourseRegistrationService>();
        services.AddScoped<IParticipantService, ParticipantService>();

        return services;
    }
}
