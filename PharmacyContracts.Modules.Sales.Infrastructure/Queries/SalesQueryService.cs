using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Sales.Domain.Enums;
using PharmacyContracts.Modules.Sales.Infrastructure.Data;
using PharmacyContracts.SharedKernel.Contracts;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Queries;

public class SalesQueryService : ISalesQueryService
{
    private readonly SalesDbContext _context;
    public SalesQueryService(SalesDbContext context) => _context = context;

    public async Task<List<string>> GetDistinctBranchesAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        return await _context.SalesRecords
            .Where(r => r.PharmacyId == pharmacyId)
            .Select(r => r.BranchName)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SalesCompanyBranchTotalContract>> GetCompanyBranchTotalsAsync(
        Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await _context.SalesRecords
            .Where(r => r.PharmacyId == pharmacyId && r.SaleDate.Month == month && r.SaleDate.Year == year)
            .GroupBy(r => new { r.CustomerCompanyName, r.BranchName })
            .Select(g => new SalesCompanyBranchTotalContract
            {
                CompanyName = g.Key.CustomerCompanyName,
                BranchName = g.Key.BranchName,
                TotalRemainingAmount = g.Sum(r => r.RemainingAmount)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<SalesRecordProfileContract>> GetCompanyProfileAsync(
        Guid pharmacyId, string companyName, int? month, int? year,
        PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var query = _context.SalesRecords
            .Where(r => r.PharmacyId == pharmacyId && r.CustomerCompanyName == companyName);

        if (month.HasValue)
            query = query.Where(r => r.SaleDate.Month == month.Value);

        if (year.HasValue)
            query = query.Where(r => r.SaleDate.Year == year.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.SaleDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(r => new SalesRecordProfileContract
            {
                SaleDate = r.SaleDate,
                ImportedItemsTotal = r.ImportedItemsTotal,
                LocalItemsTotal = r.LocalItemsTotal,
                GrossTotal = r.GrossTotal,
                DiscountOnTotal = r.DiscountOnTotal,
                DiscountOnItems = r.DiscountOnItems,
                SubTotal = r.SubTotal,
                RemainingAmount = r.RemainingAmount
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SalesRecordProfileContract>
        {
            Items = items,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }
    public async Task<CompanyInsightsContract> GetCompanyInsightsAsync(
    Guid pharmacyId, string companyName, int month, int year, CancellationToken cancellationToken = default)
    {
        var query = _context.SalesRecords
            .Where(r => r.PharmacyId == pharmacyId
                && r.CustomerCompanyName == companyName
                && r.SaleDate.Month == month
                && r.SaleDate.Year == year);

        var salesCount = await query.CountAsync(r => r.Status == SalesRecordStatus.Sale, cancellationToken);
        var returnsCount = await query.CountAsync(r => r.Status == SalesRecordStatus.Return, cancellationToken);

        var totalRemaining = await query.SumAsync(r => r.RemainingAmount, cancellationToken);
        var totalLocalItems = await query.SumAsync(r => r.LocalItemsTotal, cancellationToken);
        var totalImportedItems = await query.SumAsync(r => r.ImportedItemsTotal, cancellationToken);

        return new CompanyInsightsContract
        {
            PrescriptionsCount = salesCount - returnsCount,
            TotalRemainingAmount = totalRemaining,
            TotalLocalItemsAmount = totalLocalItems,
            TotalImportedItemsAmount = totalImportedItems
        };
    }
}