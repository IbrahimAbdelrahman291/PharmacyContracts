using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.CreateTable(
                name: "SalesRecords",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerCompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImportedItemsTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocalItemsTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountOnTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountOnItems = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesUploadBatches",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LocalFilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    ProcessedRows = table.Column<int>(type: "int", nullable: false),
                    FailedRows = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ErrorLog = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecoveryAttempts = table.Column<int>(type: "int", nullable: false),
                    LastProcessingAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesUploadBatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesRecords_PharmacyId_CustomerCompanyName_SaleDate",
                schema: "sales",
                table: "SalesRecords",
                columns: new[] { "PharmacyId", "CustomerCompanyName", "SaleDate" })
                .Annotation("SqlServer:Include", new[] { "RemainingAmount" });

            migrationBuilder.CreateIndex(
                name: "IX_SalesRecords_UploadBatchId",
                schema: "sales",
                table: "SalesRecords",
                column: "UploadBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesUploadBatches_PharmacyId_FileHash",
                schema: "sales",
                table: "SalesUploadBatches",
                columns: new[] { "PharmacyId", "FileHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesUploadBatches_Status",
                schema: "sales",
                table: "SalesUploadBatches",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesRecords",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "SalesUploadBatches",
                schema: "sales");
        }
    }
}
