using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;
using System.Data;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Repositories
{
    public class ChequeRepository : IChequeRepository
    {
        private readonly ClaimsDbContext _context;
        public ChequeRepository(ClaimsDbContext context) => _context = context;

        public Task<Cheque?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Cheques.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public Task<bool> ExistsForClaimAsync(Guid claimId, CancellationToken cancellationToken = default)
            => _context.Cheques.AnyAsync(c => c.ClaimId == claimId, cancellationToken);

        public Task<bool> ExistsByChequeNumberAsync(Guid pharmacyId, string chequeNumber, Guid? excludedChequeId = null, CancellationToken cancellationToken = default)
            => _context.Cheques.AnyAsync(
                c => c.PharmacyId == pharmacyId && c.ChequeNumber == chequeNumber &&
                     (!excludedChequeId.HasValue || c.Id != excludedChequeId.Value),
                cancellationToken);

        public Task<List<Cheque>> GetByPharmacyAsync(Guid pharmacyId, string? companyName, int? claimMonth, int? claimYear, ChequeStatus? status, CancellationToken cancellationToken = default)
        {
            var query = _context.Cheques.Where(c => c.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(c => c.CompanyName == companyName);

            if (claimMonth.HasValue)
                query = query.Where(c => c.ClaimMonth == claimMonth.Value);

            if (claimYear.HasValue)
                query = query.Where(c => c.ClaimYear == claimYear.Value);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            return query.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalCollectedAsync(Guid pharmacyId, string? companyName, CancellationToken cancellationToken = default)
        {
            var query = _context.Cheques.Where(c => c.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(c => c.CompanyName == companyName);

            return await query.SumAsync(c => (decimal?)(
                c.Status == ChequeStatus.PaidInFull
                    ? c.Amount
                    : c.Status == ChequeStatus.PartiallyPaid
                        ? c.Amount - (c.RemainingAmount ?? 0m)
                        : 0m), cancellationToken) ?? 0m;
        }

        public Task<List<UnpaidChequeBalanceDto>> GetUnpaidOrPartiallyPaidChequesAsync(
            Guid pharmacyId, string? companyName, CancellationToken cancellationToken = default)
        {
            var query = _context.Cheques.Where(c =>
                c.PharmacyId == pharmacyId &&
                (c.Status == ChequeStatus.Pending ||
                 c.Status == ChequeStatus.Deferred ||
                 c.Status == ChequeStatus.Overdue ||
                 c.Status == ChequeStatus.PartiallyPaid));

            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(c => c.CompanyName == companyName);

            return query.Select(c => new UnpaidChequeBalanceDto
            {
                EndDate = c.EndDate,
                Amount = c.Amount,
                RemainingAmount = c.RemainingAmount,
                Status = c.Status
            }).ToListAsync(cancellationToken);
        }

        public Task<List<Cheque>> GetUpcomingDueAsync(Guid pharmacyId, int days, CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(days);

            return _context.Cheques
                .Where(c => c.PharmacyId == pharmacyId &&
                            c.Status == ChequeStatus.Pending &&
                            c.EndDate >= today && c.EndDate <= endDate)
                .OrderBy(c => c.EndDate)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Cheque>> GetOverdueCandidatesAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
        {
            return _context.Cheques
                .Where(c => c.Status == ChequeStatus.Pending && c.EndDate < asOfDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Cheque entity, CancellationToken cancellationToken = default)
            => await _context.Cheques.AddAsync(entity, cancellationToken);

        public async Task AddRangeAsync(List<Cheque> cheques, CancellationToken cancellationToken = default)
            => await _context.Cheques.AddRangeAsync(cheques, cancellationToken);

        public async Task<bool> TryAddRangeForClaimAsync(
            Guid claimId, List<Cheque> cheques, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            if (await _context.Cheques.AnyAsync(c => c.ClaimId == claimId, cancellationToken))
            {
                await transaction.RollbackAsync(cancellationToken);
                return false;
            }

            await _context.Cheques.AddRangeAsync(cheques, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        public void Update(Cheque entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Cheques.Update(entity);
        }

        public void UpdateRange(List<Cheque> cheques)
        {
            foreach (var cheque in cheques)
                cheque.UpdatedAt = DateTime.UtcNow;

            _context.Cheques.UpdateRange(cheques);
        }

        public void Remove(Cheque entity) => _context.Cheques.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}
