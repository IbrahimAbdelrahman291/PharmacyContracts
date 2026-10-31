using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChequePaymentTransactionDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfTransaction",
                schema: "claims",
                table: "Cheques",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeOfPay",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfTransaction",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "TypeOfPay",
                schema: "claims",
                table: "Cheques");
        }
    }
}
