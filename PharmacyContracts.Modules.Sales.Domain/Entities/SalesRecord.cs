using PharmacyContracts.Modules.Sales.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Sales.Domain.Entities
{
    public class SalesRecord : BaseEntity
    {
        public Guid PharmacyId { get; set; }
        public Guid UploadBatchId { get; set; }

        public string BranchName { get; set; } = string.Empty;
        public string CustomerCompanyName { get; set; } = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal ImportedItemsTotal { get; set; }
        public decimal LocalItemsTotal { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal DiscountOnTotal { get; set; }
        public decimal DiscountOnItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal RemainingAmount { get; set; }

        public SalesRecordStatus Status { get; set; }
    }
}
