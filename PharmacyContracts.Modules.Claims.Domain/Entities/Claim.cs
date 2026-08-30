using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities
{
    public class Claim : BaseEntity
    {
        public Guid PharmacyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public int Month { get; set; }
        public int Year { get; set; }

        public decimal ClaimAmountAfterDiscount { get; set; }
        public decimal? CorrectedAmount { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    }
}
