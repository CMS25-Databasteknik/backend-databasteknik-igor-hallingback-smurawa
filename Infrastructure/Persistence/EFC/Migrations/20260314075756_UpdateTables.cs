using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InstructorRoles_RoleName_NotEmpty",
                table: "InstructorRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseEventTypes_TypeName_NotEmpty",
                table: "CourseEventTypes");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "InstructorRoles",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorRoles_RoleName",
                table: "InstructorRoles",
                newName: "IX_InstructorRoles_Name");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "CourseEventTypes",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEventTypes_TypeName",
                table: "CourseEventTypes",
                newName: "IX_CourseEventTypes_Name");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InstructorRoles_RoleName_NotEmpty",
                table: "InstructorRoles",
                sql: "LEN([Name]) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseEventTypes_TypeName_NotEmpty",
                table: "CourseEventTypes",
                sql: "LTRIM(RTRIM([Name])) <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InstructorRoles_RoleName_NotEmpty",
                table: "InstructorRoles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseEventTypes_TypeName_NotEmpty",
                table: "CourseEventTypes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "InstructorRoles",
                newName: "RoleName");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorRoles_Name",
                table: "InstructorRoles",
                newName: "IX_InstructorRoles_RoleName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CourseEventTypes",
                newName: "TypeName");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEventTypes_Name",
                table: "CourseEventTypes",
                newName: "IX_CourseEventTypes_TypeName");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InstructorRoles_RoleName_NotEmpty",
                table: "InstructorRoles",
                sql: "LEN([RoleName]) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseEventTypes_TypeName_NotEmpty",
                table: "CourseEventTypes",
                sql: "LTRIM(RTRIM([TypeName])) <> ''");
        }
    }
}
