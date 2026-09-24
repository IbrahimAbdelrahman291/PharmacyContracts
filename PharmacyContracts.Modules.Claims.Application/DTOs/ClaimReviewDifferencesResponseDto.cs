namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimReviewDifferencesResponseDto
    {
        public Guid ClaimId { get; set; }
        public Guid ReviewId { get; set; }
        public decimal AmountBeforeDiscount { get; set; }
        public decimal? CorrectedAmount { get; set; }
        public decimal AmountDifference { get; set; }
        public string AmountDifferenceType { get; set; } = string.Empty;
        public int PrescriptionsCount { get; set; }
        public int? CorrectedPrescriptionsCount { get; set; }
        public int PrescriptionsCountDifference { get; set; }
        public string PrescriptionsCountDifferenceType { get; set; } = string.Empty;
        public decimal DifferenceAmount { get; set; }
        public string DifferenceType { get; set; } = string.Empty;
        public List<ClaimReviewDifferenceResponseDto> Differences { get; set; } = [];
    }
}
