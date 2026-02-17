using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class SeedInstructorRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM InstructorRoles WHERE RoleName = 'Lead')
                    INSERT INTO InstructorRoles (RoleName) VALUES ('Lead');
                IF NOT EXISTS (SELECT 1 FROM InstructorRoles WHERE RoleName = 'Assistant')
                    INSERT INTO InstructorRoles (RoleName) VALUES ('Assistant');
                IF NOT EXISTS (SELECT 1 FROM InstructorRoles WHERE RoleName = 'Guest')
                    INSERT INTO InstructorRoles (RoleName) VALUES ('Guest');

                DECLARE @LeadId      int = (SELECT TOP 1 Id FROM InstructorRoles WHERE RoleName = 'Lead');
                DECLARE @AssistantId int = (SELECT TOP 1 Id FROM InstructorRoles WHERE RoleName = 'Assistant');
                DECLARE @GuestId     int = (SELECT TOP 1 Id FROM InstructorRoles WHERE RoleName = 'Guest');

                UPDATE Instructors
                SET InstructorRoleId = CASE
                    WHEN Id IN ('a1111111-1111-1111-1111-111111111111','a5555555-5555-5555-5555-555555555555') THEN @LeadId
                    WHEN Id IN ('a2222222-2222-2222-2222-222222222222','a6666666-6666-6666-6666-666666666666','a8888888-8888-8888-8888-888888888888') THEN @AssistantId
                    WHEN Id IN ('a3333333-3333-3333-3333-333333333333','a4444444-4444-4444-4444-444444444444','a7777777-7777-7777-7777-777777777777') THEN @GuestId
                    ELSE InstructorRoleId
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM InstructorRoles WHERE RoleName IN ('Lead','Assistant','Guest');");
        }
    }
}
