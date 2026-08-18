using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class CompanyPivotRowDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public Dictionary<string, decimal> AmountsByBranch { get; set; } = new();
        public decimal Total { get; set; }
    }
}
