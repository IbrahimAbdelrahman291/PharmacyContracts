using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.Services;

namespace PharmacyContracts.Modules.Claims.Application.DependencyInjection
{
    public static class ClaimsApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddClaimsApplication(this IServiceCollection services)
        {
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IClaimsPivotService, ClaimsPivotService>();
            services.AddScoped<ICompanyProfileService, CompanyProfileService>();
            services.AddScoped<ICompanyInsightsService, CompanyInsightsService>();

            return services;
        }
    }
}
