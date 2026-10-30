using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimReviewRepository : IGenericRepository<ClaimReview>
    {
        Task<ClaimReview?> GetByClaimIdAsync(Guid claimId, CancellationToken cancellationToken = default);
        Task<PagedResult<ClaimReviewDifferenceReportDto>> GetDifferenceReportAsync(Guid pharmacyId, string? companyName, PaginationParams pagination, CancellationToken cancellationToken = default);
    }
}
