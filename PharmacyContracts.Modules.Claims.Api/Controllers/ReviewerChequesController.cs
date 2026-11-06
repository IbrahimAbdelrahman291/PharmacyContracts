using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Api.Controllers;

[ApiController]
[Route("api/v1/reviewer/cheques")]
[Authorize(Roles = "ClaimsReviewer")]
public class ReviewerChequesController : ControllerBase
{
    private readonly IChequeService _chequeService;
    private readonly ICurrentUserService _currentUserService;

    public ReviewerChequesController(IChequeService chequeService, ICurrentUserService currentUserService)
    {
        _chequeService = chequeService;
        _currentUserService = currentUserService;
    }

    [HttpGet("returned")]
    public async Task<IActionResult> GetReturnedCheques(CancellationToken cancellationToken)
    {
        if (_currentUserService.EffectivePharmacyId is not Guid pharmacyId)
            return Forbid();

        var result = await _chequeService.GetAsync(
            pharmacyId, null, null, null, "ReturnedToReviewer", null, null, cancellationToken);

        return Ok(result.Data);
    }

    [HttpPatch("{id:guid}/review")]
    public async Task<IActionResult> ReviewReturnedCheque(
        Guid id,
        [FromBody] ReviewReturnedChequeRequestDto request,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.EffectivePharmacyId is not Guid pharmacyId)
            return Forbid();

        var result = await _chequeService.ReviewReturnedChequeAsync(
            pharmacyId, id, request, cancellationToken);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
}
