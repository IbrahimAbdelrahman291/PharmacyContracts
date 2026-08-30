

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class CreateClaimReviewRequestDto
    {
        public bool IsAccurate { get; set; }
        public decimal? CorrectedAmount { get; set; }
        public string? DiscrepancyType { get; set; }
        public string? Notes { get; set; }
    }
}
