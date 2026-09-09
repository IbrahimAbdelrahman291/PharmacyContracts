using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces;

public interface IBalanceService
{
    Task<Result<CompanyBalanceResponseDto>> GetCompanyBalanceAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default);
    Task<Result<TotalBalanceResponseDto>> GetTotalBalanceAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<Result<AgingReportResponseDto>> GetAgingReportAsync(Guid pharmacyId, string? companyName, CancellationToken cancellationToken = default);
    Task<Result<List<CompanyBalanceResponseDto>>> GetTopDebtorsAsync(Guid pharmacyId, int top, CancellationToken cancellationToken = default);
}
