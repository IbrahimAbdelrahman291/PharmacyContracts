using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Contracts;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class ClaimsPivotService : IClaimsPivotService
    {
        private readonly ISalesQueryService _salesQueryService;
        private readonly ICompaniesQueryService _companiesQueryService;

        public ClaimsPivotService(ISalesQueryService salesQueryService, ICompaniesQueryService companiesQueryService)
        {
            _salesQueryService = salesQueryService;
            _companiesQueryService = companiesQueryService;
        }

        public async Task<Result<ClaimsPivotResponseDto>> GetPivotAsync(Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default)
        {
            if (month is < 1 or > 12)
                return Result<ClaimsPivotResponseDto>.Failure("الشهر يجب أن يكون رقمًا بين 1 و 12.");

            if (year is < 2000 or > 2100)
                return Result<ClaimsPivotResponseDto>.Failure("السنة غير صحيحة.");

            var totals = await _salesQueryService.GetCompanyBranchTotalsAsync(pharmacyId, month, year, cancellationToken);

            if (totals.Count == 0)
            {
                return Result<ClaimsPivotResponseDto>.Success(new ClaimsPivotResponseDto
                {
                    Branches = new List<string>(),
                    Rows = new List<CompanyPivotRowDto>(),
                    TotalsRow = new PivotTotalsRowDto()
                });
            }

            var branches = totals
                .Select(t => t.BranchName)
                .Distinct()
                .OrderBy(b => b)
                .ToList();

            var companyNames = totals.Select(t => t.CompanyName).Distinct().ToList();

            var totalDiscountByCompany = await _companiesQueryService.GetDiscountPercentagesAsync(pharmacyId, companyNames, cancellationToken);
            var discountByCompany = await _companiesQueryService.GetItemDiscountPercentagesAsync(pharmacyId, companyNames, cancellationToken);
            var insightsByCompany = new Dictionary<string, CompanyInsightsContract>();

            foreach (var companyName in companyNames)
            {
                insightsByCompany[companyName] = await _salesQueryService.GetCompanyInsightsAsync(
                    pharmacyId, companyName, month, year, cancellationToken);
            }

            var rows = totals
                .GroupBy(t => t.CompanyName)
                .Select(companyGroup =>
                {
                    var amountsByBranch = branches.ToDictionary(
                        branch => branch,
                        branch => companyGroup
                            .Where(t => t.BranchName == branch)
                            .Sum(t => t.TotalRemainingAmount));

                    var total = companyGroup.Sum(t => t.TotalRemainingAmount);
                    var discounts = discountByCompany[companyGroup.Key];
                    var insights = insightsByCompany[companyGroup.Key];
                    var totalDiscount = totalDiscountByCompany[companyGroup.Key];
                    var totalAfterDiscount = totalDiscount > 0
                        ? insights.TotalRemainingAmount * (1 - totalDiscount / 100)
                        : (insights.TotalLocalItemsAmount * (1 - discounts.LocalDiscountPercentage / 100))
                            + (insights.TotalImportedItemsAmount * (1 - discounts.ImportedDiscountPercentage / 100));

                    return new CompanyPivotRowDto
                    {
                        CompanyName = companyGroup.Key,
                        AmountsByBranch = amountsByBranch,
                        Total = total,
                        TotalAfterDiscount = totalAfterDiscount
                    };
                })
                .OrderByDescending(r => r.Total)
                .ToList();

            var totalsRow = new PivotTotalsRowDto
            {
                AmountsByBranch = branches.ToDictionary(
                    branch => branch,
                    branch => rows.Sum(r => r.AmountsByBranch[branch])),
                GrandTotal = rows.Sum(r => r.Total)
            };

            return Result<ClaimsPivotResponseDto>.Success(new ClaimsPivotResponseDto
            {
                Branches = branches,
                Rows = rows,
                TotalsRow = totalsRow
            });
        }
    }
}
