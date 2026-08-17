// BackgroundJobs/SalesBatchRecoverySweepJob.cs
using Microsoft.Extensions.Logging;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Domain.Enums;

namespace PharmacyContracts.Modules.Sales.Infrastructure.BackgroundJobs;

public class SalesBatchRecoverySweepJob
{
    private readonly ISalesUploadBatchRepository _batchRepository;
    private readonly ISalesBackgroundJobEnqueuer _jobEnqueuer;
    private readonly ILogger<SalesBatchRecoverySweepJob> _logger;

    private static readonly TimeSpan StuckProcessingThreshold = TimeSpan.FromMinutes(10);

    public SalesBatchRecoverySweepJob(
        ISalesUploadBatchRepository batchRepository,
        ISalesBackgroundJobEnqueuer jobEnqueuer,
        ILogger<SalesBatchRecoverySweepJob> logger)
    {
        _batchRepository = batchRepository;
        _jobEnqueuer = jobEnqueuer;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var threshold = DateTime.UtcNow.Subtract(StuckProcessingThreshold);
        var stuckBatches = await _batchRepository.GetStuckBatchesAsync(threshold, cancellationToken);

        foreach (var batch in stuckBatches)
        {
            batch.RecoveryAttempts += 1;

            if (batch.RecoveryAttempts >= 5)
            {
                batch.Status = BatchStatus.RequiresManualIntervention;
                _batchRepository.Update(batch);
                await _batchRepository.SaveChangesAsync(cancellationToken);

                _logger.LogCritical(
                    "Sales batch {BatchId} exceeded max recovery attempts and requires manual intervention.",
                    batch.Id);

                continue;
            }

            batch.Status = BatchStatus.Pending;
            _batchRepository.Update(batch);
            await _batchRepository.SaveChangesAsync(cancellationToken);

            _jobEnqueuer.EnqueueProcessBatch(batch.Id);

            _logger.LogWarning(
                "Sales batch {BatchId} re-enqueued for recovery. Attempt {Attempt}/5.",
                batch.Id, batch.RecoveryAttempts);
        }
    }
}