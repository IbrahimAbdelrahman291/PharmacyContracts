using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Claims.Api.Controllers;

[ApiController]
[Route("api/v1/cosmetics")]
[Authorize(Roles = "Pharmacy")]
public class CosmeticsLedgerController : ControllerBase
{
    private readonly ICosmeticsLedgerService _service;
    private readonly ICurrentUserService _currentUser;
    public CosmeticsLedgerController(ICosmeticsLedgerService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetCompanies(CancellationToken cancellationToken) =>
        Ok((await _service.GetCompaniesAsync(PharmacyId, cancellationToken)).Data);

    [HttpPost("companies")]
    public async Task<IActionResult> CreateCompany(CosmeticsLookupRequestDto request, CancellationToken cancellationToken) =>
        ToActionResult(await _service.CreateCompanyAsync(PharmacyId, request, cancellationToken));

    [HttpPut("companies/{id:guid}")]
    public async Task<IActionResult> UpdateCompany(Guid id, CosmeticsLookupRequestDto request, CancellationToken cancellationToken) =>
        ToActionResult(await _service.UpdateCompanyAsync(PharmacyId, id, request, cancellationToken));

    [HttpDelete("companies/{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id, CancellationToken cancellationToken) =>
        ToActionResult(await _service.DeleteCompanyAsync(PharmacyId, id, cancellationToken));

    [HttpGet("names")]
    public async Task<IActionResult> GetNames(CancellationToken cancellationToken) =>
        Ok((await _service.GetNamesAsync(PharmacyId, cancellationToken)).Data);

    [HttpPost("names")]
    public async Task<IActionResult> CreateName(CosmeticsLookupRequestDto request, CancellationToken cancellationToken) =>
        ToActionResult(await _service.CreateNameAsync(PharmacyId, request, cancellationToken));

    [HttpPut("names/{id:guid}")]
    public async Task<IActionResult> UpdateName(Guid id, CosmeticsLookupRequestDto request, CancellationToken cancellationToken) =>
        ToActionResult(await _service.UpdateNameAsync(PharmacyId, id, request, cancellationToken));

    [HttpDelete("names/{id:guid}")]
    public async Task<IActionResult> DeleteName(Guid id, CancellationToken cancellationToken) =>
        ToActionResult(await _service.DeleteNameAsync(PharmacyId, id, cancellationToken));

    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction(CosmeticsTransactionRequestDto request, CancellationToken cancellationToken) =>
        ToActionResult(await _service.CreateTransactionAsync(PharmacyId, request, cancellationToken));

    [HttpPut("transactions/{id:guid}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, CosmeticsTransactionRequestDto request, CancellationToken cancellationToken) =>
        ToActionResult(await _service.UpdateTransactionAsync(PharmacyId, id, request, cancellationToken));

    [HttpDelete("transactions/{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id, CancellationToken cancellationToken) =>
        ToActionResult(await _service.DeleteTransactionAsync(PharmacyId, id, cancellationToken));

    [HttpGet("reports")]
    public async Task<IActionResult> GetReport(
        [FromQuery] Guid? companyId, [FromQuery] Guid? nameId, [FromQuery] string? name,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken) =>
        ToActionResult(await _service.GetReportAsync(PharmacyId, companyId, nameId, name, from, to, cancellationToken));

    private Guid PharmacyId => _currentUser.EffectivePharmacyId!.Value;

    private IActionResult ToActionResult<T>(PharmacyContracts.SharedKernel.Wrappers.Result<T> result) =>
        result.Succeeded ? Ok(result.Data) : BadRequest(new { errors = result.Errors });
}
