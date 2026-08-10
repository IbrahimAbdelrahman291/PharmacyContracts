// Services/UserService.cs
using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.Modules.Auth.Application.Mappings;
using PharmacyContracts.Modules.Auth.Domain.Enums;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Auth.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            return Result<UserResponseDto>.Failure("Email already exists.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            return Result<UserResponseDto>.Failure("Invalid role.");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = request.ToEntity(passwordHash, role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<UserResponseDto>.Success(user.ToResponseDto());
    }

    public async Task<Result<List<UserResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return Result<List<UserResponseDto>>.Success(users.Select(u => u.ToResponseDto()).ToList());
    }

    public async Task<Result> UpdateStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        user.IsActive = isActive;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}