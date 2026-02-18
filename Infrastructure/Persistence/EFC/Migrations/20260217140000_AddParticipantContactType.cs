using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    public partial class AddParticipantContactType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ParticipantContactTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantContactTypes_Id", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "ContactTypeId",
                table: "Participants",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantContactTypes_Name",
                table: "ParticipantContactTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ContactTypeId",
                table: "Participants",
                column: "ContactTypeId");

            migrationBuilder.InsertData(
                table: "ParticipantContactTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Primary" },
                    { 2, "Billing" },
                    { 3, "Emergency" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_ParticipantContactTypes_ContactTypeId",
                table: "Participants",
                column: "ContactTypeId",
                principalTable: "ParticipantContactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_ParticipantContactTypes_ContactTypeId",
                table: "Participants");

            migrationBuilder.DropTable(
                name: "ParticipantContactTypes");

            migrationBuilder.DropIndex(
                name: "IX_Participants_ContactTypeId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ContactTypeId",
                table: "Participants");
        }
    }
}
