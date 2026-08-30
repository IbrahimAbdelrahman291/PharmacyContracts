using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeAllocationDto
    {
        public string? DepartmentName { get; set; }   // null لو مفيش تقسيم إدارات
        public decimal Amount { get; set; }
    }
}
