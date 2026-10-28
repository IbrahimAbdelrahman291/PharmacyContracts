namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimReviewDifferenceRequestDto
    {
        public decimal Value { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? EmployeeName { get; set; }
        public string? PharmacyName { get; set; }
    }
}
