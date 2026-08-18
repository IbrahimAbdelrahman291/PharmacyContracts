using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services;

public class ClaimsPivotService : IClaimsPivotService
{
    private readonly ISalesQueryService _salesQueryService;
    public ClaimsPivotService(ISalesQueryService salesQueryService) => _salesQueryService = salesQueryService;

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
                Rows = new List<CompanyPivotRowDto>()
            });
        }

        // أعمدة الجدول = كل الفروع المميزة اللي ظهرت في الشهر دا
        var branches = totals
            .Select(t => t.BranchName)
            .Distinct()
            .OrderBy(b => b)
            .ToList();

        // صفوف الجدول = كل شركة، مع مبلغها في كل فرع + إجمالي كل الفروع
        var rows = totals
            .GroupBy(t => t.CompanyName)
            .Select(companyGroup =>
            {
                var amountsByBranch = branches.ToDictionary(
                    branch => branch,
                    branch => companyGroup
                        .Where(t => t.BranchName == branch)
                        .Sum(t => t.TotalRemainingAmount));

                return new CompanyPivotRowDto
                {
                    CompanyName = companyGroup.Key,
                    AmountsByBranch = amountsByBranch,
                    Total = companyGroup.Sum(t => t.TotalRemainingAmount)
                };
            })
            .OrderByDescending(r => r.Total)
            .ToList();

        return Result<ClaimsPivotResponseDto>.Success(new ClaimsPivotResponseDto
        {
            Branches = branches,
            Rows = rows
        });
    }
}