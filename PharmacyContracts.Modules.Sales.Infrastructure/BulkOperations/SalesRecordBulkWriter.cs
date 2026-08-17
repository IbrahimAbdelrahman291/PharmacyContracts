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

        // كل العملية (bulk copy للـ staging + move للجدول الرئيسي + تنضيف الـ staging)
        // جوه transaction واحدة - لو أي خطوة فشلت، كل حاجة بترجع زي ما كانت
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1) SqlBulkCopy للـ staging table
            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
            {
                bulkCopy.DestinationTableName = "sales.SalesRecordsStaging";
                bulkCopy.BatchSize = BulkCopyBatchSize;
                bulkCopy.NotifyAfter = BulkCopyBatchSize;
                bulkCopy.SqlRowsCopied += (sender, e) => onProgressBatchCompleted?.Invoke((int)e.RowsCopied);

                var dataTable = BuildDataTable(records, batchId);
                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            }

            // 2) نقل الصفوف من الـ staging للجدول الرئيسي، مفلترة بالـ BatchId فقط
            //    (عشان لو فيه batch تانية شغالة بالتوازي، منلمسش صفوفها)
            const string moveSql = @"
                INSERT INTO sales.SalesRecords
                    (Id, PharmacyId, UploadBatchId, BranchName, CustomerCompanyName, SaleDate,
                     ImportedItemsTotal, LocalItemsTotal, GrossTotal, DiscountOnTotal, DiscountOnItems,
                     SubTotal, RemainingAmount, Status, CreatedAt, IsDeleted)
                SELECT
                    Id, PharmacyId, UploadBatchId, BranchName, CustomerCompanyName, SaleDate,
                    ImportedItemsTotal, LocalItemsTotal, GrossTotal, DiscountOnTotal, DiscountOnItems,
                    SubTotal, RemainingAmount, Status, CreatedAt, IsDeleted
                FROM sales.SalesRecordsStaging
                WHERE UploadBatchId = @BatchId;

                DELETE FROM sales.SalesRecordsStaging WHERE UploadBatchId = @BatchId;";

            await using var moveCommand = new SqlCommand(moveSql, connection, (SqlTransaction)transaction);
            moveCommand.Parameters.AddWithValue("@BatchId", batchId);
            var movedRows = await moveCommand.ExecuteNonQueryAsync(cancellationToken);

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