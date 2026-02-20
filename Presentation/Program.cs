using Backend.Application.Extensions;
using Backend.Infrastructure.Extensions;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace Backend.Presentation.API;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        builder.Services.AddMemoryCache();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration, builder.Environment);

        var app = builder.Build();

        var isSqliteTestMode = string.Equals(
            Environment.GetEnvironmentVariable("DB_PROVIDER"),
            "Sqlite",
            StringComparison.OrdinalIgnoreCase);

        if (isSqliteTestMode)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CoursesOnlineDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        app.MapOpenApi();
        app.UseHttpsRedirection();
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
        app.MapApiEndpoints();

        app.Run();
    }
}

