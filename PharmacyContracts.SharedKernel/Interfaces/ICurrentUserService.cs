using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.SharedKernel.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? EffectivePharmacyId { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
    }
}
