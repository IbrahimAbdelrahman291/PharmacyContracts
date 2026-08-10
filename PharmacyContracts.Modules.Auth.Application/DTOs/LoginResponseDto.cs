using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? PharmacyName { get; set; }
    }
}
