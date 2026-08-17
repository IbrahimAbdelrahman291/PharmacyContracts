using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Application.DTOs
{
    public class BatchStatusResponseDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int FailedRows { get; set; }
        public string? ErrorLog { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
