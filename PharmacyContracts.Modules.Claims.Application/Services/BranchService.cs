using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class BranchService : IBranchService
    {
        private readonly ISalesQueryService _salesQueryService;
        public BranchService(ISalesQueryService salesQueryService) => _salesQueryService = salesQueryService;

        public async Task<Result<List<BranchResponseDto>>> GetBranchesAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
        {
            var branches = await _salesQueryService.GetDistinctBranchesAsync(pharmacyId, cancellationToken);

            var response = branches.Select(b => new BranchResponseDto { Name = b }).ToList();
            return Result<List<BranchResponseDto>>.Success(response);
        }
    }
}
