using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.Modules.Auth.Infrastructure.Data;
using PharmacyContracts.Modules.Auth.Infrastructure.Repositories;
using PharmacyContracts.Modules.Auth.Infrastructure.Security;

namespace PharmacyContracts.Modules.Auth.Infrastructure.DependencyInjection
{
    public static class AuthInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "auth")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasherService>();
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
