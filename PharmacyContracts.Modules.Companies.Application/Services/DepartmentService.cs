using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.Modules.Companies.Domain.Entities;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Companies.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICompanyRepository _companyRepository;

        public DepartmentService(IDepartmentRepository departmentRepository, ICompanyRepository companyRepository)
        {
            _departmentRepository = departmentRepository;
            _companyRepository = companyRepository;
        }

        public async Task<Result<DepartmentResponseDto>> CreateAsync(Guid pharmacyId, Guid companyId, CreateDepartmentRequestDto request, CancellationToken cancellationToken = default)
        {
            var company = await _companyRepository.GetByIdForPharmacyAsync(companyId, pharmacyId, cancellationToken);
            if (company is null)
                return Result<DepartmentResponseDto>.Failure("الشركة غير موجودة.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<DepartmentResponseDto>.Failure("اسم الإدارة مطلوب.");

            var exists = await _departmentRepository.ExistsByNameAsync(companyId, request.Name, cancellationToken);
            if (exists)
                return Result<DepartmentResponseDto>.Failure("هذه الإدارة مضافة بالفعل.");

            var department = new CompanyDepartment { CompanyId = companyId, Name = request.Name.Trim() };
            await _departmentRepository.AddAsync(department, cancellationToken);
            await _departmentRepository.SaveChangesAsync(cancellationToken);

            return Result<DepartmentResponseDto>.Success(new DepartmentResponseDto { Id = department.Id, Name = department.Name });
        }

        public async Task<Result<List<DepartmentResponseDto>>> GetByCompanyIdAsync(Guid pharmacyId, Guid companyId, CancellationToken cancellationToken = default)
        {
            var company = await _companyRepository.GetByIdForPharmacyAsync(companyId, pharmacyId, cancellationToken);
            if (company is null)
                return Result<List<DepartmentResponseDto>>.Failure("الشركة غير موجودة.");

            var departments = await _departmentRepository.GetByCompanyIdAsync(companyId, cancellationToken);
            return Result<List<DepartmentResponseDto>>.Success(
                departments.Select(d => new DepartmentResponseDto { Id = d.Id, Name = d.Name }).ToList());
        }

        public async Task<Result> DeleteAsync(Guid pharmacyId, Guid companyId, Guid departmentId, CancellationToken cancellationToken = default)
        {
            var company = await _companyRepository.GetByIdForPharmacyAsync(companyId, pharmacyId, cancellationToken);
            if (company is null)
                return Result.Failure("الشركة غير موجودة.");

            var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            if (department is null || department.CompanyId != companyId)
                return Result.Failure("الإدارة غير موجودة.");

            _departmentRepository.Remove(department);
            await _departmentRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
