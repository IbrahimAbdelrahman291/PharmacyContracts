using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/users")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequestDto> _createValidator;
    private readonly ICurrentUserService _currentUserService;   // ← جديد

    public UsersController(
        IUserService userService,
        IValidator<CreateUserRequestDto> createValidator,
        ICurrentUserService currentUserService)   // ← جديد
    {
        _userService = userService;
        _createValidator = createValidator;
        _currentUserService = currentUserService;   // ← جديد
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        var result = await _userService.CreateUserAsync(request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetAll), null, result.Data);
    }

    [HttpPost("reviewers")]
    [Authorize(Roles = "Pharmacy")]
    public async Task<IActionResult> CreateReviewer([FromBody] CreateReviewerRequestDto request, CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _userService.CreateReviewerAsync(pharmacyId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetAll), null, result.Data);
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateStatusAsync(id, request.IsActive, cancellationToken);
        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return NoContent();
    }
}