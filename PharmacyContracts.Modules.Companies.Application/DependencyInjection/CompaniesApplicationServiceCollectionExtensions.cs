using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.Modules.Companies.Application.Services;
using PharmacyContracts.Modules.Companies.Application.Validators;

namespace PharmacyContracts.Modules.Companies.Application.DependencyInjection;

public static class CompaniesApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCompaniesApplication(this IServiceCollection services)
    {
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddValidatorsFromAssemblyContaining<CreateCompanyRequestValidator>();

        return services;
    }
}