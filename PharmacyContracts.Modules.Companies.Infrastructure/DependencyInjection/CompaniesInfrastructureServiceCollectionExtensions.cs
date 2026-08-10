using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.Modules.Companies.Infrastructure.Data;
using PharmacyContracts.Modules.Companies.Infrastructure.Repositories;

namespace PharmacyContracts.Modules.Companies.Infrastructure.DependencyInjection;

public static class CompaniesInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddCompaniesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CompaniesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "companies")));

        services.AddScoped<ICompanyRepository, CompanyRepository>();

        return services;
    }
}