# Copilot Instructions

## Git

Never perform any `git commit` operations. The user commits manually.

## Build & Run

```bash
dotnet restore
dotnet build
dotnet run --project Presentation
```

API base: `https://localhost:7118` / `http://localhost:5400`  
OpenAPI: `https://localhost:7118/openapi/v1.json`

Apply migrations:
```bash
dotnet ef database update --project Infrastructure --startup-project Presentation
```

## Tests

```bash
# All tests
dotnet test

# By type
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.Unit"
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.Integration"
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.E2E"

# Single test class
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Course_Tests"

# Single test method (fully qualified)
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName=Backend.Tests.Unit.Domain.Modules.Courses.Models.Course_Tests.Constructor_Should_Create_Course_When_All_Parameters_Are_Valid"
```

- **Unit**: NSubstitute mocks, no DB
- **Integration**: SQLite in-memory via `SqliteInMemoryFixture` (sets `DB_PROVIDER=Sqlite`)
- **E2E**: Full HTTP via `WebApplicationFactory` + SQLite in-memory, data reseeded between tests

## Architecture

Clean Architecture, service-based (no CQRS). Dependency flow:

```
Presentation → Application → Domain
Infrastructure → Application → Domain
```

| Layer | Responsibility |
|-------|---------------|
| **Domain** | Business models, validation, `IRepositoryBase<TModel, TId>` |
| **Application** | Services, DTOs (Input/Output), caching (lookup entities only) |
| **Infrastructure** | EF Core `DbContext`, repositories, entity configs, migrations |
| **Presentation** | Minimal API endpoints, request models, HTTP result mapping |

## Key Conventions

### Naming

| Artifact | Pattern | Example |
|----------|---------|---------|
| Service interface | `I{Entity}Service` | `ICourseService` |
| Service | `{Entity}Service` | `CourseService` |
| Repository interface | `I{Entity}Repository` | `ICourseRepository` |
| Repository | `{Entity}Repository` | `CourseRepository` |
| EF entity | `{Entity}Entity` | `CourseEntity` |
| Domain model | `{Entity}` | `Course` |
| EF config | `{Entity}EntityConfiguration` | `CourseEntityConfiguration` |
| Create DTO | `Create{Entity}Input` | `CreateCourseInput` |
| Update DTO | `Update{Entity}Input` | `UpdateCourseInput` |
| Output DTO | `{Entity}Result`, `{Entity}ListResult` | `CourseResult` |
| API request | `Create{Entity}Request`, `Update{Entity}Request` | `CreateCourseRequest` |
| Cache interface | `I{Entity}Cache` | `ICourseEventTypeCache` |
| Cache class | `{Entity}Cache` | `CourseEventTypeCache` |
| Test class | `{Subject}_Tests` | `Course_Tests`, `CourseRepository_Tests` |

### Domain Models

All domain models use a `constructor + Update(...) + SetValues(...)` pattern:
- Constructor and `Update()` both delegate validation to a private `SetValues()`
- `Reconstitute(...)` is a static factory used by repositories to hydrate a model from the DB — bypasses constructor validation side effects
- Service update flows: load existing → `existing.Update(...)` → `repository.UpdateAsync(...)`
- Validation throws `ArgumentException` / `ArgumentOutOfRangeException.ThrowIfNegativeOrZero` for invalid input
- `ArgumentNullException.ThrowIfNull(x)` for null reference guards

### Result Pattern

All service methods return `ResultBase` or `ResultBase<T>` (record types):
```csharp
ResultBase.Ok()
ResultBase.NotFound("message")
ResultBase.BadRequest("message")
ResultBase.Conflict("message")
ResultBase.Unprocessable("message")
// Generic variant:
ResultBase<T>.Ok(value)
```
Endpoints call `.ToHttpResult()` on the result to map to HTTP status codes.

### Repository Pattern

`RepositoryBase<TModel, TId, TEntity, TDbContext>` provides default CRUD. Each repository implements:
- `ToEntity(model)` — domain model → EF entity
- `ToModel(entity)` — EF entity → domain model (loads lookup entities via `Include(...)`, fails fast if missing)

Transactions are used explicitly only in repositories with atomic multi-step operations (`CourseRegistrationRepository`, `CourseEventRepository`, `ParticipantRepository`).

Raw SQL (`Database.SqlQuery<T>` / `Database.ExecuteSqlAsync`) is used only in those same repositories for seat-check calculations and relation-table cleanup.

### Caching

