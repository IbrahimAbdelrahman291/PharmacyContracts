

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimResponseDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal ClaimAmountAfterDiscount { get; set; }
        public decimal? CorrectedAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
