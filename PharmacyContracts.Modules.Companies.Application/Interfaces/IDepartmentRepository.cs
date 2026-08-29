using PharmacyContracts.Modules.Companies.Domain.Entities;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Companies.Application.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<CompanyDepartment>
    {
        Task<List<CompanyDepartment>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(Guid companyId, string name, CancellationToken cancellationToken = default);
    }
}
