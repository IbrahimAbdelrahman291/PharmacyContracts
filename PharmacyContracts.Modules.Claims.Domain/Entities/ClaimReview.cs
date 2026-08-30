using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities
{
    public class ClaimReview : BaseEntity
    {
        public Guid ClaimId { get; set; }
        public Guid ReviewedByUserId { get; set; }

        public bool IsAccurate { get; set; }
        public decimal? CorrectedAmount { get; set; }
        public DiscrepancyType DiscrepancyType { get; set; } = DiscrepancyType.None;
        public string? Notes { get; set; }

        public bool WasEditedByPharmacy { get; set; }
        public DateTime? LastEditedAt { get; set; }
    }
}
