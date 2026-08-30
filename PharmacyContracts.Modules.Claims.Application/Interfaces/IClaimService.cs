using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;


namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimService
    {
        Task<Result<List<ClaimResponseDto>>> GetByPeriodAsync(Guid pharmacyId, int? month, int? year, CancellationToken cancellationToken = default);
    }
}
