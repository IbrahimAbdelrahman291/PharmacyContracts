using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces;

public interface ICosmeticsLedgerRepository
{
    Task<List<CosmeticsCompany>> GetCompaniesAsync(Guid pharmacyId, CancellationToken cancellationToken);
    Task<CosmeticsCompany?> GetCompanyAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken);
    Task<CosmeticsCompany?> FindCompanyByNameAsync(Guid pharmacyId, string name, Guid? excludeId, CancellationToken cancellationToken);
    Task<List<CosmeticsName>> GetNamesAsync(Guid pharmacyId, CancellationToken cancellationToken);
    Task<CosmeticsName?> GetNameAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken);
    Task<CosmeticsName?> FindNameByNameAsync(Guid pharmacyId, string name, Guid? excludeId, CancellationToken cancellationToken);
    Task<CosmeticsTransaction?> GetTransactionAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken);
    Task<List<CosmeticsTransaction>> GetTransactionsAsync(Guid pharmacyId, Guid? companyId, Guid? nameId, string? name, DateTime? from, DateTime? to, CancellationToken cancellationToken);
    Task<bool> CompanyHasTransactionsAsync(Guid pharmacyId, Guid companyId, CancellationToken cancellationToken);
    Task<bool> NameHasTransactionsAsync(Guid pharmacyId, Guid nameId, CancellationToken cancellationToken);
    Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
    void Remove<T>(T entity) where T : class;
    Task RecalculateBalancesAsync(Guid pharmacyId, IEnumerable<Guid> companyIds, CancellationToken cancellationToken);
    Task SaveAndRecalculateBalancesAsync(Guid pharmacyId, IEnumerable<Guid> companyIds, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
