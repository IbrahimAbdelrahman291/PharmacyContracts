using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.Services;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Api.Controllers;

[ApiController]
[Route("api/v1/claims")]
[Authorize(Roles = "Pharmacy")]
public class ClaimsController : ControllerBase
{
    private readonly IBranchService _branchService;
    private readonly IClaimsPivotService _pivotService;
    private readonly ICompanyInsightsService _companyInsightsService;
    private readonly ICompanyProfileService _companyProfileService;
    private readonly ICurrentUserService _currentUserService;

    public ClaimsController(
        IBranchService branchService,
        IClaimsPivotService pivotService,
        ICompanyInsightsService companyInsightsService,
        ICompanyProfileService companyProfileService,
        ICurrentUserService currentUserService)
    {
        _branchService = branchService;
        _pivotService = pivotService;
        _companyInsightsService = companyInsightsService;
        _companyProfileService = companyProfileService;
        _currentUserService = currentUserService;
    }

    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _branchService.GetBranchesAsync(pharmacyId, cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("pivot")]
    public async Task<IActionResult> GetPivot([FromQuery] int month, [FromQuery] int year, CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _pivotService.GetPivotAsync(pharmacyId, month, year, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("company-insights")]
    public async Task<IActionResult> GetCompanyInsights(
        [FromQuery] string companyName,
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _companyInsightsService.GetInsightsAsync(pharmacyId, companyName, month, year, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
    [HttpGet("company-profile")]
    public async Task<IActionResult> GetCompanyProfile(
       [FromQuery] string companyName,
       [FromQuery] int? month,
       [FromQuery] int? year,
       [FromQuery] PaginationParams pagination,
       CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _companyProfileService.GetProfileAsync(pharmacyId, companyName, month, year, pagination, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
}