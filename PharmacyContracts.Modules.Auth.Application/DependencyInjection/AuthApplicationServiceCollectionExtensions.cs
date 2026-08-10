using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.Modules.Auth.Application.Services;
using PharmacyContracts.Modules.Auth.Application.Validators;

namespace PharmacyContracts.Modules.Auth.Application.DependencyInjection
{
    public static class AuthApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

            return services;
        }
    }
}
