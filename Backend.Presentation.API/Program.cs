using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Presentation.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<CoursesOnlineDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("CoursesOnlineDatabase"),
            sql => sql.MigrationsAssembly("Backend.Infrastructure")
        ));
        builder.Services.AddCors();

        var app = builder.Build();

        app.MapOpenApi();
        app.UseHttpsRedirection();
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.Run();
    }
}
