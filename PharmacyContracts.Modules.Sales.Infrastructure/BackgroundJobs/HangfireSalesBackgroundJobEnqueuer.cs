using Hangfire;
using PharmacyContracts.Modules.Sales.Application.Interfaces;

namespace PharmacyContracts.Modules.Sales.Infrastructure.BackgroundJobs
{
    public class HangfireSalesBackgroundJobEnqueuer : ISalesBackgroundJobEnqueuer
    {
        public void EnqueueProcessBatch(Guid batchId)
        {
            BackgroundJob.Enqueue<ProcessSalesBatchJob>(job => job.ExecuteAsync(batchId, CancellationToken.None));
        }
    }
}
