

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface IFileIntegrityService
    {
        bool IsValidXlsxSignature(Stream fileStream);
        Task<string> ComputeSha256Async(Stream fileStream, CancellationToken cancellationToken = default);
    }
}