Caching lives in the **Application layer** (not repositories). Only used for lookup/reference entities (e.g., `CourseEventType`, `PaymentMethod`, `VenueType`). Cache classes extend `CacheEntityBase` and implement `ICacheEntityBase<TEntity, TId>`. Cache is invalidated on writes.

### Folder Structure per Module

```
Domain/Modules/{Module}/Models/{Entity}.cs
Domain/Modules/{Module}/Contracts/I{Entity}Repository.cs

Application/Modules/{Module}/I{Entity}Service.cs
Application/Modules/{Module}/{Entity}Service.cs
Application/Modules/{Module}/Inputs/
Application/Modules/{Module}/Outputs/
Application/Modules/{Module}/Caching/   ← lookup entities only

Infrastructure/Persistence/EFC/Repositories/{Entity}Repository.cs
Infrastructure/Persistence/EFC/Configurations/{Entity}EntityConfiguration.cs
Infrastructure/Persistence/Entities/{Entity}Entity.cs

Presentation/Endpoints/{Entity}Endpoints.cs
Presentation/Models/{Module}/Create{Entity}Request.cs

Tests/Unit/Domain/Modules/{Module}/Models/{Entity}_Tests.cs
Tests/Unit/Application/Modules/{Module}/{Entity}Service_Tests.cs
Tests/Integration/Infrastructure/{Entity}Repository_Tests.cs
Tests/E2E/{EntityPlural}/{Entity}Endpoints_Tests.cs
```

### DI Registration

- **Application**: `builder.Services.AddApplication(...)` — all services and caches as `Scoped`
- **Infrastructure**: `builder.Services.AddInfrastructureServices(...)` — DbContext + all repositories as `Scoped`
- Infrastructure detects `DB_PROVIDER=Sqlite` env var and uses SQLite instead of SQL Server (for tests)

## Database Strategy

### Provider switching
`ContextRegistrationExtension` checks `env.IsDevelopment()`:
- **Development** → SQLite in-memory via a singleton `SqliteConnection("DataSource=:memory:;Cache=Shared")` kept open to preserve the in-memory DB for the lifetime of the app.
- **Production** → SQL Server from connection string `"CoursesOnlineDatabase"`.

Tests use `WebApplicationFactory` with `builder.UseEnvironment("Development")` to get SQLite automatically.

### Schema initialization
`PersistenceDatabaseInitializer.InitializeAsync` is called from `Program.cs` after `app.Build()`:
```csharp
if (env.IsDevelopment())
    await context.Database.EnsureCreatedAsync(ct);  // SQLite: build schema from EF model
else
    await context.Database.MigrateAsync(ct);         // SQL Server: apply migrations

await DatabaseSeeder.SeedAsync(context, ct);         // runs in both environments
```

### Seeding
`DatabaseSeeder` (in `Infrastructure/Persistence/`) is a static class with a single `SeedAsync` entry point:
- Idempotent: guards with `AnyAsync()` on a sentinel table (e.g. `VenueTypes`) before inserting anything.
- Seeds in dependency order: fixed-ID lookups → auto-ID lookups → transactional data.
- Uses EF navigation properties for junction tables (no raw SQL).
- Never called from test factories — E2E tests call `EnsureDeletedAsync` + `EnsureCreatedAsync` + their own raw SQL seed directly, bypassing the initializer entirely.

### EF entity configuration
Entity configurations take a `bool isSqlite` constructor parameter to handle the concurrency token difference:
```csharp
if (isSqlite)
    e.Property(x => x.Concurrency).IsConcurrencyToken().IsRequired(false);
else
    e.Property(x => x.Concurrency).IsRowVersion().IsConcurrencyToken().IsRequired();
```

### Migrations
Migrations are SQL Server-specific and live in `Infrastructure/Persistence/EFC/Migrations/`. They are **never run in dev or tests** — only in prod via `MigrateAsync`. Do not put seed data in migrations; use `DatabaseSeeder` instead.

Generate a new migration with:
```bash
dotnet ef migrations add <Name> --project Infrastructure --startup-project Presentation --output-dir Persistence/EFC/Migrations
```
The `--output-dir` flag is required; without it EF places files in the wrong folder.

### Lookup entity ID strategy
- `ValueGeneratedNever()` (explicit IDs required, used in seeder): `VenueType`, `PaymentMethod`, `ParticipantContactType`, `CourseRegistrationStatus`
- `ValueGeneratedOnAdd()` (DB generates IDs): `CourseEventType`, `InstructorRole`, `Location`, `InPlaceLocation`, and all transactional entities

## Project Settings

- Target framework: **.NET 10.0** across all projects
- Nullable reference types: **enabled**
- Implicit usings: **enabled**
