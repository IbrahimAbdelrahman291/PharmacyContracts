using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Companies.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "companies");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LocalDiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ImportedDiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    AdministrativeExpensesPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ChequeSettlementPeriodInDays = table.Column<int>(type: "int", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PharmacyId_Name",
                schema: "companies",
                table: "Companies",
                columns: new[] { "PharmacyId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies",
                schema: "companies");
        }
    }
}
