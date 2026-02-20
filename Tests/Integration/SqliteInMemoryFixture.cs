using Backend.Infrastructure.Persistence.EFC.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration;

public sealed class SqliteInMemoryFixture : IAsyncLifetime
{
    private SqliteConnection? _conn;

    public DbContextOptions<CoursesOnlineDbContext> Options { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        _conn = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        await _conn.OpenAsync();

        await using (var cmd = _conn.CreateCommand())
        {
            cmd.CommandText =
                """
                CREATE TABLE IF NOT EXISTS Courses (
                    Id TEXT NOT NULL PRIMARY KEY,
                    Title TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    DurationInDays INTEGER NOT NULL,
                    Concurrency BLOB NULL,
                    CreatedAtUtc TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
                    ModifiedAtUtc TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
                );
                """;
            await cmd.ExecuteNonQueryAsync();
        }

        Options = new DbContextOptionsBuilder<CoursesOnlineDbContext>()
            .UseSqlite(_conn)
            .EnableSensitiveDataLogging()
            .Options;
    }

    public async Task DisposeAsync()
    {
        if (_conn is not null)
        {
            await _conn.DisposeAsync();
        }

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);

    }

    public CoursesOnlineDbContext CreateDbContext() => new(Options);
}

[CollectionDefinition(Name)]
public sealed class SqliteInMemoryCollection : ICollectionFixture<SqliteInMemoryFixture>
{
    public const string Name = "SqliteInMemory";
}
