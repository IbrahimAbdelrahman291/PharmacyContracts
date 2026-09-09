using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Enums;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IChequeRepository : IGenericRepository<Cheque>
    {
        Task<bool> ExistsForClaimAsync(Guid claimId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByChequeNumberAsync(Guid pharmacyId, string chequeNumber, CancellationToken cancellationToken = default);
        Task<List<Cheque>> GetByPharmacyAsync(Guid pharmacyId, string? companyName, int? month, int? year, ChequeStatus? status, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalCollectedAsync(Guid pharmacyId, string? companyName, CancellationToken cancellationToken = default);
        Task<List<UnpaidChequeBalanceDto>> GetUnpaidOrPartiallyPaidChequesAsync(Guid pharmacyId, string? companyName, CancellationToken cancellationToken = default);
        Task<List<Cheque>> GetUpcomingDueAsync(Guid pharmacyId, int days, CancellationToken cancellationToken = default);
        Task<List<Cheque>> GetOverdueCandidatesAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
        Task AddRangeAsync(List<Cheque> cheques, CancellationToken cancellationToken = default);
        void UpdateRange(List<Cheque> cheques);
    }
}
