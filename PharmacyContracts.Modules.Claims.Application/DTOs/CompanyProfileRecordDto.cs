using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class CompanyProfileRecordDto
    {
        public DateTime SaleDate { get; set; }
        public decimal ImportedItemsTotal { get; set; }
        public decimal LocalItemsTotal { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal DiscountOnTotal { get; set; }
        public decimal DiscountOnItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
