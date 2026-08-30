using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Infrastructure.BackgroundJobs;
using PharmacyContracts.Modules.Claims.Infrastructure.Data;
using PharmacyContracts.Modules.Claims.Infrastructure.Repositories;


namespace PharmacyContracts.Modules.Claims.Infrastructure.DependencyInjection
{
    public static class ClaimsInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddClaimsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ClaimsDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "claims")));

            services.AddScoped<IClaimRepository, ClaimRepository>();
            services.AddScoped<IClaimReviewRepository, ClaimReviewRepository>();
            services.AddScoped<IChequeRepository, ChequeRepository>();

            services.AddScoped<ChequeOverdueSweepJob>();

            return services;
        }
    }
}
