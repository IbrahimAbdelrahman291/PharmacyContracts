using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Repositories;

public class CosmeticsLedgerRepository : ICosmeticsLedgerRepository
{
    private readonly ClaimsDbContext _context;
    public CosmeticsLedgerRepository(ClaimsDbContext context) => _context = context;

    public Task<List<CosmeticsCompany>> GetCompaniesAsync(Guid pharmacyId, CancellationToken cancellationToken) =>
        _context.CosmeticsCompanies.Where(x => x.PharmacyId == pharmacyId).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public Task<CosmeticsCompany?> GetCompanyAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken) =>
        _context.CosmeticsCompanies.FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.Id == id, cancellationToken);
    public Task<CosmeticsCompany?> FindCompanyByNameAsync(Guid pharmacyId, string name, Guid? excludeId, CancellationToken cancellationToken) =>
        _context.CosmeticsCompanies.FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.Name == name && (!excludeId.HasValue || x.Id != excludeId), cancellationToken);
    public Task<List<CosmeticsName>> GetNamesAsync(Guid pharmacyId, CancellationToken cancellationToken) =>
        _context.CosmeticsNames.Where(x => x.PharmacyId == pharmacyId).OrderBy(x => x.Name).ToListAsync(cancellationToken);
    public Task<CosmeticsName?> GetNameAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken) =>
        _context.CosmeticsNames.FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.Id == id, cancellationToken);
    public Task<CosmeticsName?> FindNameByNameAsync(Guid pharmacyId, string name, Guid? excludeId, CancellationToken cancellationToken) =>
        _context.CosmeticsNames.FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.Name == name && (!excludeId.HasValue || x.Id != excludeId), cancellationToken);
    public Task<CosmeticsTransaction?> GetTransactionAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken) =>
        _context.CosmeticsTransactions.Include(x => x.Company).Include(x => x.Name)
            .FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.Id == id, cancellationToken);

    public Task<List<CosmeticsTransaction>> GetTransactionsAsync(Guid pharmacyId, Guid? companyId, Guid? nameId, string? name, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var query = _context.CosmeticsTransactions.Include(x => x.Company).Include(x => x.Name)
            .Where(x => x.PharmacyId == pharmacyId);
        if (companyId.HasValue) query = query.Where(x => x.CompanyId == companyId.Value);
        if (nameId.HasValue) query = query.Where(x => x.NameId == nameId.Value);
        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalized = name.Trim();
            query = query.Where(x => x.Name.Name == normalized);
        }
        if (from.HasValue) query = query.Where(x => x.Date >= from.Value);
        if (to.HasValue)
        {
            var exclusiveTo = to.Value.Date.AddDays(1);
            query = query.Where(x => x.Date < exclusiveTo);
        }
        return query.OrderBy(x => x.Date).ThenBy(x => x.CreatedAt).ThenBy(x => x.Id).ToListAsync(cancellationToken);
    }

    public Task<bool> CompanyHasTransactionsAsync(Guid pharmacyId, Guid companyId, CancellationToken cancellationToken) =>
        _context.CosmeticsTransactions.AnyAsync(x => x.PharmacyId == pharmacyId && x.CompanyId == companyId, cancellationToken);
    public Task<bool> NameHasTransactionsAsync(Guid pharmacyId, Guid nameId, CancellationToken cancellationToken) =>
        _context.CosmeticsTransactions.AnyAsync(x => x.PharmacyId == pharmacyId && x.NameId == nameId, cancellationToken);
    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class => await _context.Set<T>().AddAsync(entity, cancellationToken);
    public void Remove<T>(T entity) where T : class
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task RecalculateBalancesAsync(Guid pharmacyId, IEnumerable<Guid> companyIds, CancellationToken cancellationToken)
    {
        foreach (var companyId in companyIds.Distinct())
        {
            var transactions = await _context.CosmeticsTransactions
                .Where(x => x.PharmacyId == pharmacyId && x.CompanyId == companyId)
                .OrderBy(x => x.Date).ThenBy(x => x.CreatedAt).ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
            decimal balance = 0m;
            foreach (var transaction in transactions)
            {
                balance += transaction.IsNegative ? -transaction.Amount : transaction.Amount;
                transaction.Balance = balance;
            }
        }
    }

    public async Task SaveAndRecalculateBalancesAsync(Guid pharmacyId, IEnumerable<Guid> companyIds, CancellationToken cancellationToken)
    {
        await using var databaseTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await RecalculateBalancesAsync(pharmacyId, companyIds, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await databaseTransaction.CommitAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}
