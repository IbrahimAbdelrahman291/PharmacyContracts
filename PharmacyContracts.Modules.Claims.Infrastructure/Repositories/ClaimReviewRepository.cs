using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Repositories
{
    public class ClaimReviewRepository : IClaimReviewRepository
    {
        private readonly ClaimsDbContext _context;
        public ClaimReviewRepository(ClaimsDbContext context) => _context = context;

        public Task<ClaimReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.ClaimReviews.Include(r => r.Differences).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        public Task<ClaimReview?> GetByClaimIdAsync(Guid claimId, CancellationToken cancellationToken = default)
            => _context.ClaimReviews.Include(r => r.Differences).FirstOrDefaultAsync(r => r.ClaimId == claimId, cancellationToken);

        public async Task<PagedResult<ClaimReviewDifferenceReportDto>> GetContractualDeductionReportAsync(
            Guid pharmacyId, string? companyName, PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var query = from difference in _context.ClaimReviewDifferences
                        join review in _context.ClaimReviews on difference.ReviewId equals review.Id
                        join claim in _context.Claims on review.ClaimId equals claim.Id
                        where difference.PharmacyId == pharmacyId && difference.Reason == DifferenceReason.ContractualDeduction
                        select new { claim.CompanyName, difference.Value, difference.Notes };

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                var normalizedName = companyName.Trim();
                query = query.Where(x => x.CompanyName == normalizedName);
            }

            var pageNumber = Math.Max(1, pagination.PageNumber);
            var pageSize = Math.Clamp(pagination.PageSize, 1, 100);
            var totalCount = await query.Select(x => x.CompanyName).Distinct().CountAsync(cancellationToken);
            var companyNames = await query.Select(x => x.CompanyName).Distinct().OrderBy(x => x)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            var rows = await query.Where(x => companyNames.Contains(x.CompanyName)).ToListAsync(cancellationToken);

            return new PagedResult<ClaimReviewDifferenceReportDto>
            {
                Items = companyNames.Select(name => new ClaimReviewDifferenceReportDto
                {
                    CompanyName = name,
                    TotalAmount = rows.Where(x => x.CompanyName == name).Sum(x => x.Value),
                    Notes = rows.Where(x => x.CompanyName == name && !string.IsNullOrWhiteSpace(x.Notes))
                        .Select(x => x.Notes!).Distinct().ToList()
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

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
