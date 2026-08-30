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
            var discountByCompany = await _companiesQueryService.GetDiscountPercentagesAsync(pharmacyId, companyNames, cancellationToken);

            var claims = companyTotals.Select(c =>
            {
                var discountPercentage = discountByCompany[c.CompanyName];
                var afterDiscount = c.Total - (c.Total * discountPercentage / 100);

                return new Claim
                {
                    PharmacyId = pharmacyId,
                    CompanyName = c.CompanyName,
                    Month = month,
                    Year = year,
                    ClaimAmountAfterDiscount = afterDiscount,
                    Status = ClaimStatus.Pending
                };
            }).ToList();

            await _claimRepository.AddRangeAsync(claims, cancellationToken);
            await _claimRepository.SaveChangesAsync(cancellationToken);

            return Result<List<ClaimResponseDto>>.Success(claims.Select(c => c.ToResponseDto()).ToList());
        }
    }
}