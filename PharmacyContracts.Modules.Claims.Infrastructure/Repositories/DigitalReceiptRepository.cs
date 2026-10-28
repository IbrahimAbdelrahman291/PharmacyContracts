using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Repositories;

public class DigitalReceiptRepository : IDigitalReceiptRepository
{
    private readonly ClaimsDbContext _context;
    public DigitalReceiptRepository(ClaimsDbContext context) => _context = context;
    public Task<DigitalReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => _context.DigitalReceipts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<List<DigitalReceipt>> GetByPharmacyAsync(Guid pharmacyId, CancellationToken cancellationToken = default) => _context.DigitalReceipts.Where(x => x.PharmacyId == pharmacyId).OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
    public Task<bool> ExistsForPeriodAsync(Guid pharmacyId, Guid companyId, int month, int year, CancellationToken cancellationToken = default) => _context.DigitalReceipts.AnyAsync(x => x.PharmacyId == pharmacyId && x.CompanyId == companyId && x.Month == month && x.Year == year, cancellationToken);
    public Task<List<DigitalReceipt>> GetExpiredCandidatesAsync(DateTime utcNow, CancellationToken cancellationToken = default) => _context.DigitalReceipts.Where(x => x.Status == DigitalReceiptStatus.Available && x.ExpiresAt <= utcNow).ToListAsync(cancellationToken);
    public Task AddAsync(DigitalReceipt receipt, CancellationToken cancellationToken = default) => _context.DigitalReceipts.AddAsync(receipt, cancellationToken).AsTask();
    public void Update(DigitalReceipt receipt) { receipt.UpdatedAt = DateTime.UtcNow; _context.DigitalReceipts.Update(receipt); }
    public void Remove(DigitalReceipt receipt) => _context.DigitalReceipts.Remove(receipt);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}
