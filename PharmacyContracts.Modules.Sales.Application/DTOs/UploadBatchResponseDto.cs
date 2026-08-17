using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Application.DTOs
{
    public class UploadBatchResponseDto
    {
        public Guid BatchId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
