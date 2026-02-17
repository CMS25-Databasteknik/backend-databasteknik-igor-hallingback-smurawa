using System;
using Backend.Infrastructure.Persistence.EFC.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(CoursesOnlineDbContext))]
    [Migration("20260217133000_SeedInitialData")]
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT CourseEventTypes ON;
                INSERT INTO CourseEventTypes (Id, TypeName)
                VALUES 
                (1, 'Online'),
                (2, 'In-Person'),
                (3, 'Hybrid'),
                (4, 'Workshop'),
                (5, 'Webinar');
                SET IDENTITY_INSERT CourseEventTypes OFF;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Courses (Id, Title, Description, DurationInDays)
                VALUES 
                ('11111111-1111-1111-1111-111111111111', 'Introduction to C# Programming', 'Learn the fundamentals of C# programming language, including syntax, data types, control structures, and object-oriented programming concepts.', 30),
                ('22222222-2222-2222-2222-222222222222', 'Advanced ASP.NET Core Development', 'Master advanced ASP.NET Core concepts including middleware, dependency injection, authentication, and building RESTful APIs.', 45),
                ('33333333-3333-3333-3333-333333333333', 'Database Design with Entity Framework', 'Comprehensive course on database design principles, SQL fundamentals, and Entity Framework Core ORM.', 25),
                ('44444444-4444-4444-4444-444444444444', 'React and TypeScript Fundamentals', 'Build modern web applications using React library with TypeScript for type-safe development.', 35),
                ('55555555-5555-5555-5555-555555555555', 'Cloud Architecture with Azure', 'Learn to design and implement scalable cloud solutions using Microsoft Azure services.', 40),
                ('66666666-6666-6666-6666-666666666666', 'DevOps and CI/CD Pipelines', 'Master DevOps practices, automated testing, continuous integration and deployment using GitHub Actions and Azure DevOps.', 30),
                ('77777777-7777-7777-7777-777777777777', 'Microservices Architecture', 'Design and implement microservices-based applications with Docker, Kubernetes, and service mesh.', 50),
                ('88888888-8888-8888-8888-888888888888', 'Machine Learning with Python', 'Introduction to machine learning algorithms, data analysis, and AI model development using Python.', 60);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Instructors (Id, Name)
                VALUES 
                ('a1111111-1111-1111-1111-111111111111', 'Emma Johnson'),
                ('a2222222-2222-2222-2222-222222222222', 'Michael Chen'),
                ('a3333333-3333-3333-3333-333333333333', 'Sarah Williams'),
                ('a4444444-4444-4444-4444-444444444444', 'David Anderson'),
                ('a5555555-5555-5555-5555-555555555555', 'Lisa Martinez'),
                ('a6666666-6666-6666-6666-666666666666', 'Robert Taylor'),
                ('a7777777-7777-7777-7777-777777777777', 'Jennifer Brown'),
                ('a8888888-8888-8888-8888-888888888888', 'James Wilson');
            ");

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT Locations ON;
                INSERT INTO Locations (Id, StreetName, PostalCode, City)
                VALUES 
                (1, 'Drottninggatan 95', '113 60', 'Stockholm'),
                (2, 'Kungsgatan 12', '411 19', 'Gothenburg'),
                (3, 'Stortorget 7', '211 34', 'Malmö'),
                (4, 'Sveavägen 44', '111 34', 'Stockholm'),
                (5, 'Avenyn 15', '411 36', 'Gothenburg');
                SET IDENTITY_INSERT Locations OFF;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Participants (Id, FirstName, LastName, Email, PhoneNumber)
                VALUES 
                ('b1111111-1111-1111-1111-111111111111', 'Alice', 'Karlsson', 'alice.karlsson@email.com', '+46701234567'),
                ('b2222222-2222-2222-2222-222222222222', 'Bob', 'Andersson', 'bob.andersson@email.com', '+46702345678'),
                ('b3333333-3333-3333-3333-333333333333', 'Charlie', 'Johansson', 'charlie.johansson@email.com', '+46703456789'),
                ('b4444444-4444-4444-4444-444444444444', 'Diana', 'Eriksson', 'diana.eriksson@email.com', '+46704567890'),
                ('b5555555-5555-5555-5555-555555555555', 'Erik', 'Larsson', 'erik.larsson@email.com', '+46705678901'),
                ('b6666666-6666-6666-6666-666666666666', 'Fiona', 'Olsson', 'fiona.olsson@email.com', '+46706789012'),
                ('b7777777-7777-7777-7777-777777777777', 'Gustav', 'Nilsson', 'gustav.nilsson@email.com', '+46707890123'),
                ('b8888888-8888-8888-8888-888888888888', 'Hannah', 'Persson', 'hannah.persson@email.com', '+46708901234'),
                ('b9999999-9999-9999-9999-999999999999', 'Ivan', 'Svensson', 'ivan.svensson@email.com', '+46709012345'),
                ('ba111111-1111-1111-1111-111111111111', 'Julia', 'Gustafsson', 'julia.gustafsson@email.com', '+46700123456'),
                ('bb222222-2222-2222-2222-222222222222', 'Karl', 'Pettersson', 'karl.pettersson@email.com', '+46701112222'),
                ('bc333333-3333-3333-3333-333333333333', 'Linda', 'Jonsson', 'linda.jonsson@email.com', '+46702223333'),
                ('bd444444-4444-4444-4444-444444444444', 'Marcus', 'Hansson', 'marcus.hansson@email.com', '+46703334444'),
                ('be555555-5555-5555-5555-555555555555', 'Nina', 'Bengtsson', 'nina.bengtsson@email.com', '+46704445555'),
                ('bf666666-6666-6666-6666-666666666666', 'Oscar', 'Berg', 'oscar.berg@email.com', '+46705556666');
            ");

            migrationBuilder.Sql(@"
                DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
                
                INSERT INTO CourseEvents (Id, CourseId, EventDate, Price, Seats, CourseEventTypeId)
                VALUES 
                ('c1111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', DATEADD(DAY, 10, @Today), 4500, 20, 1),
                ('c1111112-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', DATEADD(DAY, 45, @Today), 4500, 25, 2),
                ('c2222221-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', DATEADD(DAY, 15, @Today), 7500, 15, 3),
                ('c2222222-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', DATEADD(DAY, 60, @Today), 7200, 18, 1),
                ('c3333331-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', DATEADD(DAY, 20, @Today), 5500, 22, 2),
                ('c3333332-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', DATEADD(DAY, 55, @Today), 5500, 20, 4),
                ('c4444441-4444-4444-4444-444444444444', '44444444-4444-4444-4444-444444444444', DATEADD(DAY, 25, @Today), 6500, 16, 1),
                ('c4444442-4444-4444-4444-444444444444', '44444444-4444-4444-4444-444444444444', DATEADD(DAY, 70, @Today), 6500, 18, 5),
                ('c5555551-5555-5555-5555-555555555555', '55555555-5555-5555-5555-555555555555', DATEADD(DAY, 30, @Today), 8500, 12, 3),
                ('c5555552-5555-5555-5555-555555555555', '55555555-5555-5555-5555-555555555555', DATEADD(DAY, 75, @Today), 8200, 14, 2),
                ('c6666661-6666-6666-6666-666666666666', '66666666-6666-6666-6666-666666666666', DATEADD(DAY, 35, @Today), 7000, 18, 4),
                ('c6666662-6666-6666-6666-666666666666', '66666666-6666-6666-6666-666666666666', DATEADD(DAY, 80, @Today), 7000, 20, 1),
                ('c7777771-7777-7777-7777-777777777777', '77777777-7777-7777-7777-777777777777', DATEADD(DAY, 40, @Today), 9500, 10, 2),
                ('c7777772-7777-7777-7777-777777777777', '77777777-7777-7777-7777-777777777777', DATEADD(DAY, 90, @Today), 9200, 12, 3),
                ('c8888881-8888-8888-8888-888888888888', '88888888-8888-8888-8888-888888888888', DATEADD(DAY, 50, @Today), 12000, 15, 1),
                ('c8888882-8888-8888-8888-888888888888', '88888888-8888-8888-8888-888888888888', DATEADD(DAY, 100, @Today), 11500, 16, 5);
            ");

            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT InPlaceLocations ON;
                INSERT INTO InPlaceLocations (Id, LocationId, RoomNumber, Seats)
                VALUES 
                (1, 1, 101, 25),
                (2, 1, 102, 30),
                (3, 1, 201, 20),
                (4, 2, 101, 22),
                (5, 2, 102, 28),
                (6, 3, 101, 18),
                (7, 3, 201, 24),
                (8, 4, 301, 15),
                (9, 4, 302, 26),
                (10, 5, 101, 20);
                SET IDENTITY_INSERT InPlaceLocations OFF;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO CourseRegistrations (Id, ParticipantId, CourseEventId, RegistrationDate, IsPaid)
                VALUES 
                (NEWID(), 'b1111111-1111-1111-1111-111111111111', 'c1111111-1111-1111-1111-111111111111', DATEADD(DAY, -5, GETUTCDATE()), 1),
                (NEWID(), 'b2222222-2222-2222-2222-222222222222', 'c1111111-1111-1111-1111-111111111111', DATEADD(DAY, -4, GETUTCDATE()), 1),
                (NEWID(), 'b3333333-3333-3333-3333-333333333333', 'c1111111-1111-1111-1111-111111111111', DATEADD(DAY, -3, GETUTCDATE()), 0),
                (NEWID(), 'b4444444-4444-4444-4444-444444444444', 'c2222221-2222-2222-2222-222222222222', DATEADD(DAY, -7, GETUTCDATE()), 1),
                (NEWID(), 'b5555555-5555-5555-5555-555555555555', 'c2222221-2222-2222-2222-222222222222', DATEADD(DAY, -6, GETUTCDATE()), 1),
                (NEWID(), 'b6666666-6666-6666-6666-666666666666', 'c2222221-2222-2222-2222-222222222222', DATEADD(DAY, -5, GETUTCDATE()), 0),
                (NEWID(), 'b7777777-7777-7777-7777-777777777777', 'c3333331-3333-3333-3333-333333333333', DATEADD(DAY, -8, GETUTCDATE()), 1),
                (NEWID(), 'b8888888-8888-8888-8888-888888888888', 'c3333331-3333-3333-3333-333333333333', DATEADD(DAY, -7, GETUTCDATE()), 1),
                (NEWID(), 'b9999999-9999-9999-9999-999999999999', 'c3333331-3333-3333-3333-333333333333', DATEADD(DAY, -6, GETUTCDATE()), 1),
                (NEWID(), 'ba111111-1111-1111-1111-111111111111', 'c3333331-3333-3333-3333-333333333333', DATEADD(DAY, -5, GETUTCDATE()), 0),
                (NEWID(), 'bb222222-2222-2222-2222-222222222222', 'c4444441-4444-4444-4444-444444444444', DATEADD(DAY, -9, GETUTCDATE()), 1),
                (NEWID(), 'bc333333-3333-3333-3333-333333333333', 'c4444441-4444-4444-4444-444444444444', DATEADD(DAY, -8, GETUTCDATE()), 1),
                (NEWID(), 'bd444444-4444-4444-4444-444444444444', 'c5555551-5555-5555-5555-555555555555', DATEADD(DAY, -10, GETUTCDATE()), 1),
                (NEWID(), 'be555555-5555-5555-5555-555555555555', 'c5555551-5555-5555-5555-555555555555', DATEADD(DAY, -9, GETUTCDATE()), 0),
                (NEWID(), 'bf666666-6666-6666-6666-666666666666', 'c5555551-5555-5555-5555-555555555555', DATEADD(DAY, -8, GETUTCDATE()), 1),
                (NEWID(), 'b1111111-1111-1111-1111-111111111111', 'c6666661-6666-6666-6666-666666666666', DATEADD(DAY, -11, GETUTCDATE()), 1),
                (NEWID(), 'b3333333-3333-3333-3333-333333333333', 'c6666661-6666-6666-6666-666666666666', DATEADD(DAY, -10, GETUTCDATE()), 1),
                (NEWID(), 'b5555555-5555-5555-5555-555555555555', 'c7777771-7777-7777-7777-777777777777', DATEADD(DAY, -12, GETUTCDATE()), 0),
                (NEWID(), 'b7777777-7777-7777-7777-777777777777', 'c7777771-7777-7777-7777-777777777777', DATEADD(DAY, -11, GETUTCDATE()), 1),
                (NEWID(), 'b9999999-9999-9999-9999-999999999999', 'c8888881-8888-8888-8888-888888888888', DATEADD(DAY, -13, GETUTCDATE()), 1),
                (NEWID(), 'bb222222-2222-2222-2222-222222222222', 'c8888881-8888-8888-8888-888888888888', DATEADD(DAY, -12, GETUTCDATE()), 1),
                (NEWID(), 'bd444444-4444-4444-4444-444444444444', 'c8888881-8888-8888-8888-888888888888', DATEADD(DAY, -11, GETUTCDATE()), 0);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO CourseEventInstructors (CourseEventId, InstructorId)
                VALUES 
                ('c1111111-1111-1111-1111-111111111111', 'a1111111-1111-1111-1111-111111111111'),
                ('c1111112-1111-1111-1111-111111111111', 'a2222222-2222-2222-2222-222222222222'),
                ('c2222221-2222-2222-2222-222222222222', 'a3333333-3333-3333-3333-333333333333'),
                ('c2222222-2222-2222-2222-222222222222', 'a3333333-3333-3333-3333-333333333333'),
                ('c3333331-3333-3333-3333-333333333333', 'a4444444-4444-4444-4444-444444444444'),
                ('c3333331-3333-3333-3333-333333333333', 'a5555555-5555-5555-5555-555555555555'),
                ('c3333332-3333-3333-3333-333333333333', 'a4444444-4444-4444-4444-444444444444'),
                ('c4444441-4444-4444-4444-444444444444', 'a6666666-6666-6666-6666-666666666666'),
                ('c4444442-4444-4444-4444-444444444444', 'a6666666-6666-6666-6666-666666666666'),
                ('c5555551-5555-5555-5555-555555555555', 'a7777777-7777-7777-7777-777777777777'),
                ('c5555552-5555-5555-5555-555555555555', 'a7777777-7777-7777-7777-777777777777'),
                ('c6666661-6666-6666-6666-666666666666', 'a8888888-8888-8888-8888-888888888888'),
                ('c6666661-6666-6666-6666-666666666666', 'a3333333-3333-3333-3333-333333333333'),
                ('c6666662-6666-6666-6666-666666666666', 'a8888888-8888-8888-8888-888888888888'),
                ('c7777771-7777-7777-7777-777777777777', 'a2222222-2222-2222-2222-222222222222'),
                ('c7777771-7777-7777-7777-777777777777', 'a7777777-7777-7777-7777-777777777777'),
                ('c7777772-7777-7777-7777-777777777777', 'a2222222-2222-2222-2222-222222222222'),
                ('c8888881-8888-8888-8888-888888888888', 'a4444444-4444-4444-4444-444444444444'),
                ('c8888882-8888-8888-8888-888888888888', 'a4444444-4444-4444-4444-444444444444');
            ");

            migrationBuilder.Sql(@"
                INSERT INTO InPlaceEventLocations (CourseEventId, InPlaceLocationId)
                VALUES 
                ('c1111112-1111-1111-1111-111111111111', 1),
                ('c2222221-2222-2222-2222-222222222222', 2),
                ('c3333331-3333-3333-3333-333333333333', 3),
                ('c5555552-5555-5555-5555-555555555555', 4),
                ('c6666661-6666-6666-6666-666666666666', 5),
                ('c7777771-7777-7777-7777-777777777777', 6),
                ('c7777772-7777-7777-7777-777777777777', 7);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM InPlaceEventLocations");
            migrationBuilder.Sql("DELETE FROM CourseEventInstructors");
            migrationBuilder.Sql("DELETE FROM CourseRegistrations");
            migrationBuilder.Sql("DELETE FROM InPlaceLocations");
            migrationBuilder.Sql("DELETE FROM CourseEvents");
            migrationBuilder.Sql("DELETE FROM Participants");
            migrationBuilder.Sql("DELETE FROM Locations");
            migrationBuilder.Sql("DELETE FROM Instructors");
            migrationBuilder.Sql("DELETE FROM Courses");
            migrationBuilder.Sql("DELETE FROM CourseEventTypes");
        }
    }
}
