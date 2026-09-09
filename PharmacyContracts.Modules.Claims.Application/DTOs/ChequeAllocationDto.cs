using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeAllocationDto
    {
        public string? DepartmentName { get; set; }   // null لو مفيش تقسيم إدارات
        public decimal Amount { get; set; }
        public string ChequeNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
    }
}
