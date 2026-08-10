using Microsoft.Extensions.Configuration;
using PharmacyContracts.Modules.Auth.Domain.Entities;
using PharmacyContracts.Modules.Auth.Domain.Enums;
using PharmacyContracts.Modules.Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Auth.Application.Interfaces;

namespace PharmacyContracts.Modules.Auth.Infrastructure.Seeding
{
    public static class AuthSeeder
    {
        public static async Task SeedAsync(AuthDbContext dbContext, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            var hasSuperAdmin = await dbContext.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin);
            if (hasSuperAdmin)
                return;

            var email = configuration["SuperAdminSeed:Email"];
            var password = configuration["SuperAdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("SuperAdminSeed:Email and SuperAdminSeed:Password must be configured.");

            var superAdmin = new User
            {
                Email = email,
                PasswordHash = passwordHasher.Hash(password),
                Role = UserRole.SuperAdmin,
                IsActive = true
            };

            await dbContext.Users.AddAsync(superAdmin);
            await dbContext.SaveChangesAsync();
        }
    }
}
