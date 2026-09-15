using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    [DbContext(typeof(ClaimsDbContext))]
    [Migration("20260915120000_MakeChequeDetailsNullable")]
    public partial class MakeChequeDetailsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.AlterColumn<string>(
                name: "ChequeNumber",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques",
                columns: new[] { "PharmacyId", "ChequeNumber" },
                unique: true,
                filter: "[ChequeNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.Sql("UPDATE [claims].[Cheques] SET [ChequeNumber] = CONCAT('LEGACY-', CONVERT(nvarchar(36), [Id])) WHERE [ChequeNumber] IS NULL;");
            migrationBuilder.Sql("UPDATE [claims].[Cheques] SET [BankName] = N'غير محدد' WHERE [BankName] IS NULL;");

            migrationBuilder.AlterColumn<string>(
                name: "ChequeNumber",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques",
                columns: new[] { "PharmacyId", "ChequeNumber" },
                unique: true);
        }
    }
}
