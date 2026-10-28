using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities;

public class CosmeticsTransaction : BaseEntity
{
    public Guid PharmacyId { get; set; }
    public Guid CompanyId { get; set; }
    public CosmeticsCompany Company { get; set; } = null!;
    public Guid NameId { get; set; }
    public CosmeticsName Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public decimal Balance { get; set; }
    public bool IsNegative { get; set; }
}
