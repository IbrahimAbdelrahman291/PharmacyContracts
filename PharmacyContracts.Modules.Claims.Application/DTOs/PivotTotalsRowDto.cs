using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class PivotTotalsRowDto
    {
        public Dictionary<string, decimal> AmountsByBranch { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }
}
