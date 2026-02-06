using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Presentation.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<MemblerDbContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("MemblerDatabase"),
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
