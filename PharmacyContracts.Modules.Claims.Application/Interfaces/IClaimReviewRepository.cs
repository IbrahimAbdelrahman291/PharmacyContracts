using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimReviewRepository : IGenericRepository<ClaimReview>
    {
        Task<ClaimReview?> GetByClaimIdAsync(Guid claimId, CancellationToken cancellationToken = default);
    }
}
