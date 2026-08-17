using PharmacyContracts.Modules.Sales.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface ISalesUploadBatchRepository : IGenericRepository<SalesUploadBatch>
    {
        Task<SalesUploadBatch?> GetByPharmacyAndHashAsync(Guid pharmacyId, string fileHash, CancellationToken cancellationToken = default);
        Task<List<SalesUploadBatch>> GetStuckBatchesAsync(DateTime processingThreshold, CancellationToken cancellationToken = default);
    }
}
