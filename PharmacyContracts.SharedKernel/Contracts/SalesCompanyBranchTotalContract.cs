

namespace PharmacyContracts.SharedKernel.Contracts
{
    public class SalesCompanyBranchTotalContract
    {
        public string CompanyName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalRemainingAmount { get; set; }
    }
}
