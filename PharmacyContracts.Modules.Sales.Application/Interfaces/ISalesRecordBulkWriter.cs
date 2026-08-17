using PharmacyContracts.Modules.Sales.Domain.Entities;

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface ISalesRecordBulkWriter
    {
        // بينفذ: SqlBulkCopy للـ staging -> INSERT...SELECT للجدول الرئيسي -> تنضيف الـ staging
        // كل دا جوه transaction واحدة. بيرجع عدد الصفوف اللي اتنقلت فعليًا
        Task<int> BulkInsertAsync(List<SalesRecord> records, Guid batchId,
            Action<int>? onProgressBatchCompleted = null, CancellationToken cancellationToken = default);
    }
}
