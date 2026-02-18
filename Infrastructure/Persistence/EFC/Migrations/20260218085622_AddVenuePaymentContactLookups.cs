using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddVenuePaymentContactLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('Participants', 'ContactTypeId') IS NULL
                BEGIN
                    ALTER TABLE Participants
                    ADD ContactTypeId int NOT NULL CONSTRAINT DF_Participants_ContactTypeId DEFAULT 1;
                END
                """);

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

            migrationBuilder.InsertData(
                table: "ParticipantContactTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Primary" },
                    { 2, "Billing" },
                    { 3, "Emergency" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantContactTypes_Name",
                table: "ParticipantContactTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_ParticipantContactTypes_ContactTypeId",
                table: "Participants",
                column: "ContactTypeId",
                principalTable: "ParticipantContactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_ParticipantContactTypes_ContactTypeId",
                table: "Participants");

            migrationBuilder.DropTable(
                name: "ParticipantContactTypes");
        }
    }
}
