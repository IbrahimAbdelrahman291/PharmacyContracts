using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Application.DTOs
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? PharmacyName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
