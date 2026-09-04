using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.Mappings;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class ClaimReviewService : IClaimReviewService
    {
        private readonly IClaimReviewRepository _claimReviewRepository;
        private readonly IClaimRepository _claimRepository;

        public ClaimReviewService(IClaimReviewRepository claimReviewRepository, IClaimRepository claimRepository)
        {
            _claimReviewRepository = claimReviewRepository;
            _claimRepository = claimRepository;
        }

        public async Task<Result<ClaimReviewResponseDto>> CreateAsync(
            Guid pharmacyId, Guid claimId, Guid reviewerUserId, CreateClaimReviewRequestDto request, CancellationToken cancellationToken = default)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId, cancellationToken);

            // إضافة فحص الملكية
            if (claim is null || claim.PharmacyId != pharmacyId)
                return Result<ClaimReviewResponseDto>.Failure("المطالبة غير موجودة.");

            var existingReview = await _claimReviewRepository.GetByClaimIdAsync(claimId, cancellationToken);
            if (existingReview is not null)
                return Result<ClaimReviewResponseDto>.Failure("تم إضافة مراجعة لهذه المطالبة من قبل.");

            var validation = ValidateReviewInput(request.IsAccurate, request.CorrectedAmount, request.DiscrepancyType, out var discrepancyType);
            if (!validation.Succeeded)
                return Result<ClaimReviewResponseDto>.Failure(validation.Errors);

            var review = new ClaimReview
            {
                ClaimId = claimId,
                ReviewedByUserId = reviewerUserId,
                IsAccurate = request.IsAccurate,
                CorrectedAmount = request.IsAccurate ? null : request.CorrectedAmount,
                DiscrepancyType = request.IsAccurate ? DiscrepancyType.None : discrepancyType,
                Notes = request.Notes,
                WasEditedByPharmacy = false
            };

            await _claimReviewRepository.AddAsync(review, cancellationToken);

            claim.CorrectedAmount = request.IsAccurate ? claim.ClaimAmountAfterDiscount : request.CorrectedAmount!.Value;
            claim.Status = ClaimStatus.Reviewed;
            _claimRepository.Update(claim);

            await _claimReviewRepository.SaveChangesAsync(cancellationToken);

            return Result<ClaimReviewResponseDto>.Success(review.ToResponseDto());
        }

        public async Task<Result<ClaimReviewResponseDto>> UpdateAsync(
            Guid pharmacyId, Guid claimId, UpdateClaimReviewRequestDto request, CancellationToken cancellationToken = default)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim is null || claim.PharmacyId != pharmacyId)
                return Result<ClaimReviewResponseDto>.Failure("المطالبة غير موجودة.");

            var review = await _claimReviewRepository.GetByClaimIdAsync(claimId, cancellationToken);
            if (review is null)
                return Result<ClaimReviewResponseDto>.Failure("لا توجد مراجعة لهذه المطالبة بعد.");

            var validation = ValidateReviewInput(request.IsAccurate, request.CorrectedAmount, request.DiscrepancyType, out var discrepancyType);
            if (!validation.Succeeded)
                return Result<ClaimReviewResponseDto>.Failure(validation.Errors);

            review.IsAccurate = request.IsAccurate;
            review.CorrectedAmount = request.IsAccurate ? null : request.CorrectedAmount;
            review.DiscrepancyType = request.IsAccurate ? DiscrepancyType.None : discrepancyType;
            review.Notes = request.Notes;
            review.WasEditedByPharmacy = true;
            review.LastEditedAt = DateTime.UtcNow;

            _claimReviewRepository.Update(review);

            claim.CorrectedAmount = request.IsAccurate ? claim.ClaimAmountAfterDiscount : request.CorrectedAmount!.Value;
            claim.Status = ClaimStatus.EditedAfterReview;
            _claimRepository.Update(claim);

            await _claimReviewRepository.SaveChangesAsync(cancellationToken);

            return Result<ClaimReviewResponseDto>.Success(review.ToResponseDto());
        }

        public async Task<Result<ClaimReviewResponseDto>> GetByClaimIdAsync(Guid pharmacyId, Guid claimId, CancellationToken cancellationToken = default)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim is null || claim.PharmacyId != pharmacyId)
                return Result<ClaimReviewResponseDto>.Failure("المطالبة غير موجودة.");

            var review = await _claimReviewRepository.GetByClaimIdAsync(claimId, cancellationToken);
            if (review is null)
                return Result<ClaimReviewResponseDto>.Failure("لا توجد مراجعة لهذه المطالبة.");

            return Result<ClaimReviewResponseDto>.Success(review.ToResponseDto());
        }

        private static Result ValidateReviewInput(bool isAccurate, decimal? correctedAmount, string? discrepancyTypeRaw, out DiscrepancyType discrepancyType)
        {
            discrepancyType = DiscrepancyType.None;

            if (isAccurate)
                return Result.Success();

            if (!correctedAmount.HasValue)
                return Result.Failure("يجب إدخال المبلغ الصحيح عند الإشارة إلى وجود خطأ في المطالبة.");

            if (string.IsNullOrWhiteSpace(discrepancyTypeRaw) || !Enum.TryParse(discrepancyTypeRaw, ignoreCase: true, out discrepancyType) || discrepancyType == DiscrepancyType.None)
                return Result.Failure("يجب تحديد سبب التباين.");

            return Result.Success();
        }
    }
}