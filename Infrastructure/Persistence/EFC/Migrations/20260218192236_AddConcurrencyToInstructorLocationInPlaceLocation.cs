using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrencyToInstructorLocationInPlaceLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "VenueTypes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "PaymentMethods",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "ParticipantContactTypes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "InstructorRoles",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "CourseRegistrationStatuses",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "Concurrency",
                table: "CourseEventTypes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "VenueTypes");

            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "ParticipantContactTypes");

            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "InstructorRoles");

            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "CourseRegistrationStatuses");

            migrationBuilder.DropColumn(
                name: "Concurrency",
                table: "CourseEventTypes");
        }
    }
}
