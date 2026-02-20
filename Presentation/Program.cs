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

        builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
        builder.Services.AddApplication(builder.Configuration, builder.Environment);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
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
        app.MapApiEndpoints();

        app.Run();
    }
}

