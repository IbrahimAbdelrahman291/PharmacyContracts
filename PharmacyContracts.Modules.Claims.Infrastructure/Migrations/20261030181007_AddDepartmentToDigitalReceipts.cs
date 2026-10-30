using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyContracts.Modules.Claims.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentToDigitalReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DigitalReceipts_PharmacyId_CompanyId_Month_Year",
                schema: "claims",
                table: "DigitalReceipts");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "claims",
                table: "DigitalReceipts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                schema: "claims",
                table: "DigitalReceipts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DigitalReceipts_PharmacyId_CompanyId_DepartmentId_Month_Year",
                schema: "claims",
                table: "DigitalReceipts",
                columns: new[] { "PharmacyId", "CompanyId", "DepartmentId", "Month", "Year" },
                unique: true,
                filter: "[DepartmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalReceipts_PharmacyId_CompanyId_Month_Year",
                schema: "claims",
                table: "DigitalReceipts",
                columns: new[] { "PharmacyId", "CompanyId", "Month", "Year" },
                unique: true,
                filter: "[DepartmentId] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DigitalReceipts_PharmacyId_CompanyId_DepartmentId_Month_Year",
                schema: "claims",
                table: "DigitalReceipts");

            migrationBuilder.DropIndex(
                name: "IX_DigitalReceipts_PharmacyId_CompanyId_Month_Year",
                schema: "claims",
                table: "DigitalReceipts");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "claims",
                table: "DigitalReceipts");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                schema: "claims",
                table: "DigitalReceipts");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalReceipts_PharmacyId_CompanyId_Month_Year",
                schema: "claims",
                table: "DigitalReceipts",
                columns: new[] { "PharmacyId", "CompanyId", "Month", "Year" },
                unique: true);
        }
    }
}
