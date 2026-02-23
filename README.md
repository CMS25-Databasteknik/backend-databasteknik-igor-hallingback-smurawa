# Backend - Course Management System

A .NET 10 backend solution for managing courses, course events, instructors, participants, locations, and registrations.

## Architecture

The solution follows Clean Architecture and is split into these layers:

### Projects

#### **Domain** (`Domain.csproj`)

Core business models and business rules.

- **Target Framework**: .NET 10.0
- **Dependencies**: None

#### **Application** (`Application.csproj`)

Application services, use cases, and caching logic.

- **Target Framework**: .NET 10.0
- **Dependencies**:
  - Domain project
  - Microsoft.Extensions.Configuration (10.0.2)
  - Microsoft.Extensions.DependencyInjection (10.0.2)
- Microsoft.Extensions.Hosting (10.0.2)

#### **Infrastructure** (`Infrastructure.csproj`)

Database access and EF Core implementation.

- **Target Framework**: .NET 10.0
- **Dependencies**:
  - Application project
  - Microsoft.EntityFrameworkCore (10.0.2)
  - Microsoft.EntityFrameworkCore.SqlServer (10.0.2)
  - Microsoft.EntityFrameworkCore.Sqlite (10.0.2)
  - Microsoft.EntityFrameworkCore.Tools (10.0.2)
- **Main runtime DB**: SQL Server
- **Test DB**: SQLite in-memory (integration tests)

#### **Presentation** (`Presentation.csproj`)

Minimal API layer exposing HTTP endpoints.

- **Target Framework**: .NET 10.0
- **Dependencies**:
  - Application project
  - Infrastructure project
  - Microsoft.AspNetCore.OpenApi (10.0.2)
  - Microsoft.EntityFrameworkCore.Design (10.0.2)
- **Features**:
  - Minimal API endpoints
  - OpenAPI documentation
  - CORS configuration
  - HTTPS redirection

#### **Tests** (`Tests.csproj`)

Unit and integration tests.

- **Target Framework**: .NET 10.0
- **Testing Framework**: xUnit (2.9.3)
- **Dependencies**:
  - Application project
  - Infrastructure project
  - Microsoft.NET.Test.Sdk (17.14.1)
  - NSubstitute (5.3.0) - for mocking
  - coverlet.collector (6.0.4) - for code coverage

## Features

The system has CRUD support for these central modules:

- Courses
- Course events
- Course event types
- Course registrations and statuses
- Instructors and instructor roles
- Participants
- Locations and in-place locations

### CRUD by resource

- **Courses**: Create, Read (all/by id), Update, Delete
- **Course Events**: Create, Read (all/by id/by course), Update, Delete
- **Course Event Types**: Create, Read (all/by id), Update, Delete
- **Course Registrations**: Create, Read (all/by id/by participant/by event), Update, Delete
- **Course Registration Statuses**: Create, Read (all/by id), Update, Delete
- **Instructors**: Create, Read (all/by id), Update, Delete
- **Instructor Roles**: Create, Read (all/by id), Update, Delete
- **Participants**: Create, Read (all/by id), Update, Delete
- **Locations**: Create, Read (all/by id), Update, Delete
- **In-Place Locations**: Create, Read (all/by id/by location), Update, Delete

## API Endpoints

All endpoints are prefixed with `/api` and follow RESTful conventions:

- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get a specific course
- `POST /api/courses` - Create a new course
- `PUT /api/courses/{id}` - Update a course
- `DELETE /api/courses/{id}` - Delete a course

Similar patterns exist for other modules (course events, instructors, participants, etc.).

## Database

- **Runtime database**: SQL Server
- **Database name**: CoursesOnline
- **ORM**: Entity Framework Core 10.0.2
- **Connection**: Configured in `appsettings.json`
- **Migrations**: Located in `Infrastructure/Persistence/EFC/Migrations/`

### Transaction Handling

Transactions are used in repository operations that need atomic multi-step behavior.

- `Infrastructure/Persistence/EFC/Repositories/CourseRegistrationRepository.cs`
  - Uses `BeginTransactionAsync(...)` in registration create flows (`AddAsync`, `CreateRegistrationWithSeatCheckAsync`).
  - Commits on success and rolls back on failure.
- `Infrastructure/Persistence/EFC/Repositories/CourseEventRepository.cs`
  - Wraps multi-step delete operations (event + relation table cleanup) in a transaction.
- `Infrastructure/Persistence/EFC/Repositories/ParticipantRepository.cs`
  - Uses transaction for multi-step delete operations (registrations + participant).

