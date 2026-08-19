
namespace PharmacyContracts.SharedKernel.Contracts
{
    public class CompanyInsightsContract
    {
        public int PrescriptionsCount { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public decimal TotalLocalDiscount { get; set; }
        public decimal TotalImportedDiscount { get; set; }
    }
}
