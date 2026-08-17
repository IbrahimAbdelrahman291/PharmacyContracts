// Repositories/SalesUploadBatchRepository.cs
using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Domain.Entities;
using PharmacyContracts.Modules.Sales.Domain.Enums;
using PharmacyContracts.Modules.Sales.Infrastructure.Data;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Repositories;

public class SalesUploadBatchRepository : ISalesUploadBatchRepository
{
    private readonly SalesDbContext _context;
    public SalesUploadBatchRepository(SalesDbContext context) => _context = context;

    public Task<SalesUploadBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.SalesUploadBatches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<SalesUploadBatch?> GetByPharmacyAndHashAsync(Guid pharmacyId, string fileHash, CancellationToken cancellationToken = default)
        => _context.SalesUploadBatches.FirstOrDefaultAsync(b => b.PharmacyId == pharmacyId && b.FileHash == fileHash, cancellationToken);

    public Task<List<SalesUploadBatch>> GetStuckBatchesAsync(DateTime processingThreshold, CancellationToken cancellationToken = default)
    {
        return _context.SalesUploadBatches
            .Where(b =>
                b.Status == BatchStatus.Failed ||
                (b.Status == BatchStatus.Processing &&
                 (b.LastProcessingAttemptAt == null || b.LastProcessingAttemptAt < processingThreshold)))
            .Where(b => b.RecoveryAttempts < 5)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SalesUploadBatch entity, CancellationToken cancellationToken = default)
        => await _context.SalesUploadBatches.AddAsync(entity, cancellationToken);

    public void Update(SalesUploadBatch entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.SalesUploadBatches.Update(entity);
    }

    public void Remove(SalesUploadBatch entity) => _context.SalesUploadBatches.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}