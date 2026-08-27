

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class CompanyInsightsResponseDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public int PrescriptionsCount { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public decimal TotalLocalItemsAmount { get; set; }
        public decimal TotalImportedItemsAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal AmountAfterDiscount { get; set; }
    }
}
