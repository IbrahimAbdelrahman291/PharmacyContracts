using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities;

public class CosmeticsCompany : BaseEntity
{
    public Guid PharmacyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CosmeticsTransaction> Transactions { get; set; } = new List<CosmeticsTransaction>();
}
