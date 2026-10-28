using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces;

public interface IDigitalReceiptService
{
    Task<Result<DigitalReceiptResponseDto>> CreateAsync(Guid pharmacyId, CreateDigitalReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<List<DigitalReceiptResponseDto>>> GetAllAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<Result<DigitalReceiptResponseDto>> UploadAsync(Guid pharmacyId, Guid id, Stream content, string fileName, CancellationToken cancellationToken = default);
    Task<Result<DigitalReceiptDownloadDto>> DownloadAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken = default);
    Task<Result> CancelAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken = default);
}
