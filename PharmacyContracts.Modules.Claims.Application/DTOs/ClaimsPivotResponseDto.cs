using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ClaimsPivotResponseDto
    {
        public List<string> Branches { get; set; } = new();
        public List<CompanyPivotRowDto> Rows { get; set; } = new();
    }
}
