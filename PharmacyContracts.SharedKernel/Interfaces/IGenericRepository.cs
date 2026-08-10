using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.SharedKernel.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
