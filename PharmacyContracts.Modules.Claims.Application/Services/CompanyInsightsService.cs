using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class CompanyInsightsService : ICompanyInsightsService
    {
        private readonly ISalesQueryService _salesQueryService;
        private readonly ICompaniesQueryService _companiesQueryService;

        public CompanyInsightsService(ISalesQueryService salesQueryService, ICompaniesQueryService companiesQueryService)
        {
            _salesQueryService = salesQueryService;
            _companiesQueryService = companiesQueryService;
        }

        public async Task<Result<CompanyInsightsResponseDto>> GetInsightsAsync(
            Guid pharmacyId, string companyName, int month, int year, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return Result<CompanyInsightsResponseDto>.Failure("اسم الشركة مطلوب.");

            if (month is < 1 or > 12)
                return Result<CompanyInsightsResponseDto>.Failure("الشهر يجب أن يكون رقمًا بين 1 و 12.");

            var insights = await _salesQueryService.GetCompanyInsightsAsync(pharmacyId, companyName, month, year, cancellationToken);
            var discountPercentage = await _companiesQueryService.GetDiscountPercentageAsync(pharmacyId, companyName, cancellationToken);

            var amountAfterDiscount = insights.TotalRemainingAmount - (insights.TotalRemainingAmount * discountPercentage / 100);

            return Result<CompanyInsightsResponseDto>.Success(new CompanyInsightsResponseDto
            {
                CompanyName = companyName,
                PrescriptionsCount = insights.PrescriptionsCount,
                TotalRemainingAmount = insights.TotalRemainingAmount,
                TotalLocalItemsAmount = insights.TotalLocalItemsAmount,
                TotalImportedItemsAmount = insights.TotalImportedItemsAmount,
                DiscountPercentage = discountPercentage,
                AmountAfterDiscount = amountAfterDiscount
            });
        }
    }
}
