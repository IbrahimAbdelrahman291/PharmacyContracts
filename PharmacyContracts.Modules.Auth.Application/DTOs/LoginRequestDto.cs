using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Application.DTOs
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
