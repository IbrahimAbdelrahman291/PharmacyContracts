

namespace PharmacyContracts.Modules.Sales.Application.DTOs
{
    public class ParsedSalesRowDto
    {
        public int RowNumber { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string CustomerCompanyName { get; set; } = string.Empty;
        public string RawSaleDate { get; set; } = string.Empty;
        public string RawImportedItemsTotal { get; set; } = string.Empty;
        public string RawLocalItemsTotal { get; set; } = string.Empty;
        public string RawGrossTotal { get; set; } = string.Empty;
        public string RawDiscountOnTotal { get; set; } = string.Empty;
        public string RawDiscountOnItems { get; set; } = string.Empty;
        public string RawSubTotal { get; set; } = string.Empty;
        public string RawRemainingAmount { get; set; } = string.Empty;
        public string RawStatus { get; set; } = string.Empty;
    }
}
