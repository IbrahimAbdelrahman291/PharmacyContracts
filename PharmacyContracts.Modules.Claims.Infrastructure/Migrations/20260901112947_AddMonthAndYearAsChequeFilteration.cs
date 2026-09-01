using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthAndYearAsChequeFilteration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClaimMonth",
                schema: "claims",
                table: "Cheques",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClaimYear",
                schema: "claims",
                table: "Cheques",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_PharmacyId_CompanyName_ClaimMonth_ClaimYear",
                schema: "claims",
                table: "Cheques",
                columns: new[] { "PharmacyId", "CompanyName", "ClaimMonth", "ClaimYear" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_PharmacyId_CompanyName_ClaimMonth_ClaimYear",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ClaimMonth",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ClaimYear",
                schema: "claims",
                table: "Cheques");
        }
    }
}
