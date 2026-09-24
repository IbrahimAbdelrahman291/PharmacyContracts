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
        public int? CorrectedPrescriptionsCount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public DifferenceType DifferenceType { get; set; } = DifferenceType.NoDifference;
        public ICollection<ClaimReviewDifference> Differences { get; set; } = [];

        public bool WasEditedByPharmacy { get; set; }
        public DateTime? LastEditedAt { get; set; }
    }
}
