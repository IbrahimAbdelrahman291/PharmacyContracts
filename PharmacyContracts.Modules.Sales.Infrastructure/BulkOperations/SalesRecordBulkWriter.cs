// BulkOperations/SalesRecordBulkWriter.cs
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Domain.Entities;
using PharmacyContracts.Modules.Sales.Infrastructure.Data;

namespace PharmacyContracts.Modules.Sales.Infrastructure.BulkOperations;

public class SalesRecordBulkWriter : ISalesRecordBulkWriter
{
    private readonly SalesDbContext _context;
    private const int BulkCopyBatchSize = 5000;

    public SalesRecordBulkWriter(SalesDbContext context) => _context = context;

    public async Task<int> BulkInsertAsync(List<SalesRecord> records, Guid batchId,
    Action<int>? onProgressBatchCompleted = null, CancellationToken cancellationToken = default)
    {
        var connection = (SqlConnection)_context.Database.GetDbConnection();
        var connectionWasClosed = connection.State != ConnectionState.Open;

        if (connectionWasClosed)
            await connection.OpenAsync(cancellationToken);

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
            {
                bulkCopy.DestinationTableName = "sales.SalesRecordsStaging";
                bulkCopy.BatchSize = BulkCopyBatchSize;
                bulkCopy.NotifyAfter = BulkCopyBatchSize;
                bulkCopy.SqlRowsCopied += (sender, e) => onProgressBatchCompleted?.Invoke((int)e.RowsCopied);

                bulkCopy.ColumnMappings.Add("Id", "Id");
                bulkCopy.ColumnMappings.Add("PharmacyId", "PharmacyId");
                bulkCopy.ColumnMappings.Add("UploadBatchId", "UploadBatchId");
                bulkCopy.ColumnMappings.Add("BranchName", "BranchName");
                bulkCopy.ColumnMappings.Add("CustomerCompanyName", "CustomerCompanyName");
                bulkCopy.ColumnMappings.Add("SaleDate", "SaleDate");
                bulkCopy.ColumnMappings.Add("ImportedItemsTotal", "ImportedItemsTotal");
                bulkCopy.ColumnMappings.Add("LocalItemsTotal", "LocalItemsTotal");
                bulkCopy.ColumnMappings.Add("GrossTotal", "GrossTotal");
                bulkCopy.ColumnMappings.Add("DiscountOnTotal", "DiscountOnTotal");
                bulkCopy.ColumnMappings.Add("DiscountOnItems", "DiscountOnItems");
                bulkCopy.ColumnMappings.Add("SubTotal", "SubTotal");
                bulkCopy.ColumnMappings.Add("RemainingAmount", "RemainingAmount");
                bulkCopy.ColumnMappings.Add("Status", "Status");
                bulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                bulkCopy.ColumnMappings.Add("IsDeleted", "IsDeleted");

                var dataTable = BuildDataTable(records, batchId);
                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            }

            // خطوة منفصلة: النقل الفعلي، ونمسك عدد الصفوف المنقولة فقط
            const string insertSql = @"
            INSERT INTO sales.SalesRecords
                (Id, PharmacyId, UploadBatchId, BranchName, CustomerCompanyName, SaleDate,
                 ImportedItemsTotal, LocalItemsTotal, GrossTotal, DiscountOnTotal, DiscountOnItems,
                 SubTotal, RemainingAmount, Status, CreatedAt, IsDeleted)
            SELECT
                Id, PharmacyId, UploadBatchId, BranchName, CustomerCompanyName, SaleDate,
                ImportedItemsTotal, LocalItemsTotal, GrossTotal, DiscountOnTotal, DiscountOnItems,
                SubTotal, RemainingAmount, Status, CreatedAt, IsDeleted
            FROM sales.SalesRecordsStaging
            WHERE UploadBatchId = @BatchId;";

            int movedRows;
            await using (var insertCommand = new SqlCommand(insertSql, connection, (SqlTransaction)transaction))
            {
                insertCommand.Parameters.AddWithValue("@BatchId", batchId);
                movedRows = await insertCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            // خطوة منفصلة تانية: التنظيف، مش بنحسب عدد صفوفها في الـ result
            const string deleteSql = "DELETE FROM sales.SalesRecordsStaging WHERE UploadBatchId = @BatchId;";
            await using (var deleteCommand = new SqlCommand(deleteSql, connection, (SqlTransaction)transaction))
            {
                deleteCommand.Parameters.AddWithValue("@BatchId", batchId);
                await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return movedRows;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (connectionWasClosed)
                await connection.CloseAsync();
        }
    }

    private static DataTable BuildDataTable(List<SalesRecord> records, Guid batchId)
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(Guid));
        table.Columns.Add("PharmacyId", typeof(Guid));
        table.Columns.Add("UploadBatchId", typeof(Guid));
        table.Columns.Add("BranchName", typeof(string));
        table.Columns.Add("CustomerCompanyName", typeof(string));
        table.Columns.Add("SaleDate", typeof(DateTime));
        table.Columns.Add("ImportedItemsTotal", typeof(decimal));
        table.Columns.Add("LocalItemsTotal", typeof(decimal));
        table.Columns.Add("GrossTotal", typeof(decimal));
        table.Columns.Add("DiscountOnTotal", typeof(decimal));
        table.Columns.Add("DiscountOnItems", typeof(decimal));
        table.Columns.Add("SubTotal", typeof(decimal));
        table.Columns.Add("RemainingAmount", typeof(decimal));
        table.Columns.Add("Status", typeof(string));
        table.Columns.Add("CreatedAt", typeof(DateTime));
        table.Columns.Add("IsDeleted", typeof(bool));

        foreach (var record in records)
        {
            table.Rows.Add(
                record.Id, record.PharmacyId, batchId, record.BranchName, record.CustomerCompanyName,
                record.SaleDate, record.ImportedItemsTotal, record.LocalItemsTotal, record.GrossTotal,
                record.DiscountOnTotal, record.DiscountOnItems, record.SubTotal, record.RemainingAmount,
                record.Status.ToString(), record.CreatedAt, false);
        }

        return table;
    }
}