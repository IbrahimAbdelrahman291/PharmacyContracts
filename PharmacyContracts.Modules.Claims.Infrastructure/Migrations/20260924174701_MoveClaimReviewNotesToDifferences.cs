using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveClaimReviewNotesToDifferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "claims",
                table: "ClaimReviews");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "claims",
                table: "ClaimReviewDifferences",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "claims",
                table: "ClaimReviewDifferences");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "claims",
                table: "ClaimReviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
