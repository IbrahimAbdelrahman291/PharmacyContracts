using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Companies.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<Result<DepartmentResponseDto>> CreateAsync(Guid pharmacyId, Guid companyId, CreateDepartmentRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<List<DepartmentResponseDto>>> GetByCompanyIdAsync(Guid pharmacyId, Guid companyId, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid pharmacyId, Guid companyId, Guid departmentId, CancellationToken cancellationToken = default);
    }
}
