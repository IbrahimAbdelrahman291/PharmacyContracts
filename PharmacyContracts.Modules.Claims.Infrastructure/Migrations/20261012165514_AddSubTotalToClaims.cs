using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubTotalToClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                schema: "claims",
                table: "Claims",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE c
                SET c.SubTotal = totals.SubTotal
                FROM claims.Claims AS c
                INNER JOIN (
                    SELECT PharmacyId,
                           CustomerCompanyName,
                           MONTH(SaleDate) AS [Month],
                           YEAR(SaleDate) AS [Year],
                           SUM(SubTotal) AS SubTotal
                    FROM sales.SalesRecords
                    GROUP BY PharmacyId, CustomerCompanyName, MONTH(SaleDate), YEAR(SaleDate)
                ) AS totals
                    ON totals.PharmacyId = c.PharmacyId
                    AND totals.CustomerCompanyName = c.CompanyName
                    AND totals.[Month] = c.[Month]
                    AND totals.[Year] = c.[Year];
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTotal",
                schema: "claims",
                table: "Claims");
        }
    }
}
