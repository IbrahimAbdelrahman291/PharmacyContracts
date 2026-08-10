using PharmacyContracts.SharedKernel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Companies.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal LocalDiscountPercentage { get; set; }
        public decimal ImportedDiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AdministrativeExpensesPercentage { get; set; }
        public int ChequeSettlementPeriodInDays { get; set; }

        // مفيش navigation property لـ Auth module - Companies module مستقل تمامًا
        public Guid PharmacyId { get; set; }
    }
}
