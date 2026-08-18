// SharedKernel/Interfaces/ISalesQueryService.cs
using PharmacyContracts.SharedKernel.Contracts;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.SharedKernel.Interfaces;

// Contract بيسمح لأي module تاني (زي Claims) يقرأ بيانات المبيعات المجمّعة
// من غير ما يعرف حاجة عن تفاصيل Sales module الداخلية (DbContext, Entities, إلخ)
public interface ISalesQueryService
{
    Task<List<string>> GetDistinctBranchesAsync(Guid pharmacyId, CancellationToken cancellationToken = default);

    Task<List<SalesCompanyBranchTotalContract>> GetCompanyBranchTotalsAsync(
        Guid pharmacyId, int month, int year, CancellationToken cancellationToken = default);

    Task<PagedResult<SalesRecordProfileContract>> GetCompanyProfileAsync(
        Guid pharmacyId, string companyName, int? month, int? year,
        PaginationParams pagination, CancellationToken cancellationToken = default);
}