// Services/AuthService.cs
using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result<LoginResponseDto>.Failure("Email or password is incorrect.");

        if (!user.IsActive)
            return Result<LoginResponseDto>.Failure("This account is deactivated.");

        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
            return Result<LoginResponseDto>.Failure("Email or password is incorrect.");

        var token = _tokenGenerator.GenerateToken(user);

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Role = user.Role.ToString(),
            PharmacyName = user.PharmacyName
        });
    }
}