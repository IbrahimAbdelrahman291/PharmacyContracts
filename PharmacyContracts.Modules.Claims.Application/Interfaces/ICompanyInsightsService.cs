using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface ICompanyInsightsService
    {
        Task<Result<CompanyInsightsResponseDto>> GetInsightsAsync(
            Guid pharmacyId, string companyName, int month, int year, CancellationToken cancellationToken = default);
    }
}
