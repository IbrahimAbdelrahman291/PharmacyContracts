using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Interfaces
{
    public interface IBranchService
    {
        Task<Result<List<BranchResponseDto>>> GetBranchesAsync(Guid pharmacyId, CancellationToken cancellationToken = default);
    }
}
