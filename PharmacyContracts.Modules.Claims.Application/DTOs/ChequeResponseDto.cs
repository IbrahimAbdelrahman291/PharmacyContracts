

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeResponseDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public decimal AmountBeforeDiscount { get; set; }
        public decimal CorrectAmount { get; set; }
        public decimal AmountDifference { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AdministrativeExpensesPercentage { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal? ActualAmount { get; set; }
        public decimal? PaymentDifference { get; set; }
        public string? PaymentDifferenceType { get; set; }
        public DateTime? ChequeDate { get; set; }
        public int ClaimMonth { get; set; }
        public int ClaimYear { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SettlementDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? RemainingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
