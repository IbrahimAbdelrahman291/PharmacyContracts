using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities
{
    public class Cheque : BaseEntity
    {
        public Guid ClaimId { get; set; }
        public Guid PharmacyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }

        public int ClaimMonth { get; set; }  
        public int ClaimYear { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountBeforeDiscount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal DiscountDifference { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AdministrativeExpensesPercentage { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PaymentDifference { get; set; }
        public PaymentDifferenceType PaymentDifferenceType { get; set; } = PaymentDifferenceType.Equal;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SettlementDays { get; set; }

        public ChequeStatus Status { get; set; } = ChequeStatus.Pending;
        public decimal? RemainingAmount { get; set; }
    }
}
