using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.Modules.Companies.Domain.Entities;
using PharmacyContracts.Modules.Companies.Infrastructure.Data;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly CompaniesDbContext _context;
    public CompanyRepository(CompaniesDbContext context) => _context = context;

    public Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Companies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Company?> GetByIdForPharmacyAsync(Guid id, Guid pharmacyId, CancellationToken cancellationToken = default)
        => _context.Companies.FirstOrDefaultAsync(c => c.Id == id && c.PharmacyId == pharmacyId, cancellationToken);

    public async Task<PagedResult<Company>> GetPagedByPharmacyAsync(Guid pharmacyId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = _context.Companies
            .Where(c => c.PharmacyId == pharmacyId)
            .OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Company>
        {
            Items = items,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public Task<bool> ExistsByNameAsync(Guid pharmacyId, string name, CancellationToken cancellationToken = default)
        => _context.Companies.AnyAsync(c => c.PharmacyId == pharmacyId && c.Name == name, cancellationToken);

    public async Task AddAsync(Company entity, CancellationToken cancellationToken = default)
        => await _context.Companies.AddAsync(entity, cancellationToken);

    public void Update(Company entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Companies.Update(entity);
    }

    public void Remove(Company entity) => _context.Companies.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}