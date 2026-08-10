using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Companies.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<CompanyResponseDto>> CreateAsync(Guid pharmacyId, CreateCompanyRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<CompanyResponseDto>> UpdateAsync(Guid pharmacyId, Guid companyId, UpdateCompanyRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<CompanyResponseDto>>> GetPagedAsync(Guid pharmacyId, PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<Result<CompanyResponseDto>> GetByIdAsync(Guid pharmacyId, Guid companyId, CancellationToken cancellationToken = default);
    }
}
