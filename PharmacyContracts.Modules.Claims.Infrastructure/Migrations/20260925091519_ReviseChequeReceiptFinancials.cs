using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReviseChequeReceiptFinancials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "AmountAfterDiscount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                schema: "claims",
                table: "Cheques",
                newName: "CorrectAmount");

            migrationBuilder.RenameColumn(
                name: "DiscountDifference",
                schema: "claims",
                table: "Cheques",
                newName: "AmountDifference");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentDifferenceType",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaymentDifference",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualAmount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChequeDate",
                schema: "claims",
                table: "Cheques",
                type: "datetime2",
                nullable: true);

            // The former payment difference was calculated from the allocated
            // (correct) amount, not an amount actually received by the pharmacy.
            // It must remain unknown until receipt details are entered.
            migrationBuilder.Sql(
                """
                UPDATE claims.Cheques
                SET PaymentDifference = NULL,
                    PaymentDifferenceType = NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualAmount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ChequeDate",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.RenameColumn(
                name: "CorrectAmount",
                schema: "claims",
                table: "Cheques",
                newName: "PaidAmount");

            migrationBuilder.RenameColumn(
                name: "AmountDifference",
                schema: "claims",
                table: "Cheques",
                newName: "DiscountDifference");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentDifferenceType",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaymentDifference",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterDiscount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE claims.Cheques
                SET Amount = PaidAmount,
                    AmountAfterDiscount = AmountBeforeDiscount - DiscountDifference;
                """);
        }
    }
}
