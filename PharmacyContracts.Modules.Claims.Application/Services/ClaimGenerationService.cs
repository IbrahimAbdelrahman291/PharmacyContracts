using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.Mappings;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class ClaimGenerationService : IClaimGenerationService
    {
        private readonly ISalesQueryService _salesQueryService;
        private readonly ICompaniesQueryService _companiesQueryService;
        private readonly IClaimRepository _claimRepository;

        public ClaimGenerationService(
            ISalesQueryService salesQueryService,
            ICompaniesQueryService companiesQueryService,
            IClaimRepository claimRepository)
        {
            _salesQueryService = salesQueryService;
            _companiesQueryService = companiesQueryService;
            _claimRepository = claimRepository;
        }

        public async Task<Result<List<ClaimResponseDto>>> GenerateAsync(Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default)
        {
            if (month is < 1 or > 12)
                return Result<List<ClaimResponseDto>>.Failure("الشهر يجب أن يكون رقمًا بين 1 و 12.");

            var alreadyExists = await _claimRepository.ExistsForPeriodAsync(pharmacyId, month, year, cancellationToken);
            if (alreadyExists)
                return Result<List<ClaimResponseDto>>.Failure("تم استخراج مطالبات هذا الشهر بالفعل.");

            var totals = await _salesQueryService.GetCompanyBranchTotalsAsync(pharmacyId, month, year, cancellationToken);
            if (totals.Count == 0)
                return Result<List<ClaimResponseDto>>.Failure("لا توجد بيانات مبيعات لهذا الشهر.");

            var companyTotals = totals
                .GroupBy(t => t.CompanyName)
                .Select(g => new { CompanyName = g.Key, Total = g.Sum(t => t.TotalRemainingAmount) })
                .ToList();

            var companyNames = companyTotals.Select(c => c.CompanyName).ToList();
            var totalDiscountByCompany = await _companiesQueryService.GetDiscountPercentagesAsync(pharmacyId, companyNames, cancellationToken);
            var discountByCompany = await _companiesQueryService.GetItemDiscountPercentagesAsync(pharmacyId, companyNames, cancellationToken);
            var claims = new List<Claim>();

            foreach (var companyTotal in companyTotals)
            {
                var discounts = discountByCompany[companyTotal.CompanyName];
                var insights = await _salesQueryService.GetCompanyInsightsAsync(
                    pharmacyId, companyTotal.CompanyName, month, year, cancellationToken);
                var totalDiscount = totalDiscountByCompany[companyTotal.CompanyName];
                var afterDiscount = totalDiscount > 0
                    ? insights.TotalRemainingAmount * (1 - totalDiscount / 100)
                    : (insights.TotalLocalItemsAmount * (1 - discounts.LocalDiscountPercentage / 100))
                        + (insights.TotalImportedItemsAmount * (1 - discounts.ImportedDiscountPercentage / 100));

                claims.Add(new Claim
                {
                    PharmacyId = pharmacyId,
                    CompanyName = companyTotal.CompanyName,
                    Month = month,
                    Year = year,
                    ClaimAmount = insights.TotalRemainingAmount,
                    ClaimAmountAfterDiscount = afterDiscount,
                    PrescriptionsCount = insights.PrescriptionsCount,
                    Status = ClaimStatus.Pending
                });
            }

            await _claimRepository.AddRangeAsync(claims, cancellationToken);
            await _claimRepository.SaveChangesAsync(cancellationToken);

            return Result<List<ClaimResponseDto>>.Success(claims.Select(c => c.ToResponseDto()).ToList());
        }
    }
}
