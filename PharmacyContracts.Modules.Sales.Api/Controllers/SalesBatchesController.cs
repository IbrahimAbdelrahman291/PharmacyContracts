// Controllers/SalesBatchesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Sales.Api.Controllers;

[ApiController]
[Authorize(Roles = "Pharmacy")]
public class SalesBatchesController : ControllerBase
{
    private readonly ISalesUploadService _salesUploadService;
    private readonly ICurrentUserService _currentUserService;

    public SalesBatchesController(ISalesUploadService salesUploadService, ICurrentUserService currentUserService)
    {
        _salesUploadService = salesUploadService;
        _currentUserService = currentUserService;
    }

    [HttpPost("api/v1/pharmacies/{pharmacyId:guid}/sales-batches")]
    public async Task<IActionResult> Upload(Guid pharmacyId, IFormFile file, CancellationToken cancellationToken)
    {
        // تأكيد إضافي: الصيدلية اللي عاملة login لازم تكون هي نفسها اللي بترفع الملف بتاعها
        var currentUserId = _currentUserService.UserId!.Value;
        if (currentUserId != pharmacyId)
            return Forbid();

        if (file is null || file.Length == 0)
            return BadRequest(new { errors = new[] { "يجب اختيار ملف." } });

        var result = await _salesUploadService.UploadAsync(pharmacyId, currentUserId, file, cancellationToken);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Accepted(new { batchId = result.Data!.BatchId, status = result.Data.Status });
    }

    [HttpGet("api/v1/sales-batches/{id:guid}")]
    public async Task<IActionResult> GetStatus(Guid id, CancellationToken cancellationToken)
    {
        var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
        var result = await _salesUploadService.GetStatusAsync(pharmacyId, id, cancellationToken);

        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
}