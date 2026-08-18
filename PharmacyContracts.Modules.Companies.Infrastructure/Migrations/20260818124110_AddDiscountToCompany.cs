using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Companies.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                schema: "companies",
                table: "Companies",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                schema: "companies",
                table: "Companies");
        }
    }
}
