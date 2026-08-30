using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Application.Mappings
{
    public static class ClaimReviewMappings
    {
        public static ClaimReviewResponseDto ToResponseDto(this ClaimReview review)
        {
            return new ClaimReviewResponseDto
            {
                Id = review.Id,
                ClaimId = review.ClaimId,
                ReviewedByUserId = review.ReviewedByUserId,
                IsAccurate = review.IsAccurate,
                CorrectedAmount = review.CorrectedAmount,
                DiscrepancyType = review.DiscrepancyType.ToString(),
                Notes = review.Notes,
                WasEditedByPharmacy = review.WasEditedByPharmacy,
                CreatedAt = review.CreatedAt,
                LastEditedAt = review.LastEditedAt
            };
        }
    }
}
