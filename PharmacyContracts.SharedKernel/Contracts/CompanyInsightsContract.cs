
namespace PharmacyContracts.SharedKernel.Contracts
{
    public class CompanyInsightsContract
    {
        public int PrescriptionsCount { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public decimal TotalLocalItemsAmount { get; set; }
        public decimal TotalImportedItemsAmount { get; set; }
    }
}
