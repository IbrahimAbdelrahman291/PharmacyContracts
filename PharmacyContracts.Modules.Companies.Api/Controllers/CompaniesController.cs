using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Companies.Api.Controllers;

[ApiController]
[Route("api/v1/companies")]
[Authorize(Roles = "Pharmacy")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateCompanyRequestDto> _createValidator;
    private readonly IValidator<UpdateCompanyRequestDto> _updateValidator;

    public CompaniesController(
        ICompanyService companyService,
        ICurrentUserService currentUserService,
        IValidator<CreateCompanyRequestDto> createValidator,
        IValidator<UpdateCompanyRequestDto> updateValidator)
    {
        _companyService = companyService;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequestDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        var pharmacyId = _currentUserService.UserId!.Value;
        var result = await _companyService.CreateAsync(pharmacyId, request, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyRequestDto request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        var pharmacyId = _currentUserService.UserId!.Value;
        var result = await _companyService.UpdateAsync(pharmacyId, id, request, cancellationToken);
        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.UserId!.Value;
        var result = await _companyService.GetPagedAsync(pharmacyId, pagination, cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.UserId!.Value;
        var result = await _companyService.GetByIdAsync(pharmacyId, id, cancellationToken);
        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
}