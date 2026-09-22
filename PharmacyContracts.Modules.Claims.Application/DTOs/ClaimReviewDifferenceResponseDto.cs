namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimReviewDifferenceResponseDto
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid ReviewId { get; set; }
        public Guid PharmacyId { get; set; }
    }
}
