using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Repositories
{
    public class ClaimReviewRepository : IClaimReviewRepository
    {
        private readonly ClaimsDbContext _context;
        public ClaimReviewRepository(ClaimsDbContext context) => _context = context;

        public Task<ClaimReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.ClaimReviews.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        public Task<ClaimReview?> GetByClaimIdAsync(Guid claimId, CancellationToken cancellationToken = default)
            => _context.ClaimReviews.FirstOrDefaultAsync(r => r.ClaimId == claimId, cancellationToken);

        public async Task AddAsync(ClaimReview entity, CancellationToken cancellationToken = default)
            => await _context.ClaimReviews.AddAsync(entity, cancellationToken);

        public void Update(ClaimReview entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.ClaimReviews.Update(entity);
        }

        public void Remove(ClaimReview entity) => _context.ClaimReviews.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}