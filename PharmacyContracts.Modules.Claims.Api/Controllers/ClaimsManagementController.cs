using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Api.Controllers
{
    [ApiController]
    [Route("api/v1/claims")]
    public class ClaimsManagementController : ControllerBase
    {
        private readonly IClaimGenerationService _claimGenerationService;
        private readonly IClaimService _claimService;
        private readonly IClaimReviewService _claimReviewService;
        private readonly ICurrentUserService _currentUserService;

        public ClaimsManagementController(
            IClaimGenerationService claimGenerationService,
            IClaimService claimService,
            IClaimReviewService claimReviewService,
            ICurrentUserService currentUserService)
        {
            _claimGenerationService = claimGenerationService;
            _claimService = claimService;
            _claimReviewService = claimReviewService;
            _currentUserService = currentUserService;
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> Generate([FromBody] GenerateClaimsRequestDto request, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _claimGenerationService.GenerateAsync(pharmacyId, request.Month, request.Year, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Pharmacy,ClaimsReviewer")]
        public async Task<IActionResult> GetClaims([FromQuery] int? month, [FromQuery] int? year, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _claimService.GetByPeriodAsync(pharmacyId, month, year, cancellationToken);
            return Ok(result.Data);
        }

        [HttpPost("{id:guid}/reviews")]
        [Authorize(Roles = "ClaimsReviewer")]
        public async Task<IActionResult> CreateReview(Guid id, [FromBody] CreateClaimReviewRequestDto request, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;   // ← إضافة
            var reviewerUserId = _currentUserService.UserId!.Value;
            var result = await _claimReviewService.CreateAsync(pharmacyId, id, reviewerUserId, request, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpPut("{id:guid}/reviews")]
        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateClaimReviewRequestDto request, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _claimReviewService.UpdateAsync(pharmacyId, id, request, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpGet("{id:guid}/reviews")]
        [Authorize(Roles = "Pharmacy,ClaimsReviewer")]
        public async Task<IActionResult> GetReview(Guid id, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;   // ← إضافة
            var result = await _claimReviewService.GetByClaimIdAsync(pharmacyId, id, cancellationToken);

            if (!result.Succeeded)
                return NotFound(new { errors = result.Errors });

            return Ok(result.Data);
        }
    }
}