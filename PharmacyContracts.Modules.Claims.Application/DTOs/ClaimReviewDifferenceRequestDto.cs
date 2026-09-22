namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimReviewDifferenceRequestDto
    {
        public decimal Value { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