### Raw SQL Usage

Raw SQL is used only in specific repository scenarios where direct SQL is more suitable.

- `Infrastructure/Persistence/EFC/Repositories/CourseRegistrationRepository.cs`
  - Uses `Database.SqlQuery<int>(...)` for SQL-based checks/calculations in registration flows.
- `Infrastructure/Persistence/EFC/Repositories/CourseEventRepository.cs`
  - Uses `Database.ExecuteSqlAsync(...)` for relation-table cleanup in transactional delete operations.
- `Infrastructure/Persistence/EFC/Repositories/ParticipantRepository.cs`
  - Uses `Database.ExecuteSqlAsync(...)` for registration + participant delete operations.

### Caching

Caching is implemented in the **Application layer** (not in repositories), using `IMemoryCache`.

- Base caching abstractions:
  - `Application/Common/Caching/ICacheEntityBase.cs`
  - `Application/Common/Caching/CacheEntityBase.cs`
  - `Application/Extensions/Caching/MemoryCacheExtensions.cs`
- Concrete cache implementations:
  - `Application/Modules/CourseEventTypes/Caching/CourseEventTypeCache.cs`
  - `Application/Modules/CourseRegistrationStatuses/Caching/CourseRegistrationStatusCache.cs`
- Services that currently use cache:
  - `Application/Modules/CourseEventTypes/CourseEventTypeService.cs`
  - `Application/Modules/CourseRegistrationStatuses/CourseRegistrationStatusService.cs`

The cache is primarily used for read operations (`get by id`, `get all`) and is invalidated/reset on writes.

### Connection String

```json
{
  "ConnectionStrings": {
    "CoursesOnlineDatabase": "Data Source=localhost;Initial Catalog=CoursesOnline;..."
  }
}
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- SQL Server (local or remote instance)
- Visual Studio 2022 or later / VS Code / Rider

### Setup

1. **Clone the repository**

   ```bash
   cd Backend
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   - Edit `Presentation/appsettings.json`
   - Update the `CoursesOnlineDatabase` connection string with your SQL Server details

4. **Apply database migrations**

   ```bash
   dotnet ef database update --project Infrastructure --startup-project Presentation
   ```

5. **Run the application**

   ```bash
   dotnet run --project Presentation
   ```

6. **Access the API**
   - The API will be available at `https://localhost:<port>`
   - OpenAPI documentation: `https://localhost:<port>/openapi/v1.json`

### Running Tests

Run all tests:

```bash
dotnet test
```

Run only integration tests:

```bash
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.Integration"
```

Run only unit tests:

```bash
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~Tests.Unit"
```

## Testing Setup

The project has two kinds of tests:

1. **Unit tests**

- Test small pieces of logic (usually services/domain behavior).
- Use mocks (NSubstitute) instead of a real database.
- Fast and focused.

2. **Integration tests**

- Test real repository behavior against a real EF Core database context.
- Use **SQLite in-memory** so tests are fast and isolated.
- Data is created directly in a temporary in-memory database during test execution.

### How SQLite test mode works

- Integration tests use `Tests/Integration/SqliteInMemoryFixture.cs`.
- The fixture sets `DB_PROVIDER=Sqlite`.
- Infrastructure registration detects this and uses SQLite in-memory instead of SQL Server.
- EF configurations also check this flag and use SQLite-compatible settings.

In normal app runtime, `DB_PROVIDER` is not set, so SQL Server is used.

## Technology Stack

- **.NET 10.0**
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 10.0.2** - ORM
- **SQL Server** - Database
- **xUnit** - Testing framework
- **NSubstitute** - Mocking library
- **OpenAPI** - API documentation

## Project Structure

```
Backend/
├── Domain/                 # Enterprise business logic and entities
├── Application/           # Application business logic and services
├── Infrastructure/        # Data access and external services
├── Presentation/          # Web API layer
├── Tests/                 # Unit and integration tests
└── Backend.slnx          # Solution file
```

## Development Notes

- The solution uses **nullable reference types** enabled by default
- **Implicit usings** are enabled for cleaner code
- All projects target **.NET 10.0**
- The architecture promotes **testability** and **maintainability**
- **Dependency injection** is used throughout the application
- Transaction handling is used in repositories that need atomic multi-step operations.
- Caching is implemented in the **Application** layer (services/caches), not in repositories.
- Common persistence/domain conversion logic is centralized in:
  - `Infrastructure/Common/Repositories/DomainValueConverters.cs`
