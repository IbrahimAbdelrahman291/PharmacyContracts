using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.Modules.Companies.Domain.Entities;

namespace PharmacyContracts.Modules.Companies.Application.Mappings;

public static class CompanyMappings
{
    public static CompanyResponseDto ToResponseDto(this Company company)
    {
        return new CompanyResponseDto
        {
            Id = company.Id,
            Name = company.Name,
            LocalDiscountPercentage = company.LocalDiscountPercentage,
            ImportedDiscountPercentage = company.ImportedDiscountPercentage,
            TaxPercentage = company.TaxPercentage,
            AdministrativeExpensesPercentage = company.AdministrativeExpensesPercentage,
            ChequeSettlementPeriodInDays = company.ChequeSettlementPeriodInDays,
            CreatedAt = company.CreatedAt
        };
    }

    public static Company ToEntity(this CreateCompanyRequestDto dto, Guid pharmacyId)
    {
        return new Company
        {
            Name = dto.Name,
            LocalDiscountPercentage = dto.LocalDiscountPercentage,
            ImportedDiscountPercentage = dto.ImportedDiscountPercentage,
            TaxPercentage = dto.TaxPercentage,
            AdministrativeExpensesPercentage = dto.AdministrativeExpensesPercentage,
            ChequeSettlementPeriodInDays = dto.ChequeSettlementPeriodInDays,
            PharmacyId = pharmacyId
        };
    }

    public static void ApplyUpdate(this Company company, UpdateCompanyRequestDto dto)
    {
        company.Name = dto.Name;
        company.LocalDiscountPercentage = dto.LocalDiscountPercentage;
        company.ImportedDiscountPercentage = dto.ImportedDiscountPercentage;
        company.TaxPercentage = dto.TaxPercentage;
        company.AdministrativeExpensesPercentage = dto.AdministrativeExpensesPercentage;
        company.ChequeSettlementPeriodInDays = dto.ChequeSettlementPeriodInDays;
    }
}