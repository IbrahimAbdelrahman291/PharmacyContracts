using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimAmountAndPrescriptionCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ClaimAmount",
                schema: "claims",
                table: "Claims",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CorrectedPrescriptionsCount",
                schema: "claims",
                table: "Claims",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrescriptionsCount",
                schema: "claims",
                table: "Claims",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CorrectedPrescriptionsCount",
                schema: "claims",
                table: "ClaimReviews",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimAmount",
                schema: "claims",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "CorrectedPrescriptionsCount",
                schema: "claims",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "PrescriptionsCount",
                schema: "claims",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "CorrectedPrescriptionsCount",
                schema: "claims",
                table: "ClaimReviews");
        }
    }
}
