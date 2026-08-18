using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Companies.Application.DTOs
{
    public class UpdateCompanyRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal LocalDiscountPercentage { get; set; }
        public decimal ImportedDiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AdministrativeExpensesPercentage { get; set; }
        public int ChequeSettlementPeriodInDays { get; set; }
        public decimal Discount { get; set; }   

    }
}
