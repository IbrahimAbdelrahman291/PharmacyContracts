using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IChequeRepository : IGenericRepository<Cheque>
    {
        Task<bool> ExistsForClaimAsync(Guid claimId, CancellationToken cancellationToken = default);
        Task<List<Cheque>> GetByPharmacyAsync(Guid pharmacyId, string? companyName, int? month, int? year, CancellationToken cancellationToken = default);
        Task<List<Cheque>> GetOverdueCandidatesAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
        Task AddRangeAsync(List<Cheque> cheques, CancellationToken cancellationToken = default);
        void UpdateRange(List<Cheque> cheques);
    }
}
