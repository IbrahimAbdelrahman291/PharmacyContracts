using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;
using Microsoft.AspNetCore.Http;

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface ISalesUploadService
    {
        Task<Result<UploadBatchResponseDto>> UploadAsync(Guid pharmacyId, Guid createdBy, IFormFile file, CancellationToken cancellationToken = default);
        Task<Result<BatchStatusResponseDto>> GetStatusAsync(Guid pharmacyId, Guid batchId, CancellationToken cancellationToken = default);   // ← إضافة pharmacyId
    }
}
