using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    [DbContext(typeof(ClaimsDbContext))]
    [Migration("20261030190000_AllowDuplicateChequeNumbers")]
    public partial class AllowDuplicateChequeNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques",
                columns: new[] { "PharmacyId", "ChequeNumber" },
                filter: "[ChequeNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques",
                columns: new[] { "PharmacyId", "ChequeNumber" },
                unique: true,
                filter: "[ChequeNumber] IS NOT NULL");
        }
    }
}
