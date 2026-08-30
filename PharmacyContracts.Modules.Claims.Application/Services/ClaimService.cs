using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.Mappings;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        public ClaimService(IClaimRepository claimRepository) => _claimRepository = claimRepository;

        public async Task<Result<List<ClaimResponseDto>>> GetByPeriodAsync(Guid pharmacyId, int? month, int? year, CancellationToken cancellationToken = default)
        {
            var claims = await _claimRepository.GetByPeriodAsync(pharmacyId, month, year, cancellationToken);
            return Result<List<ClaimResponseDto>>.Success(claims.Select(c => c.ToResponseDto()).ToList());
        }
    }
}