using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyContracts.Modules.Companies.Application.DTOs;
using PharmacyContracts.Modules.Companies.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;

namespace PharmacyContracts.Modules.Companies.Api.Controllers
{
    [ApiController]
    [Route("api/v1/companies/{companyId:guid}/departments")]
    [Authorize(Roles = "Pharmacy")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ICurrentUserService _currentUserService;

        public DepartmentsController(IDepartmentService departmentService, ICurrentUserService currentUserService)
        {
            _departmentService = departmentService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid companyId, [FromBody] CreateDepartmentRequestDto request, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _departmentService.CreateAsync(pharmacyId, companyId, request, cancellationToken);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid companyId, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _departmentService.GetByCompanyIdAsync(pharmacyId, companyId, cancellationToken);

            if (!result.Succeeded)
                return NotFound(new { errors = result.Errors });

            return Ok(result.Data);
        }

        [HttpDelete("{departmentId:guid}")]
        public async Task<IActionResult> Delete(Guid companyId, Guid departmentId, CancellationToken cancellationToken)
        {
            var pharmacyId = _currentUserService.EffectivePharmacyId!.Value;
            var result = await _departmentService.DeleteAsync(pharmacyId, companyId, departmentId, cancellationToken);

            if (!result.Succeeded)
                return NotFound(new { errors = result.Errors });

            return NoContent();
        }
    }
}