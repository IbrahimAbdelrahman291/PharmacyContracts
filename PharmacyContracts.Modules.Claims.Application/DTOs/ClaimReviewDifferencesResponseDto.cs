namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimReviewDifferencesResponseDto
    {
        public Guid ClaimId { get; set; }
        public Guid ReviewId { get; set; }
        public decimal DifferenceAmount { get; set; }
        public string DifferenceType { get; set; } = string.Empty;
        public List<ClaimReviewDifferenceResponseDto> Differences { get; set; } = [];
    }
}
