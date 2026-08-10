// Security/PasswordHasherService.cs
using Microsoft.AspNetCore.Identity;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.Modules.Auth.Domain.Entities;

namespace PharmacyContracts.Modules.Auth.Infrastructure.Security;

public class PasswordHasherService : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public bool Verify(string hashedPassword, string providedPassword)
        => _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
}