using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimReviewService
    {
        Task<Result<ClaimReviewResponseDto>> CreateAsync(Guid pharmacyId, Guid claimId, Guid reviewerUserId, CreateClaimReviewRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<ClaimReviewResponseDto>> UpdateAsync(Guid pharmacyId, Guid claimId, UpdateClaimReviewRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<ClaimReviewResponseDto>> GetByClaimIdAsync(Guid pharmacyId, Guid claimId, CancellationToken cancellationToken = default);
    }
}
