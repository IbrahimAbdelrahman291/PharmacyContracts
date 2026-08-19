using PharmacyContracts.Modules.Auth.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Auth.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<UserResponseDto>> CreateReviewerAsync(Guid pharmacyId, CreateReviewerRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<List<UserResponseDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result> UpdateStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
    }
}
