
namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeCreationPreparationDto
    {
        public Guid ClaimId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal AmountBeforeDiscount { get; set; }
        public decimal CorrectAmount { get; set; }
        public decimal AmountDifference { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AdministrativeExpensesPercentage { get; set; }
        public decimal FinalAmount { get; set; }
        public int SettlementDays { get; set; }       // من Companies module، دلوقتي بالظبط
        public List<string> Departments { get; set; } = new();
    }
}
