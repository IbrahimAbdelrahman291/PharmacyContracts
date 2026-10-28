using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities;

public class DigitalReceipt : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid PharmacyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DigitalReceiptStatus Status { get; set; } = DigitalReceiptStatus.PendingUpload;
    public string? FilePath { get; set; }
    public string? OriginalFileName { get; set; }
    public DateTime? UploadedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
