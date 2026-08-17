// Data/StagingTableScripts.cs
namespace PharmacyContracts.Modules.Sales.Infrastructure.Data;

// SQL الخام لإنشاء جدول الـ staging - بيتنفذ من جوه الـ migration
public static class StagingTableScripts
{
    public const string CreateStagingTable = @"
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesRecordsStaging' AND schema_id = SCHEMA_ID('sales'))
        BEGIN
            CREATE TABLE sales.SalesRecordsStaging
            (
                Id UNIQUEIDENTIFIER NOT NULL,
                PharmacyId UNIQUEIDENTIFIER NOT NULL,
                UploadBatchId UNIQUEIDENTIFIER NOT NULL,
                BranchName NVARCHAR(200) NOT NULL,
                CustomerCompanyName NVARCHAR(200) NOT NULL,
                SaleDate DATETIME2 NOT NULL,
                ImportedItemsTotal DECIMAL(18,2) NOT NULL,
                LocalItemsTotal DECIMAL(18,2) NOT NULL,
                GrossTotal DECIMAL(18,2) NOT NULL,
                DiscountOnTotal DECIMAL(18,2) NOT NULL,
                DiscountOnItems DECIMAL(18,2) NOT NULL,
                SubTotal DECIMAL(18,2) NOT NULL,
                RemainingAmount DECIMAL(18,2) NOT NULL,
                Status NVARCHAR(20) NOT NULL,
                CreatedAt DATETIME2 NOT NULL,
                IsDeleted BIT NOT NULL DEFAULT 0
            );

            -- Index واحد بس على UploadBatchId عشان الـ move والـ cleanup يبقوا سريعين
            CREATE INDEX IX_SalesRecordsStaging_UploadBatchId ON sales.SalesRecordsStaging (UploadBatchId);
        END";

    public const string DropStagingTable = @"
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesRecordsStaging' AND schema_id = SCHEMA_ID('sales'))
        BEGIN
            DROP TABLE sales.SalesRecordsStaging;
        END";
}