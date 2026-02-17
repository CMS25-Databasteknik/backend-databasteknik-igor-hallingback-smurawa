using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructorRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorRoles_Id", x => x.Id);
                    table.CheckConstraint("CK_InstructorRoles_RoleName_NotEmpty", "LEN([RoleName]) > 0");
                });

            migrationBuilder.InsertData(
                table: "InstructorRoles",
                columns: new[] { "RoleName" },
                values: new object[,]
                {
                    { "Lead" },
                    { "Assistant" },
                    { "Guest" }
                });

            migrationBuilder.AddColumn<int>(
                name: "InstructorRoleId",
                table: "Instructors",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("UPDATE Instructors SET InstructorRoleId = 1 WHERE InstructorRoleId = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_InstructorRoleId",
                table: "Instructors",
                column: "InstructorRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorRoles_RoleName",
                table: "InstructorRoles",
                column: "RoleName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_InstructorRoles_InstructorRoleId",
                table: "Instructors",
                column: "InstructorRoleId",
                principalTable: "InstructorRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_InstructorRoles_InstructorRoleId",
                table: "Instructors");

            migrationBuilder.DropTable(
                name: "InstructorRoles");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_InstructorRoleId",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "InstructorRoleId",
                table: "Instructors");
        }
    }
}
