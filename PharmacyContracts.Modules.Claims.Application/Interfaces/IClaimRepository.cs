using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IClaimRepository : IGenericRepository<Claim>
    {
        Task<bool> ExistsForPeriodAsync(Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default);
        Task<List<Claim>> GetByPeriodAsync(Guid pharmacyId, int? month, int? year, CancellationToken cancellationToken = default);
        Task AddRangeAsync(List<Claim> claims, CancellationToken cancellationToken = default);
    }
}
