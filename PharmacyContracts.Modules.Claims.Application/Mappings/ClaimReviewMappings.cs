using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Application.Mappings
{
    public static class ClaimReviewMappings
    {
        public static ClaimReviewResponseDto ToResponseDto(this ClaimReview review, Claim claim)
        {
            var correctedAmount = review.CorrectedAmount ?? claim.SubTotal;
            var correctedPrescriptionsCount = review.CorrectedPrescriptionsCount ?? claim.PrescriptionsCount;

            return new ClaimReviewResponseDto
            {
                Id = review.Id,
                ClaimId = review.ClaimId,
                ReviewedByUserId = review.ReviewedByUserId,
                IsAccurate = review.IsAccurate,
                CorrectedAmount = review.CorrectedAmount,
                CorrectedPrescriptionsCount = review.CorrectedPrescriptionsCount,
                AmountBeforeDiscount = claim.SubTotal,
                AmountDifference = Math.Abs(correctedAmount - claim.SubTotal),
                AmountDifferenceType = GetDifferenceType(correctedAmount, claim.SubTotal),
                PrescriptionsCount = claim.PrescriptionsCount,
                PrescriptionsCountDifference = Math.Abs(correctedPrescriptionsCount - claim.PrescriptionsCount),
                PrescriptionsCountDifferenceType = GetDifferenceType(correctedPrescriptionsCount, claim.PrescriptionsCount),
                DifferenceAmount = review.DifferenceAmount,
                DifferenceType = review.DifferenceType.ToString(),
                Differences = review.Differences.Select(d => new ClaimReviewDifferenceResponseDto
                {
                    Id = d.Id,
                    Value = d.Value,
                    Reason = d.Reason.ToString(),
                    Notes = d.Notes,
                    EmployeeName = d.EmployeeName,
                    PharmacyName = d.PharmacyName,
                    ReviewId = d.ReviewId,
                    PharmacyId = d.PharmacyId
                }).ToList(),
                WasEditedByPharmacy = review.WasEditedByPharmacy,
                CreatedAt = review.CreatedAt,
                LastEditedAt = review.LastEditedAt
            };
        }

        private static string GetDifferenceType(decimal correctedValue, decimal originalValue)
        {
            return correctedValue > originalValue
                ? "Increase"
                : correctedValue < originalValue
                    ? "Decrease"
                    : "NoDifference";
        }
    }
}
