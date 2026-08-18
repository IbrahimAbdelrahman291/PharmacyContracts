using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface ICompanyProfileService
    {
        Task<Result<PagedResult<CompanyProfileRecordDto>>> GetProfileAsync(
            Guid pharmacyId, string companyName, int? month, int? year,
            PaginationParams pagination, CancellationToken cancellationToken = default);
    }
}
