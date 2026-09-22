

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeResponseDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountBeforeDiscount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal DiscountDifference { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AdministrativeExpensesPercentage { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PaymentDifference { get; set; }
        public string PaymentDifferenceType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? RemainingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
