using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCosmeticsLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CosmeticsCompanies",
                schema: "claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticsCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CosmeticsNames",
                schema: "claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticsNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CosmeticsTransactions",
                schema: "claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsNegative = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosmeticsTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CosmeticsTransactions_CosmeticsCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "claims",
                        principalTable: "CosmeticsCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CosmeticsTransactions_CosmeticsNames_NameId",
                        column: x => x.NameId,
                        principalSchema: "claims",
                        principalTable: "CosmeticsNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticsCompanies_PharmacyId_Name",
                schema: "claims",
                table: "CosmeticsCompanies",
                columns: new[] { "PharmacyId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticsNames_PharmacyId_Name",
                schema: "claims",
                table: "CosmeticsNames",
                columns: new[] { "PharmacyId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticsTransactions_CompanyId",
                schema: "claims",
                table: "CosmeticsTransactions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticsTransactions_NameId",
                schema: "claims",
                table: "CosmeticsTransactions",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticsTransactions_PharmacyId_CompanyId_Date",
                schema: "claims",
                table: "CosmeticsTransactions",
                columns: new[] { "PharmacyId", "CompanyId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CosmeticsTransactions_PharmacyId_NameId_Date",
                schema: "claims",
                table: "CosmeticsTransactions",
                columns: new[] { "PharmacyId", "NameId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CosmeticsTransactions",
                schema: "claims");

            migrationBuilder.DropTable(
                name: "CosmeticsCompanies",
                schema: "claims");

            migrationBuilder.DropTable(
                name: "CosmeticsNames",
                schema: "claims");
        }
    }
}
