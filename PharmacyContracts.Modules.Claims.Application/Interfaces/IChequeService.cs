using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IChequeService
    {
        Task<Result<ChequeCreationPreparationDto>> PrepareAsync(Guid pharmacyId, string companyName, int month, int year, CancellationToken cancellationToken = default);
        Task<Result<List<ChequeResponseDto>>> CreateAsync(Guid pharmacyId, Guid claimId, CreateChequesRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<List<ChequeResponseDto>>> GetAsync(Guid pharmacyId, string? companyName, int? month, int? year, CancellationToken cancellationToken = default);
        Task<Result> UpdateStatusAsync(Guid pharmacyId, Guid chequeId, UpdateChequeStatusRequestDto request, CancellationToken cancellationToken = default);
    }
}
