using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Api.Controllers;

[ApiController]
[Route("api/v1/company-profile")]
[Authorize(Roles = "Pharmacy")]
public class CompanyProfileController : ControllerBase
{
    private readonly ICompanyProfileService _companyProfileService;
    private readonly ICurrentUserService _currentUserService;

    public CompanyProfileController(
        ICompanyProfileService companyProfileService,
        ICurrentUserService currentUserService)
    {
        _companyProfileService = companyProfileService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string companyName,
        [FromQuery] int? month,
        [FromQuery] int? year,
        [FromQuery] PaginationParams pagination,
        CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _companyProfileService.GetProfileAsync(
            pharmacyId, companyName, month, year, pagination, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
}
