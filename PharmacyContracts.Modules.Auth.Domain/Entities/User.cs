using PharmacyContracts.Modules.Auth.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Auth.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        // بيتملى لو Role = Pharmacy بس
        public string? PharmacyName { get; set; }
    }
}
