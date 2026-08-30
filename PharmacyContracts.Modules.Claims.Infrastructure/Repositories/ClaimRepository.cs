using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsDbContext _context;
        public ClaimRepository(ClaimsDbContext context) => _context = context;

        public Task<Claim?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Claims.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public Task<bool> ExistsForPeriodAsync(Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default)
            => _context.Claims.AnyAsync(c => c.PharmacyId == pharmacyId && c.Month == month && c.Year == year, cancellationToken);

        public Task<List<Claim>> GetByPeriodAsync(Guid pharmacyId, int? month, int? year, CancellationToken cancellationToken = default)
        {
            var query = _context.Claims.Where(c => c.PharmacyId == pharmacyId);

            if (month.HasValue)
                query = query.Where(c => c.Month == month.Value);

            if (year.HasValue)
                query = query.Where(c => c.Year == year.Value);

            return query.OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Claim entity, CancellationToken cancellationToken = default)
            => await _context.Claims.AddAsync(entity, cancellationToken);

        public async Task AddRangeAsync(List<Claim> claims, CancellationToken cancellationToken = default)
            => await _context.Claims.AddRangeAsync(claims, cancellationToken);

        public void Update(Claim entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Claims.Update(entity);
        }

        public void Remove(Claim entity) => _context.Claims.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}