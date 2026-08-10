using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Application.DTOs
{
    public class UpdateUserStatusRequestDto
    {
        public bool IsActive { get; set; }
    }
}
