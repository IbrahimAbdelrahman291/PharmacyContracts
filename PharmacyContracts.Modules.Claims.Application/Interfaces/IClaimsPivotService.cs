using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimsPivotService
    {
        Task<Result<ClaimsPivotResponseDto>> GetPivotAsync(Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default);
    }
}
