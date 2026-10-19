// Companies.Infrastructure/Queries/CompaniesQueryService.cs
using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Companies.Infrastructure.Data;
using PharmacyContracts.SharedKernel.Contracts;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Queries
{
    public class CompaniesQueryService : ICompaniesQueryService
    {
        private readonly CompaniesDbContext _context;
        public CompaniesQueryService(CompaniesDbContext context) => _context = context;

        public async Task<decimal> GetDiscountPercentageAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var normalizedName = companyName.Trim();
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.PharmacyId == pharmacyId && c.Name.Trim() == normalizedName, cancellationToken);

            return company?.Discount ?? 0;
        }

        // الميثود الناقصة - أضفناها هنا
        public async Task<int> GetChequeSettlementPeriodInDaysAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var normalizedName = companyName.Trim();
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.PharmacyId == pharmacyId && c.Name.Trim() == normalizedName, cancellationToken);

            return company?.ChequeSettlementPeriodInDays ?? 45; // قيمة افتراضية لو الشركة مش موجودة بنفس الاسم بالظبط
        }

        public async Task<Dictionary<string, decimal>> GetDiscountPercentagesAsync(
            Guid pharmacyId, IEnumerable<string> companyNames, CancellationToken cancellationToken = default)
        {
            var namesList = companyNames.Distinct().ToList();
            var normalizedNames = namesList.Select(name => name.Trim()).Distinct().ToList();

            if (namesList.Count == 0)
                return new Dictionary<string, decimal>();

            var companies = await _context.Companies
                .Where(c => c.PharmacyId == pharmacyId && normalizedNames.Contains(c.Name.Trim()))
                .Select(c => new { c.Name, c.Discount })
                .ToListAsync(cancellationToken);

            var discountsByNormalizedName = companies
                .GroupBy(c => c.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Discount, StringComparer.OrdinalIgnoreCase);
            var result = namesList.ToDictionary(
                name => name,
                name => discountsByNormalizedName.GetValueOrDefault(name.Trim()));

            return result;
        }

        public async Task<Dictionary<string, CompanyDiscountPercentagesContract>> GetItemDiscountPercentagesAsync(
            Guid pharmacyId, IEnumerable<string> companyNames, CancellationToken cancellationToken = default)
        {
            var namesList = companyNames.Distinct().ToList();
            var normalizedNames = namesList.Select(name => name.Trim()).Distinct().ToList();

            if (namesList.Count == 0)
                return new Dictionary<string, CompanyDiscountPercentagesContract>();

            var companies = await _context.Companies
                .Where(c => c.PharmacyId == pharmacyId && normalizedNames.Contains(c.Name.Trim()))
                .Select(c => new
                {
                    c.Name,
                    c.LocalDiscountPercentage,
                    c.ImportedDiscountPercentage
                })
                .ToListAsync(cancellationToken);

            var discountsByNormalizedName = companies
                .GroupBy(c => c.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                group => group.Key,
                group => new CompanyDiscountPercentagesContract
                {
                    LocalDiscountPercentage = group.First().LocalDiscountPercentage,
                    ImportedDiscountPercentage = group.First().ImportedDiscountPercentage
                },
                StringComparer.OrdinalIgnoreCase);

            var result = namesList.ToDictionary(
                name => name,
                name => discountsByNormalizedName.GetValueOrDefault(name.Trim())
                    ?? new CompanyDiscountPercentagesContract());

            return result;
        }
        public async Task<List<string>> GetDepartmentNamesAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var normalizedName = companyName.Trim();
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.PharmacyId == pharmacyId && c.Name.Trim() == normalizedName, cancellationToken);

            if (company is null)
                return new List<string>();

            return await _context.CompanyDepartments
                .Where(d => d.CompanyId == company.Id)
                .Select(d => d.Name)
                .OrderBy(n => n)
                .ToListAsync(cancellationToken);
        }

        public async Task<CompanyFinancialPercentagesContract?> GetFinancialPercentagesAsync(
            Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var normalizedName = companyName.Trim();
            return await _context.Companies
                .Where(c => c.PharmacyId == pharmacyId && c.Name.Trim() == normalizedName)
                .Select(c => new CompanyFinancialPercentagesContract
                {
                    TaxPercentage = c.TaxPercentage,
                    AdministrativeExpensesPercentage = c.AdministrativeExpensesPercentage
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<string>> GetMissingCompanyNamesAsync(
            Guid pharmacyId, IEnumerable<string> companyNames, CancellationToken cancellationToken = default)
        {
            var requestedNames = companyNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (requestedNames.Count == 0)
                return [];

            var registeredNames = await _context.Companies
                .Where(c => c.PharmacyId == pharmacyId && requestedNames.Contains(c.Name.Trim()))
                .Select(c => c.Name)
                .ToListAsync(cancellationToken);

            var registeredSet = registeredNames
                .Select(name => name.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return requestedNames.Where(name => !registeredSet.Contains(name)).ToList();
        }
    }
}
