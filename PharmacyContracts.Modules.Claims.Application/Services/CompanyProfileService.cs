using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class CompanyProfileService : ICompanyProfileService
    {
        private readonly ISalesQueryService _salesQueryService;
        public CompanyProfileService(ISalesQueryService salesQueryService) => _salesQueryService = salesQueryService;

        public async Task<Result<PagedResult<CompanyProfileRecordDto>>> GetProfileAsync(
            Guid pharmacyId, string companyName, int? month, int? year,
            PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return Result<PagedResult<CompanyProfileRecordDto>>.Failure("اسم الشركة مطلوب.");

            var paged = await _salesQueryService.GetCompanyProfileAsync(
                pharmacyId, companyName, month, year, pagination, cancellationToken);

            var response = new PagedResult<CompanyProfileRecordDto>
            {
                Items = paged.Items.Select(r => new CompanyProfileRecordDto
                {
                    SaleDate = r.SaleDate,
                    ImportedItemsTotal = r.ImportedItemsTotal,
                    LocalItemsTotal = r.LocalItemsTotal,
                    GrossTotal = r.GrossTotal,
                    DiscountOnTotal = r.DiscountOnTotal,
                    DiscountOnItems = r.DiscountOnItems,
                    SubTotal = r.SubTotal,
                    RemainingAmount = r.RemainingAmount
                }).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };

            return Result<PagedResult<CompanyProfileRecordDto>>.Success(response);
        }
    }
}