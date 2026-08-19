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

        public async Task<int> GetChequeSettlementPeriodInDaysAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.PharmacyId == pharmacyId && c.Name == companyName, cancellationToken);

            return company?.ChequeSettlementPeriodInDays ?? 45; // قيمة افتراضية لو الشركة مش موجودة بنفس الاسم بالظبط
        }
    }
}
