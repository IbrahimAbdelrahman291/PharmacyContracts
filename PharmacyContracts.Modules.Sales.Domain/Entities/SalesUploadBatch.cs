using PharmacyContracts.Modules.Sales.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Sales.Domain.Entities
{
    public class SalesUploadBatch : BaseEntity
    {
        public Guid PharmacyId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public string LocalFilePath { get; set; } = string.Empty;

        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int FailedRows { get; set; }

        public BatchStatus Status { get; set; } = BatchStatus.Pending;
        public string? ErrorLog { get; set; }

        public DateTime? CompletedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public int RecoveryAttempts { get; set; }
        public DateTime? LastProcessingAttemptAt { get; set; }
    }
}
