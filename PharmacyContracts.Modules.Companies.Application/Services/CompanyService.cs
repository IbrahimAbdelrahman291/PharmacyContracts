using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.Modules.Companies.Application.Mappings;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Companies.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    public CompanyService(ICompanyRepository companyRepository) => _companyRepository = companyRepository;

    public async Task<Result<CompanyResponseDto>> CreateAsync(Guid pharmacyId, CreateCompanyRequestDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _companyRepository.ExistsByNameAsync(pharmacyId, request.Name, cancellationToken);
        if (exists)
            return Result<CompanyResponseDto>.Failure("Company with the same name already exists.");

        var company = request.ToEntity(pharmacyId);
        await _companyRepository.AddAsync(company, cancellationToken);
        await _companyRepository.SaveChangesAsync(cancellationToken);

        return Result<CompanyResponseDto>.Success(company.ToResponseDto());
    }

    public async Task<Result<CompanyResponseDto>> UpdateAsync(Guid pharmacyId, Guid companyId, UpdateCompanyRequestDto request, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdForPharmacyAsync(companyId, pharmacyId, cancellationToken);
        if (company is null)
            return Result<CompanyResponseDto>.Failure("Company not found.");

        company.ApplyUpdate(request);
        _companyRepository.Update(company);
        await _companyRepository.SaveChangesAsync(cancellationToken);

        return Result<CompanyResponseDto>.Success(company.ToResponseDto());
    }

    public async Task<Result<PagedResult<CompanyResponseDto>>> GetPagedAsync(Guid pharmacyId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var paged = await _companyRepository.GetPagedByPharmacyAsync(pharmacyId, pagination, cancellationToken);

        var response = new PagedResult<CompanyResponseDto>
        {
            Items = paged.Items.Select(c => c.ToResponseDto()).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };

        return Result<PagedResult<CompanyResponseDto>>.Success(response);
    }

    public async Task<Result<CompanyResponseDto>> GetByIdAsync(Guid pharmacyId, Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdForPharmacyAsync(companyId, pharmacyId, cancellationToken);
        if (company is null)
            return Result<CompanyResponseDto>.Failure("Company not found.");

        return Result<CompanyResponseDto>.Success(company.ToResponseDto());
    }
}