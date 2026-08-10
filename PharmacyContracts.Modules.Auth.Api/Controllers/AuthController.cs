// Controllers/AuthController.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.Modules.Auth.Application.Interfaces;

namespace PharmacyContracts.Modules.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginRequestDto> _validator;

    public AuthController(IAuthService authService, IValidator<LoginRequestDto> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        var result = await _authService.LoginAsync(request, cancellationToken);
        if (!result.Succeeded)
            return Unauthorized(new { errors = result.Errors });

        return Ok(result.Data);
    }
}