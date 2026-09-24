using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities
{
    public class ClaimReviewDifference : BaseEntity
    {
        public decimal Value { get; set; }
        public DifferenceReason Reason { get; set; }
        public string? Notes { get; set; }
        public Guid ReviewId { get; set; }
        public Guid PharmacyId { get; set; }
        public ClaimReview Review { get; set; } = null!;
    }
}
