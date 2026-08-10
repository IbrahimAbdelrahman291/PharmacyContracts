using PharmacyContracts.Modules.Companies.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Companies.Application.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetByIdForPharmacyAsync(Guid id, Guid pharmacyId, CancellationToken cancellationToken = default);
        Task<PagedResult<Company>> GetPagedByPharmacyAsync(Guid pharmacyId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(Guid pharmacyId, string name, CancellationToken cancellationToken = default);
    }
}
