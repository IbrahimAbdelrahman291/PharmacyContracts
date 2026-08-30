using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Application.Mappings
{
    public static class ClaimMappings
    {
        public static ClaimResponseDto ToResponseDto(this Claim claim)
        {
            return new ClaimResponseDto
            {
                Id = claim.Id,
                CompanyName = claim.CompanyName,
                Month = claim.Month,
                Year = claim.Year,
                ClaimAmountAfterDiscount = claim.ClaimAmountAfterDiscount,
                CorrectedAmount = claim.CorrectedAmount,
                Status = claim.Status.ToString(),
                CreatedAt = claim.CreatedAt
            };
        }
    }
}
