using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class PendingCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenueTypeId",
                table: "CourseEvents",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "VenueTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueTypes_Id", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "VenueTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "InPerson" },
                    { 2, "Online" },
                    { 3, "Hybrid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvents_VenueTypeId",
                table: "CourseEvents",
                column: "VenueTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueTypes_Name",
                table: "VenueTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEvents_VenueTypes_VenueTypeId",
                table: "CourseEvents",
                column: "VenueTypeId",
                principalTable: "VenueTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEvents_VenueTypes_VenueTypeId",
                table: "CourseEvents");

            migrationBuilder.DropTable(
                name: "VenueTypes");

            migrationBuilder.DropIndex(
                name: "IX_CourseEvents_VenueTypeId",
                table: "CourseEvents");

            migrationBuilder.DropColumn(
                name: "VenueTypeId",
                table: "CourseEvents");
        }
    }
}
