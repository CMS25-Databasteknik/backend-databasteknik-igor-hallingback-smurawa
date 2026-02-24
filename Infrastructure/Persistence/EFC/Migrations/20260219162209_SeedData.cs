using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM PaymentMethods WHERE Id = 1)
                BEGIN
                    INSERT INTO PaymentMethods (Id, Name)
                    VALUES
                    (1, 'Card'),
                    (2, 'Invoice'),
                    (3, 'Cash');
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM ParticipantContactTypes WHERE Id = 1)
                BEGIN
                    INSERT INTO ParticipantContactTypes (Id, Name)
                    VALUES
                    (1, 'Primary'),
                    (2, 'Billing'),
                    (3, 'Emergency');
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM VenueTypes WHERE Id = 1)
                BEGIN
                    INSERT INTO VenueTypes (Id, Name)
                    VALUES
                    (1, 'InPerson'),
                    (2, 'Online'),
                    (3, 'Hybrid');
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM CourseRegistrationStatuses WHERE Id = 0)
                BEGIN
                    INSERT INTO CourseRegistrationStatuses (Id, Name)
                    VALUES
                    (0, 'Pending'),
                    (1, 'Paid'),
                    (2, 'Cancelled'),
                    (3, 'Refunded');
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM InstructorRoles WHERE RoleName = 'Lead')
                    INSERT INTO InstructorRoles (RoleName) VALUES ('Lead');
                IF NOT EXISTS (SELECT 1 FROM InstructorRoles WHERE RoleName = 'Assistant')
                    INSERT INTO InstructorRoles (RoleName) VALUES ('Assistant');
                IF NOT EXISTS (SELECT 1 FROM InstructorRoles WHERE RoleName = 'Guest')
                    INSERT INTO InstructorRoles (RoleName) VALUES ('Guest');
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM CourseEventTypes WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT CourseEventTypes ON;
                    INSERT INTO CourseEventTypes (Id, TypeName)
                    VALUES
                    (1, 'Online'),
                    (2, 'In-Person'),
                    (3, 'Hybrid'),
                    (4, 'Workshop'),
                    (5, 'Webinar');
                    SET IDENTITY_INSERT CourseEventTypes OFF;
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Courses WHERE Id = '11111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO Courses (Id, Title, Description, DurationInDays)
                    VALUES
                    ('11111111-1111-1111-1111-111111111111', 'Introduction to C# Programming', 'Learn the fundamentals of C# and object-oriented programming concepts.', 30),
                    ('22222222-2222-2222-2222-222222222222', 'Advanced ASP.NET Core Development', 'Master advanced ASP.NET Core concepts and build REST APIs.', 45),
                    ('33333333-3333-3333-3333-333333333333', 'Database Design with Entity Framework', 'Database design, SQL, and Entity Framework Core ORM.', 25),
                    ('44444444-4444-4444-4444-444444444444', 'React and TypeScript Fundamentals', 'Build modern web applications using React and TypeScript.', 35),
                    ('55555555-5555-5555-5555-555555555555', 'Cloud Architecture with Azure', 'Design and implement scalable solutions with Azure services.', 40);
                END
                """);

            migrationBuilder.Sql("""
                DECLARE @LeadId int = (SELECT TOP 1 Id FROM InstructorRoles WHERE RoleName = 'Lead');
                DECLARE @AssistantId int = (SELECT TOP 1 Id FROM InstructorRoles WHERE RoleName = 'Assistant');
                DECLARE @GuestId int = (SELECT TOP 1 Id FROM InstructorRoles WHERE RoleName = 'Guest');

                IF NOT EXISTS (SELECT 1 FROM Instructors WHERE Id = 'a1111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO Instructors (Id, Name, InstructorRoleId)
                    VALUES
                    ('a1111111-1111-1111-1111-111111111111', 'Emma Johnson', @LeadId),
                    ('a2222222-2222-2222-2222-222222222222', 'Michael Chen', @AssistantId),
                    ('a3333333-3333-3333-3333-333333333333', 'Sarah Williams', @GuestId),
                    ('a4444444-4444-4444-4444-444444444444', 'David Anderson', @GuestId),
                    ('a5555555-5555-5555-5555-555555555555', 'Lisa Martinez', @LeadId);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Locations WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT Locations ON;
                    INSERT INTO Locations (Id, StreetName, PostalCode, City)
                    VALUES
                    (1, 'Drottninggatan 95', '113 60', 'Stockholm'),
                    (2, 'Kungsgatan 12', '411 19', 'Gothenburg'),
                    (3, 'Stortorget 7', '211 34', 'Malmo');
                    SET IDENTITY_INSERT Locations OFF;
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM Participants WHERE Id = 'b1111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO Participants (Id, FirstName, LastName, Email, PhoneNumber, ContactTypeId)
                    VALUES
                    ('b1111111-1111-1111-1111-111111111111', 'Alice', 'Karlsson', 'alice.karlsson@email.com', '+46701234567', 1),
                    ('b2222222-2222-2222-2222-222222222222', 'Bob', 'Andersson', 'bob.andersson@email.com', '+46702345678', 1),
                    ('b3333333-3333-3333-3333-333333333333', 'Charlie', 'Johansson', 'charlie.johansson@email.com', '+46703456789', 1),
                    ('b4444444-4444-4444-4444-444444444444', 'Diana', 'Eriksson', 'diana.eriksson@email.com', '+46704567890', 1),
                    ('b5555555-5555-5555-5555-555555555555', 'Erik', 'Larsson', 'erik.larsson@email.com', '+46705678901', 1);
                END
                """);

            migrationBuilder.Sql("""
                DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
                IF NOT EXISTS (SELECT 1 FROM CourseEvents WHERE Id = 'c1111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO CourseEvents (Id, CourseId, EventDate, Price, Seats, CourseEventTypeId, VenueTypeId)
                    VALUES
                    ('c1111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', DATEADD(DAY, 10, @Today), 4500, 20, 1, 2),
                    ('c2222221-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', DATEADD(DAY, 15, @Today), 7500, 15, 3, 2),
                    ('c3333331-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', DATEADD(DAY, 20, @Today), 5500, 22, 2, 1),
                    ('c4444441-4444-4444-4444-444444444444', '44444444-4444-4444-4444-444444444444', DATEADD(DAY, 25, @Today), 6500, 16, 5, 2),
                    ('c5555551-5555-5555-5555-555555555555', '55555555-5555-5555-5555-555555555555', DATEADD(DAY, 30, @Today), 8500, 12, 3, 3);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM InPlaceLocations WHERE Id = 1)
                BEGIN
                    SET IDENTITY_INSERT InPlaceLocations ON;
                    INSERT INTO InPlaceLocations (Id, LocationId, RoomNumber, Seats)
                    VALUES
                    (1, 1, 101, 25),
                    (2, 1, 102, 30),
                    (3, 2, 101, 22),
                    (4, 3, 201, 24);
                    SET IDENTITY_INSERT InPlaceLocations OFF;
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM CourseRegistrations WHERE ParticipantId = 'b1111111-1111-1111-1111-111111111111' AND CourseEventId = 'c1111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO CourseRegistrations (Id, ParticipantId, CourseEventId, RegistrationDate, CourseRegistrationStatusId, PaymentMethodId)
                    VALUES
                    (NEWID(), 'b1111111-1111-1111-1111-111111111111', 'c1111111-1111-1111-1111-111111111111', DATEADD(DAY, -5, GETUTCDATE()), 1, 1),
                    (NEWID(), 'b2222222-2222-2222-2222-222222222222', 'c1111111-1111-1111-1111-111111111111', DATEADD(DAY, -4, GETUTCDATE()), 0, 2),
                    (NEWID(), 'b3333333-3333-3333-3333-333333333333', 'c2222221-2222-2222-2222-222222222222', DATEADD(DAY, -3, GETUTCDATE()), 1, 1),
                    (NEWID(), 'b4444444-4444-4444-4444-444444444444', 'c3333331-3333-3333-3333-333333333333', DATEADD(DAY, -2, GETUTCDATE()), 0, 3);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM CourseEventInstructors WHERE CourseEventId = 'c1111111-1111-1111-1111-111111111111' AND InstructorId = 'a1111111-1111-1111-1111-111111111111')
                BEGIN
                    INSERT INTO CourseEventInstructors (CourseEventId, InstructorId)
                    VALUES
                    ('c1111111-1111-1111-1111-111111111111', 'a1111111-1111-1111-1111-111111111111'),
                    ('c2222221-2222-2222-2222-222222222222', 'a2222222-2222-2222-2222-222222222222'),
                    ('c3333331-3333-3333-3333-333333333333', 'a3333333-3333-3333-3333-333333333333'),
                    ('c4444441-4444-4444-4444-444444444444', 'a4444444-4444-4444-4444-444444444444'),
                    ('c5555551-5555-5555-5555-555555555555', 'a5555555-5555-5555-5555-555555555555');
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM InPlaceEventLocations WHERE CourseEventId = 'c1111111-1111-1111-1111-111111111111' AND InPlaceLocationId = 1)
                BEGIN
                    INSERT INTO InPlaceEventLocations (CourseEventId, InPlaceLocationId)
                    VALUES
                    ('c1111111-1111-1111-1111-111111111111', 1),
                    ('c3333331-3333-3333-3333-333333333333', 2),
                    ('c4444441-4444-4444-4444-444444444444', 3);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM InPlaceEventLocations
                WHERE CourseEventId IN (
                    'c1111111-1111-1111-1111-111111111111',
                    'c2222221-2222-2222-2222-222222222222',
                    'c3333331-3333-3333-3333-333333333333',
                    'c4444441-4444-4444-4444-444444444444',
                    'c5555551-5555-5555-5555-555555555555'
                );

                DELETE FROM CourseEventInstructors
                WHERE CourseEventId IN (
                    'c1111111-1111-1111-1111-111111111111',
                    'c2222221-2222-2222-2222-222222222222',
                    'c3333331-3333-3333-3333-333333333333',
                    'c4444441-4444-4444-4444-444444444444',
                    'c5555551-5555-5555-5555-555555555555'
                );

                DELETE FROM CourseRegistrations
                WHERE CourseEventId IN (
                    'c1111111-1111-1111-1111-111111111111',
                    'c2222221-2222-2222-2222-222222222222',
                    'c3333331-3333-3333-3333-333333333333',
                    'c4444441-4444-4444-4444-444444444444',
                    'c5555551-5555-5555-5555-555555555555'
                );

                DELETE FROM InPlaceLocations WHERE Id IN (1,2,3,4);
                DELETE FROM CourseEvents WHERE Id IN (
                    'c1111111-1111-1111-1111-111111111111',
                    'c2222221-2222-2222-2222-222222222222',
                    'c3333331-3333-3333-3333-333333333333',
                    'c4444441-4444-4444-4444-444444444444',
                    'c5555551-5555-5555-5555-555555555555'
                );
                DELETE FROM Participants WHERE Id IN (
                    'b1111111-1111-1111-1111-111111111111',
                    'b2222222-2222-2222-2222-222222222222',
                    'b3333333-3333-3333-3333-333333333333',
                    'b4444444-4444-4444-4444-444444444444',
                    'b5555555-5555-5555-5555-555555555555'
                );
                DELETE FROM Instructors WHERE Id IN (
                    'a1111111-1111-1111-1111-111111111111',
                    'a2222222-2222-2222-2222-222222222222',
                    'a3333333-3333-3333-3333-333333333333',
                    'a4444444-4444-4444-4444-444444444444',
                    'a5555555-5555-5555-5555-555555555555'
                );
                DELETE FROM Locations WHERE Id IN (1,2,3);
                DELETE FROM Courses WHERE Id IN (
                    '11111111-1111-1111-1111-111111111111',
                    '22222222-2222-2222-2222-222222222222',
                    '33333333-3333-3333-3333-333333333333',
                    '44444444-4444-4444-4444-444444444444',
                    '55555555-5555-5555-5555-555555555555'
                );
                DELETE FROM CourseEventTypes WHERE Id IN (1,2,3,4,5);
                DELETE FROM InstructorRoles WHERE RoleName IN ('Lead','Assistant','Guest');
                DELETE FROM CourseRegistrationStatuses WHERE Id IN (0,1,2,3);
                DELETE FROM VenueTypes WHERE Id IN (1,2,3);
                DELETE FROM ParticipantContactTypes WHERE Id IN (1,2,3);
                DELETE FROM PaymentMethods WHERE Id IN (1,2,3);
                """);
        }
    }
}
