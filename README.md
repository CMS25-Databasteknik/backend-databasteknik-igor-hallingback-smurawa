# Backend - Course Management System

A .NET 10.0 backend solution for managing online courses, course events, instructors, participants, and registrations. Built using Clean Architecture principles.

## Architecture

The solution follows Clean Architecture with clear separation of concerns across four main layers:

### Projects

#### **Domain** (`Domain.csproj`)

The core layer containing enterprise business logic and entities.

- **Target Framework**: .NET 10.0
- **Dependencies**: None (independent of other layers)
- **Modules**:
  - Courses
  - CourseEvents
  - CourseEventTypes
  - CourseRegistrations
  - Instructors
  - Participants
  - Locations
  - InPlaceLocations

#### **Application** (`Application.csproj`)

Contains application business logic, use cases, and service interfaces.

- **Target Framework**: .NET 10.0
- **Dependencies**:
  - Domain project
  - Microsoft.Extensions.Configuration (10.0.2)
  - Microsoft.Extensions.DependencyInjection (10.0.2)
  - Microsoft.Extensions.Hosting (10.0.2)
- **Structure**: Organized by feature modules mirroring the Domain layer
- **Patterns**: Service pattern with dependency injection

#### **Infrastructure** (`Infrastructure.csproj`)

Handles data persistence and external concerns.

- **Target Framework**: .NET 10.0
- **Dependencies**:
  - Application project
  - Microsoft.EntityFrameworkCore (10.0.2)
  - Microsoft.EntityFrameworkCore.SqlServer (10.0.2)
  - Microsoft.EntityFrameworkCore.Tools (10.0.2)
- **Database**: SQL Server with Entity Framework Core
- **DbContext**: `CoursesOnlineDbContext`
- **Includes**: EF Core migrations and repository implementations

#### **Presentation** (`Presentation.csproj`)

Web API layer exposing HTTP endpoints.

- **Target Framework**: .NET 10.0
- **Type**: ASP.NET Core Web API
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

The system provides complete CRUD operations for the following modules:

- **Courses**: Manage course catalog with title, description, and duration
- **Course Events**: Schedule specific instances of courses
- **Course Event Types**: Define types of course events (e.g., online, in-person)
- **Course Registrations**: Handle participant enrollment in course events
- **Instructors**: Manage instructor information
- **Participants**: Manage participant/student information
- **Locations**: Manage general location data
- **In-Place Locations**: Manage physical venue locations for in-person events

## API Endpoints

All endpoints are prefixed with `/api` and follow RESTful conventions:

- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get a specific course
- `POST /api/courses` - Create a new course
- `PUT /api/courses/{id}` - Update a course
- `DELETE /api/courses/{id}` - Delete a course

Similar patterns exist for other modules (course events, instructors, participants, etc.).

## Database

- **Database**: SQL Server
- **Database Name**: CoursesOnline
- **ORM**: Entity Framework Core 10.0.2
- **Connection**: Configured in `appsettings.json`
- **Migrations**: Located in `Infrastructure/Persistence/EFC/Migrations/`

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

```bash
dotnet test
```

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
- The solution uses transactions handling in  courseEventRepository, courseRegistrationRepository and participantRepository to ensure data integrity during complex operations
## Architecture Benefits

- **Separation of Concerns**: Each layer has a specific responsibility
- **Testability**: Business logic is independent of infrastructure
- **Maintainability**: Changes in one layer don't affect others
- **Flexibility**: Easy to swap out implementations (e.g., database providers)
- **Scalability**: Clean boundaries make it easier to scale individual components
