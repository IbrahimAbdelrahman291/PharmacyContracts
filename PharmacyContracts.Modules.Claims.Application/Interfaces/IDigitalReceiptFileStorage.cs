namespace PharmacyContracts.Modules.Claims.Application.Interfaces;

public interface IDigitalReceiptFileStorage
{
    Task<string> SaveAsync(Guid receiptId, Stream content, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
    Task<Stream?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default);
}
