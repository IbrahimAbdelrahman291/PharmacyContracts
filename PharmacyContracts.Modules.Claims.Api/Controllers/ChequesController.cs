using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Api.Controllers
{
    [ApiController]
    [Route("api/v1/cheques")]
    [Authorize(Roles = "Pharmacy")]
    public class ChequesController : ControllerBase
    {
        private readonly IChequeService _chequeService;
        private readonly ICurrentUserService _currentUserService;

        public ChequesController(IChequeService chequeService, ICurrentUserService currentUserService)
        {
            _chequeService = chequeService;
            _currentUserService = currentUserService;
        }

        [HttpGet("prepare")]
        public async Task<IActionResult> Prepare([FromQuery] string companyName, [FromQuery] int month, [FromQuery] int year, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _chequeService.PrepareAsync(pharmacyId, companyName, month, year, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpPost("claims/{claimId:guid}")]
        public async Task<IActionResult> Create(Guid claimId, [FromBody] CreateChequesRequestDto request, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _chequeService.CreateAsync(pharmacyId, claimId, request, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? companyName, [FromQuery] int? month, [FromQuery] int? year, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _chequeService.GetAsync(pharmacyId, companyName, month, year, cancellationToken);
            return Ok(result.Data);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateChequeStatusRequestDto request, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _chequeService.UpdateStatusAsync(pharmacyId, id, request, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return NoContent();
        }
    }
}