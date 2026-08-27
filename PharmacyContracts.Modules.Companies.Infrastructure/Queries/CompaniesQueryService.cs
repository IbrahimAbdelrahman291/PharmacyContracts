// Companies.Infrastructure/Queries/CompaniesQueryService.cs
using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Companies.Infrastructure.Data;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Queries
{
    public class CompaniesQueryService : ICompaniesQueryService
    {
        private readonly CompaniesDbContext _context;
        public CompaniesQueryService(CompaniesDbContext context) => _context = context;

        public async Task<decimal> GetDiscountPercentageAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.PharmacyId == pharmacyId && c.Name == companyName, cancellationToken);

            return company?.Discount ?? 0;
        }

        // الميثود الناقصة - أضفناها هنا
        public async Task<int> GetChequeSettlementPeriodInDaysAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.PharmacyId == pharmacyId && c.Name == companyName, cancellationToken);

            return company?.ChequeSettlementPeriodInDays ?? 45; // قيمة افتراضية لو الشركة مش موجودة بنفس الاسم بالظبط
        }

        public async Task<Dictionary<string, decimal>> GetDiscountPercentagesAsync(
            Guid pharmacyId, IEnumerable<string> companyNames, CancellationToken cancellationToken = default)
        {
            var namesList = companyNames.Distinct().ToList();

            if (namesList.Count == 0)
                return new Dictionary<string, decimal>();

            var companies = await _context.Companies
                .Where(c => c.PharmacyId == pharmacyId && namesList.Contains(c.Name))
                .Select(c => new { c.Name, c.Discount })
                .ToListAsync(cancellationToken);

            var result = companies.ToDictionary(c => c.Name, c => c.Discount);

            foreach (var name in namesList)
            {
                result.TryAdd(name, 0);
            }

            return result;
        }
    }
}