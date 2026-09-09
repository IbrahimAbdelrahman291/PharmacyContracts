using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Api.Controllers;

[ApiController]
[Route("api/v1/claims/reports")]
[Authorize(Roles = "Pharmacy")]
public class ClaimsReportsController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    private readonly ICurrentUserService _currentUserService;

    public ClaimsReportsController(IBalanceService balanceService, ICurrentUserService currentUserService)
    {
        _balanceService = balanceService;
        _currentUserService = currentUserService;
    }

    [HttpGet("company-balance")]
    public async Task<IActionResult> GetCompanyBalance(
        [FromQuery] string companyName,
        CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _balanceService.GetCompanyBalanceAsync(pharmacyId, companyName, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("total-balance")]
    public async Task<IActionResult> GetTotalBalance(CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _balanceService.GetTotalBalanceAsync(pharmacyId, cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("aging")]
    public async Task<IActionResult> GetAgingReport(
        [FromQuery] string? companyName,
        CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _balanceService.GetAgingReportAsync(pharmacyId, companyName, cancellationToken);
        return Ok(result.Data);
    }

    [HttpGet("top-debtors")]
    public async Task<IActionResult> GetTopDebtors(
        [FromQuery] int top = 10,
        CancellationToken cancellationToken = default)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _balanceService.GetTopDebtorsAsync(pharmacyId, top, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
}
