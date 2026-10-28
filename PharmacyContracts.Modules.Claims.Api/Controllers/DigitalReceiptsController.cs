using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Api.Controllers;

[ApiController]
[Route("api/v1/digital-receipts")]
[Authorize(Roles = "ClaimsReviewer,Pharmacy")]
public class DigitalReceiptsController : ControllerBase
{
    private const long MaxPdfSize = 2 * 1024 * 1024;
    private readonly IDigitalReceiptService _service;
    private readonly ICurrentUserService _currentUser;
    public DigitalReceiptsController(IDigitalReceiptService service, ICurrentUserService currentUser) { _service = service; _currentUser = currentUser; }

    [HttpPost, Authorize(Roles = "ClaimsReviewer")]
    public async Task<IActionResult> Create(CreateDigitalReceiptRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(PharmacyId, request, cancellationToken);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created, result.Data) : BadRequest(new { errors = result.Errors });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) => Ok((await _service.GetAllAsync(PharmacyId, cancellationToken)).Data);

    [HttpPost("{id:guid}/file"), Authorize(Roles = "Pharmacy"), RequestSizeLimit(MaxPdfSize + 65536), Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(Guid id, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0) return BadRequest(new { errors = new[] { "A PDF file is required." } });
        if (file.Length > MaxPdfSize) return BadRequest(new { errors = new[] { "The PDF file cannot exceed 2 MB." } });
        if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase) || !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)) return BadRequest(new { errors = new[] { "Only PDF files are allowed." } });
        await using var stream = file.OpenReadStream();
        var header = new byte[5];
        if (await stream.ReadAsync(header, cancellationToken) != 5 || !header.SequenceEqual("%PDF-"u8.ToArray())) return BadRequest(new { errors = new[] { "The uploaded file is not a valid PDF." } });
        stream.Position = 0;
        var result = await _service.UploadAsync(PharmacyId, id, stream, file.FileName, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : BadRequest(new { errors = result.Errors });
    }

    [HttpGet("{id:guid}/file")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.DownloadAsync(PharmacyId, id, cancellationToken);
        return result.Succeeded ? File(result.Data!.Content, result.Data.ContentType, result.Data.FileName) : NotFound(new { errors = result.Errors });
    }

    [HttpDelete("{id:guid}"), Authorize(Roles = "ClaimsReviewer")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _service.CancelAsync(PharmacyId, id, cancellationToken);
        return result.Succeeded ? NoContent() : BadRequest(new { errors = result.Errors });
    }

    private Guid PharmacyId => _currentUser.EffectivePharmacyId ?? throw new UnauthorizedAccessException("The token does not contain a pharmacy id.");
}
