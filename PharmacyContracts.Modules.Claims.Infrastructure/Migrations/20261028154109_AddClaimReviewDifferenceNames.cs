using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    public partial class AddClaimReviewDifferenceNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                schema: "claims",
                table: "ClaimReviewDifferences",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PharmacyName",
                schema: "claims",
                table: "ClaimReviewDifferences",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "EmployeeName", schema: "claims", table: "ClaimReviewDifferences");
            migrationBuilder.DropColumn(name: "PharmacyName", schema: "claims", table: "ClaimReviewDifferences");
        }
    }
}
