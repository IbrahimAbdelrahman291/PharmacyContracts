using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces;

public interface IDigitalReceiptRepository
{
    Task<DigitalReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<DigitalReceipt>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForPeriodAsync(Guid pharmacyId, Guid companyId, Guid? departmentId, int month, int year, CancellationToken cancellationToken = default);
    Task<List<DigitalReceipt>> GetExpiredCandidatesAsync(DateTime utcNow, CancellationToken cancellationToken = default);
    Task AddAsync(DigitalReceipt receipt, CancellationToken cancellationToken = default);
    void Update(DigitalReceipt receipt);
    void Remove(DigitalReceipt receipt);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
