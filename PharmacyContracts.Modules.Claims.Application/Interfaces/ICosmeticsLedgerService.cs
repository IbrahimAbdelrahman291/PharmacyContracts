using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces;

public interface ICosmeticsLedgerService
{
    Task<Result<CosmeticsCompanyDto>> CreateCompanyAsync(Guid pharmacyId, CosmeticsLookupRequestDto request, CancellationToken cancellationToken);
    Task<Result<CosmeticsCompanyDto>> UpdateCompanyAsync(Guid pharmacyId, Guid id, CosmeticsLookupRequestDto request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteCompanyAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken);
    Task<Result<List<CosmeticsCompanyDto>>> GetCompaniesAsync(Guid pharmacyId, CancellationToken cancellationToken);
    Task<Result<CosmeticsNameDto>> CreateNameAsync(Guid pharmacyId, CosmeticsLookupRequestDto request, CancellationToken cancellationToken);
    Task<Result<CosmeticsNameDto>> UpdateNameAsync(Guid pharmacyId, Guid id, CosmeticsLookupRequestDto request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteNameAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken);
    Task<Result<List<CosmeticsNameDto>>> GetNamesAsync(Guid pharmacyId, CancellationToken cancellationToken);
    Task<Result<CosmeticsTransactionDto>> CreateTransactionAsync(Guid pharmacyId, CosmeticsTransactionRequestDto request, CancellationToken cancellationToken);
    Task<Result<CosmeticsTransactionDto>> UpdateTransactionAsync(Guid pharmacyId, Guid id, CosmeticsTransactionRequestDto request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteTransactionAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken);
    Task<Result<CosmeticsReportDto>> GetReportAsync(Guid pharmacyId, Guid? companyId, Guid? nameId, string? name, DateTime? from, DateTime? to, CancellationToken cancellationToken);
}
