using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimReviewDifferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DifferenceAmount",
                schema: "claims",
                table: "ClaimReviews",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DifferenceType",
                schema: "claims",
                table: "ClaimReviews",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NoDifference");

            migrationBuilder.CreateTable(
                name: "ClaimReviewDifferences",
                schema: "claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimReviewDifferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimReviewDifferences_ClaimReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "claims",
                        principalTable: "ClaimReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimReviewDifferences_PharmacyId",
                schema: "claims",
                table: "ClaimReviewDifferences",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimReviewDifferences_ReviewId",
                schema: "claims",
                table: "ClaimReviewDifferences",
                column: "ReviewId");

            migrationBuilder.Sql(
                """
                UPDATE r
                SET r.DifferenceAmount = ABS(r.CorrectedAmount - c.ClaimAmount),
                    r.DifferenceType = CASE
                        WHEN r.CorrectedAmount > c.ClaimAmount THEN 'Increase'
                        WHEN r.CorrectedAmount < c.ClaimAmount THEN 'Decrease'
                        ELSE 'NoDifference'
                    END
                FROM claims.ClaimReviews r
                INNER JOIN claims.Claims c ON c.Id = r.ClaimId
                WHERE r.CorrectedAmount IS NOT NULL;

                INSERT INTO claims.ClaimReviewDifferences
                    (Id, Value, Reason, ReviewId, PharmacyId, CreatedAt, UpdatedAt, IsDeleted)
                SELECT NEWID(), r.DifferenceAmount,
                       CASE WHEN r.DiscrepancyType IN
                           ('ContractualDeduction', 'DeferredToNextMonth', 'AccountingDeficit', 'Other')
                           THEN r.DiscrepancyType ELSE 'Other' END,
                       r.Id, c.PharmacyId,
                       SYSUTCDATETIME(), NULL, 0
                FROM claims.ClaimReviews r
                INNER JOIN claims.Claims c ON c.Id = r.ClaimId
                WHERE r.DifferenceAmount > 0;
                """);

            migrationBuilder.DropColumn(
                name: "DiscrepancyType",
                schema: "claims",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "DiscrepancyType",
                schema: "claims",
                table: "ClaimReviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimReviewDifferences",
                schema: "claims");

            migrationBuilder.DropColumn(
                name: "DifferenceAmount",
                schema: "claims",
                table: "ClaimReviews");

            migrationBuilder.DropColumn(
                name: "DifferenceType",
                schema: "claims",
                table: "ClaimReviews");

            migrationBuilder.AddColumn<string>(
                name: "DiscrepancyType",
                schema: "claims",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscrepancyType",
                schema: "claims",
                table: "ClaimReviews",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
