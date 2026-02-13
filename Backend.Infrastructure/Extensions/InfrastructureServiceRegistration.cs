using Backend.Application.Modules.Courses;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {

        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(env);

        services.AddDbContext<CoursesOnlineDbContext>(options =>
        {

            if (env.IsDevelopment())
            {

                var useInMemoryDb = config.GetValue<bool>("DatabaseOptions:UseInMemory");

                if (useInMemoryDb)
                {
                    var devDbbConfig = config.GetConnectionString("DevelopmentDatabase") ?? throw new InvalidOperationException("Connection string 'CoursesOnlineDatabase' not found.");

                    options.UseInMemoryDatabase(devDbbConfig);
                }

                else
                {
                    services.AddSingleton<SqliteConnection>(_ =>
                    {
                        var conn = new SqliteConnection("DataSource=:memory:;Cache=Shared");
                        conn.Open();
                        return conn;
                    });

                    services.AddDbContext<CoursesOnlineDbContext>((serviceProvider, options) =>
                    {
                        var conn = serviceProvider.GetRequiredService<SqliteConnection>();
                        options.UseSqlite(conn);
                    });
                }
            }
            else
            {
                var prodDbConfig = config.GetConnectionString("CoursesOnlineDatabase") ?? throw new InvalidOperationException("Connection string 'CoursesOnlineDatabase' not found.");

                options.UseSqlServer(prodDbConfig);
            }
        });

        services.AddScoped<ICoursesRepository, CourseRepository>();
        services.AddScoped<ICourseEventsRepository, CourseEventRepository>();
        services.AddScoped<ICourseService, CourseService>();
    }
}
