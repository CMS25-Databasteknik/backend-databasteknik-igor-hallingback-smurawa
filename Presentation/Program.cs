using Backend.Application.Extensions;
using Backend.Application.Common;
using Backend.Infrastructure.Extensions;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Presentation.API.Endpoints;
using Microsoft.AspNetCore.Routing;

namespace Backend.Presentation.API;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddCors();
        builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

        builder.Services.AddMemoryCache();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration, builder.Environment);

        var app = builder.Build();

        app.UseStatusCodePages(async statusCodeContext =>
        {
            var http = statusCodeContext.HttpContext;
            var response = http.Response;

            if (!response.HasStarted && (response.ContentLength ?? 0) == 0)
            {
                response.ContentType = "application/json";
                var payload = response.StatusCode switch
                {
                    StatusCodes.Status400BadRequest => new ResultBase(false, ErrorTypes.Validation, "Malformed JSON payload.", "Malformed JSON payload."),
                    StatusCodes.Status404NotFound => new ResultBase(false, ErrorTypes.NotFound, "Resource not found.", "Resource not found."),
                    StatusCodes.Status409Conflict => new ResultBase(false, ErrorTypes.Conflict, "Conflict.", "Conflict."),
                    StatusCodes.Status422UnprocessableEntity => new ResultBase(false, ErrorTypes.Unprocessable, "Unprocessable entity.", "Unprocessable entity."),
                    _ => null
                };

                if (payload is not null)
                    await response.WriteAsJsonAsync(payload);
            }
        });

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
