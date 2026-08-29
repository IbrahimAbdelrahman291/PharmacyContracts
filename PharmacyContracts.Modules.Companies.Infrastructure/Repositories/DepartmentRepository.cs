using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.Modules.Companies.Domain.Entities;
using PharmacyContracts.Modules.Companies.Infrastructure.Data;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly CompaniesDbContext _context;
        public DepartmentRepository(CompaniesDbContext context) => _context = context;

        public Task<CompanyDepartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.CompanyDepartments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        public Task<List<CompanyDepartment>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
            => _context.CompanyDepartments.Where(d => d.CompanyId == companyId).OrderBy(d => d.Name).ToListAsync(cancellationToken);

        public Task<bool> ExistsByNameAsync(Guid companyId, string name, CancellationToken cancellationToken = default)
            => _context.CompanyDepartments.AnyAsync(d => d.CompanyId == companyId && d.Name == name, cancellationToken);

        public async Task AddAsync(CompanyDepartment entity, CancellationToken cancellationToken = default)
            => await _context.CompanyDepartments.AddAsync(entity, cancellationToken);

        public void Update(CompanyDepartment entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.CompanyDepartments.Update(entity);
        }

        public void Remove(CompanyDepartment entity) => _context.CompanyDepartments.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}
