using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChequeFinancialBreakdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdministrativeExpensesPercentage",
                schema: "claims",
                table: "Cheques",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterDiscount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountBeforeDiscount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountDifference",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalAmount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentDifference",
                schema: "claims",
                table: "Cheques",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentDifferenceType",
                schema: "claims",
                table: "Cheques",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Equal");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                schema: "claims",
                table: "Cheques",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                WITH ChequeBreakdown AS
                (
                    SELECT
                        ch.Id,
                        ch.Amount AS PaidAmount,
                        ROUND(cl.ClaimAmount * ch.Amount / NULLIF(SUM(ch.Amount) OVER (PARTITION BY ch.ClaimId), 0), 2) AS AmountBeforeDiscount,
                        ROUND(cl.ClaimAmountAfterDiscount * ch.Amount / NULLIF(SUM(ch.Amount) OVER (PARTITION BY ch.ClaimId), 0), 2) AS AmountAfterDiscount,
                        co.TaxPercentage,
                        co.AdministrativeExpensesPercentage
                    FROM [claims].[Cheques] ch
                    INNER JOIN [claims].[Claims] cl ON cl.Id = ch.ClaimId
                    INNER JOIN [companies].[Companies] co
                        ON co.PharmacyId = ch.PharmacyId AND co.Name = ch.CompanyName
                )
                UPDATE ch
                SET
                    ch.PaidAmount = b.PaidAmount,
                    ch.AmountBeforeDiscount = b.AmountBeforeDiscount,
                    ch.AmountAfterDiscount = b.AmountAfterDiscount,
                    ch.DiscountDifference = ROUND(b.AmountBeforeDiscount - b.AmountAfterDiscount, 2),
                    ch.TaxPercentage = b.TaxPercentage,
                    ch.AdministrativeExpensesPercentage = b.AdministrativeExpensesPercentage,
                    ch.FinalAmount = valuesToApply.FinalAmount,
                    ch.PaymentDifference = ABS(ROUND(b.PaidAmount - valuesToApply.FinalAmount, 2)),
                    ch.PaymentDifferenceType = CASE
                        WHEN b.PaidAmount > valuesToApply.FinalAmount THEN 'Increase'
                        WHEN b.PaidAmount < valuesToApply.FinalAmount THEN 'Decrease'
                        ELSE 'Equal'
                    END
                FROM [claims].[Cheques] ch
                INNER JOIN ChequeBreakdown b ON b.Id = ch.Id
                CROSS APPLY
                (
                    SELECT ROUND(
                        b.AmountAfterDiscount *
                        (1 - (b.TaxPercentage + b.AdministrativeExpensesPercentage) / 100),
                        2) AS FinalAmount
                ) valuesToApply;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdministrativeExpensesPercentage",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "AmountAfterDiscount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "AmountBeforeDiscount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "DiscountDifference",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "FinalAmount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "PaymentDifference",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "PaymentDifferenceType",
                schema: "claims",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                schema: "claims",
                table: "Cheques");
        }
    }
}
