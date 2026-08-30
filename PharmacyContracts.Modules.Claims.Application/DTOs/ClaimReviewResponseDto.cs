

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid ClaimId { get; set; }
        public Guid ReviewedByUserId { get; set; }
        public bool IsAccurate { get; set; }
        public decimal? CorrectedAmount { get; set; }
        public string DiscrepancyType { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool WasEditedByPharmacy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastEditedAt { get; set; }
    }
}
