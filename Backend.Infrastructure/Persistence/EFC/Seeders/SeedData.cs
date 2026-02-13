using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Seeders;

public static class SeedData
{
    public static async Task SeedDataAsync(CoursesOnlineDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Courses.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;

        db.Courses.AddRange(
            new CourseEntity
            {
                Id = Guid.NewGuid(),
                Title = "Introduction to C#",
                Description = "Learn the basics of C# programming language.",
                DurationInDays = 20,
                Concurrency = new byte[8],
                CreatedAtUtc = now,
                ModifiedAtUtc = now
            },
            new CourseEntity
            {
                Id = Guid.NewGuid(),
                Title = "Advanced ASP.NET Core",
                Description = "Deep dive into ASP.NET Core for web development.",
                DurationInDays = 20,
                Concurrency = new byte[8],
                CreatedAtUtc = now,
                ModifiedAtUtc = now
            }
        );

        await db.SaveChangesAsync(cancellationToken);
    }
}
