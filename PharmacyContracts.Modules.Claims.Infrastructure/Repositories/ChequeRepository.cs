using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;

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

        public Task<List<Cheque>> GetByPharmacyAsync(Guid pharmacyId, string? companyName, int? month, int? year, CancellationToken cancellationToken = default)
        {
            var query = _context.Cheques.Where(c => c.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(c => c.CompanyName == companyName);

            if (month.HasValue)
                query = query.Where(c => c.StartDate.Month == month.Value);

            if (year.HasValue)
                query = query.Where(c => c.StartDate.Year == year.Value);

            return query.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);
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