using PharmacyContracts.Modules.Claims.Domain.Enums;

namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class DigitalReceiptResponseDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PharmacyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DigitalReceiptStatus Status { get; set; }
    public string? OriginalFileName { get; set; }
    public DateTime? UploadedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
