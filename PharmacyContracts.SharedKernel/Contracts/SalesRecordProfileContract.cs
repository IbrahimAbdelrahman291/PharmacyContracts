using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.SharedKernel.Contracts
{
    public class SalesRecordProfileContract
    {
        public string Status { get; set; } = string.Empty;
        public string CustomerCompanyName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal ImportedItemsTotal { get; set; }
        public decimal LocalItemsTotal { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal DiscountOnTotal { get; set; }
        public decimal DiscountOnItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal RemainingAmount { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
}
