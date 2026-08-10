// Controllers/UsersController.cs
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.Modules.Auth.Application.Interfaces;

namespace PharmacyContracts.Modules.Auth.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize(Roles = "SuperAdmin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequestDto> _createValidator;

    public UsersController(IUserService userService, IValidator<CreateUserRequestDto> createValidator)
    {
        _userService = userService;
        _createValidator = createValidator;
    }

    [HttpPost]
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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateStatusAsync(id, request.IsActive, cancellationToken);
        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return NoContent();
    }
}