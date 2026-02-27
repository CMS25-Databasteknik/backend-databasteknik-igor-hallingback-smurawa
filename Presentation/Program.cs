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
                    StatusCodes.Status400BadRequest => new ResultBase
                    {
                        Success = false,
                        ErrorType = ErrorTypes.Validation,
                        Message = "Malformed JSON payload."
                    },
                    StatusCodes.Status404NotFound => new ResultBase
                    {
                        Success = false,
                        ErrorType = ErrorTypes.NotFound,
                        Message = "Resource not found."
                    },
                    StatusCodes.Status409Conflict => new ResultBase
                    {
                        Success = false,
                        ErrorType = ErrorTypes.Conflict,
                        Message = "Conflict."
                    },
                    StatusCodes.Status422UnprocessableEntity => new ResultBase
                    {
                        Success = false,
                        ErrorType = ErrorTypes.Unprocessable,
                        Message = "Unprocessable entity."
                    },
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
