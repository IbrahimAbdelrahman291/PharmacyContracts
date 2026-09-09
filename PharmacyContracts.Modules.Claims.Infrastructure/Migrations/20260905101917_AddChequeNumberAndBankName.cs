using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChequeNumberAndBankName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChequeNumber",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE [claims].[Cheques]
                SET [ChequeNumber] = CONCAT('LEGACY-', CONVERT(nvarchar(36), [Id])),
                    [BankName] = N'غير محدد'
                WHERE [ChequeNumber] = N'';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques",
                columns: new[] { "PharmacyId", "ChequeNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cheques_PharmacyId_ChequeNumber",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "BankName",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ChequeNumber",
                schema: "claims",
                table: "Cheques");
        }
    }
}
