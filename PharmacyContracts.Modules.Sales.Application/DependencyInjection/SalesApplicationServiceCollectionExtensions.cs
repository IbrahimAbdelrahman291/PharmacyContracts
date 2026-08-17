using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Application.Services;

namespace PharmacyContracts.Modules.Sales.Application.DependencyInjection
{
    public static class SalesApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddSalesApplication(this IServiceCollection services)
        {
            services.AddScoped<IFileIntegrityService, FileIntegrityService>();
            services.AddScoped<ISalesRowValidator, SalesRowValidator>();
            services.AddScoped<ISalesUploadService, SalesUploadService>();

            return services;
        }
    }
}
