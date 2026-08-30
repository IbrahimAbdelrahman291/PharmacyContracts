
namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeCreationPreparationDto
    {
        public Guid ClaimId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public decimal Amount { get; set; }           // Claim.CorrectedAmount
        public int SettlementDays { get; set; }       // من Companies module، دلوقتي بالظبط
        public List<string> Departments { get; set; } = new();
    }
}
