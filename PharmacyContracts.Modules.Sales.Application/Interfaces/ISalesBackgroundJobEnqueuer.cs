

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface ISalesBackgroundJobEnqueuer
    {
        void EnqueueProcessBatch(Guid batchId);
    }
}
