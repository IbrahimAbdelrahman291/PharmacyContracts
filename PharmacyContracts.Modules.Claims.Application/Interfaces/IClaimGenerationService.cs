using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimGenerationService
    {
        Task<Result<List<ClaimResponseDto>>> GenerateAsync(Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default);
    }
}
