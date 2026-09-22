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
                CorrectedPrescriptionsCount = review.CorrectedPrescriptionsCount,
                DifferenceAmount = review.DifferenceAmount,
                DifferenceType = review.DifferenceType.ToString(),
                Differences = review.Differences.Select(d => new ClaimReviewDifferenceResponseDto
                {
                    Id = d.Id,
                    Value = d.Value,
                    Reason = d.Reason.ToString(),
                    ReviewId = d.ReviewId,
                    PharmacyId = d.PharmacyId
                }).ToList(),
                Notes = review.Notes,
                WasEditedByPharmacy = review.WasEditedByPharmacy,
                CreatedAt = review.CreatedAt,
                LastEditedAt = review.LastEditedAt
            };
        }
    }
}
