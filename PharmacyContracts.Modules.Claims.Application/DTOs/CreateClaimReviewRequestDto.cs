

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class CreateClaimReviewRequestDto
    {
        public bool IsAccurate { get; set; }
        public decimal? CorrectedAmount { get; set; }
        public int? CorrectedPrescriptionsCount { get; set; }
        public List<ClaimReviewDifferenceRequestDto> Differences { get; set; } = [];
    }
}
