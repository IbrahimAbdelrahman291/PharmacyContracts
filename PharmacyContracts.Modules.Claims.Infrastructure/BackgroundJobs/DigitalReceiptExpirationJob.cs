using Microsoft.Extensions.Logging;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Enums;

namespace PharmacyContracts.Modules.Claims.Infrastructure.BackgroundJobs;

public class DigitalReceiptExpirationJob
{
    private readonly IDigitalReceiptRepository _repository;
    private readonly IDigitalReceiptFileStorage _storage;
    private readonly ILogger<DigitalReceiptExpirationJob> _logger;
    public DigitalReceiptExpirationJob(IDigitalReceiptRepository repository, IDigitalReceiptFileStorage storage, ILogger<DigitalReceiptExpirationJob> logger) { _repository = repository; _storage = storage; _logger = logger; }
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var receipts = await _repository.GetExpiredCandidatesAsync(DateTime.UtcNow, cancellationToken);
        var expired = 0;
        foreach (var receipt in receipts)
        {
            if (receipt.FilePath is not null && !await _storage.DeleteAsync(receipt.FilePath, cancellationToken)) { _logger.LogWarning("Could not delete expired digital receipt file for {ReceiptId}.", receipt.Id); continue; }
            receipt.FilePath = null; receipt.Status = DigitalReceiptStatus.Expired; _repository.Update(receipt); expired++;
        }
        if (expired > 0) await _repository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Expired {Count} digital receipts.", expired);
    }
}
