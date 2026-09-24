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

            var validation = ValidateReviewInput(claim, request.IsAccurate, request.CorrectedAmount,
                request.CorrectedPrescriptionsCount, request.Differences, out var differenceAmount,
                out var differenceType, out var parsedDifferences);
            if (!validation.Succeeded)
                return Result<ClaimReviewResponseDto>.Failure(validation.Errors);

            var review = new ClaimReview
            {
                ClaimId = claimId,
                ReviewedByUserId = reviewerUserId,
                IsAccurate = request.IsAccurate,
                CorrectedAmount = request.IsAccurate ? null : request.CorrectedAmount,
                CorrectedPrescriptionsCount = request.IsAccurate ? null : request.CorrectedPrescriptionsCount,
                DifferenceAmount = differenceAmount,
                DifferenceType = differenceType,
                WasEditedByPharmacy = false,
                Differences = parsedDifferences.Select(d => new ClaimReviewDifference
                {
                    Value = d.Value,
                    Reason = d.Reason,
                    Notes = d.Notes,
                    PharmacyId = pharmacyId
                }).ToList()
            };

            await _claimReviewRepository.AddAsync(review, cancellationToken);

            claim.CorrectedAmount = request.IsAccurate ? claim.ClaimAmountAfterDiscount : request.CorrectedAmount!.Value;
            claim.CorrectedPrescriptionsCount = request.IsAccurate
                ? claim.PrescriptionsCount
                : request.CorrectedPrescriptionsCount!.Value;
            claim.Status = ClaimStatus.Reviewed;
            _claimRepository.Update(claim);

            await _claimReviewRepository.SaveChangesAsync(cancellationToken);

            return Result<ClaimReviewResponseDto>.Success(review.ToResponseDto(claim));
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

            var validation = ValidateReviewInput(claim, request.IsAccurate, request.CorrectedAmount,
                request.CorrectedPrescriptionsCount, request.Differences, out var differenceAmount,
                out var differenceType, out var parsedDifferences);
            if (!validation.Succeeded)
                return Result<ClaimReviewResponseDto>.Failure(validation.Errors);

            review.IsAccurate = request.IsAccurate;
            review.CorrectedAmount = request.IsAccurate ? null : request.CorrectedAmount;
            review.CorrectedPrescriptionsCount = request.IsAccurate ? null : request.CorrectedPrescriptionsCount;
            review.DifferenceAmount = differenceAmount;
            review.DifferenceType = differenceType;
            review.WasEditedByPharmacy = true;
            review.LastEditedAt = DateTime.UtcNow;
            review.Differences.Clear();
            foreach (var difference in parsedDifferences)
            {
                review.Differences.Add(new ClaimReviewDifference
                {
                    ReviewId = review.Id,
                    PharmacyId = pharmacyId,
                    Value = difference.Value,
                    Reason = difference.Reason,
                    Notes = difference.Notes
                });
            }

            _claimReviewRepository.Update(review);

            claim.CorrectedAmount = request.IsAccurate ? claim.ClaimAmountAfterDiscount : request.CorrectedAmount!.Value;
            claim.CorrectedPrescriptionsCount = request.IsAccurate
                ? claim.PrescriptionsCount
                : request.CorrectedPrescriptionsCount!.Value;
            claim.Status = ClaimStatus.EditedAfterReview;
            _claimRepository.Update(claim);

            await _claimReviewRepository.SaveChangesAsync(cancellationToken);

            return Result<ClaimReviewResponseDto>.Success(review.ToResponseDto(claim));
        }

        public async Task<Result<ClaimReviewResponseDto>> GetByClaimIdAsync(Guid pharmacyId, Guid claimId, CancellationToken cancellationToken = default)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim is null || claim.PharmacyId != pharmacyId)
                return Result<ClaimReviewResponseDto>.Failure("المطالبة غير موجودة.");

            var review = await _claimReviewRepository.GetByClaimIdAsync(claimId, cancellationToken);
            if (review is null)
                return Result<ClaimReviewResponseDto>.Failure("لا توجد مراجعة لهذه المطالبة.");

            return Result<ClaimReviewResponseDto>.Success(review.ToResponseDto(claim));
        }

        public async Task<Result<ClaimReviewDifferencesResponseDto>> GetDifferencesByClaimIdAsync(
            Guid pharmacyId, Guid claimId, CancellationToken cancellationToken = default)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim is null || claim.PharmacyId != pharmacyId)
                return Result<ClaimReviewDifferencesResponseDto>.Failure("المطالبة غير موجودة.");

            var review = await _claimReviewRepository.GetByClaimIdAsync(claimId, cancellationToken);
            if (review is null)
                return Result<ClaimReviewDifferencesResponseDto>.Failure("لا توجد مراجعة لهذه المطالبة.");

            var reviewDto = review.ToResponseDto(claim);
            return Result<ClaimReviewDifferencesResponseDto>.Success(new ClaimReviewDifferencesResponseDto
            {
                ClaimId = claimId,
                ReviewId = review.Id,
                AmountBeforeDiscount = reviewDto.AmountBeforeDiscount,
                CorrectedAmount = reviewDto.CorrectedAmount,
                AmountDifference = reviewDto.AmountDifference,
                AmountDifferenceType = reviewDto.AmountDifferenceType,
                PrescriptionsCount = reviewDto.PrescriptionsCount,
                CorrectedPrescriptionsCount = reviewDto.CorrectedPrescriptionsCount,
                PrescriptionsCountDifference = reviewDto.PrescriptionsCountDifference,
                PrescriptionsCountDifferenceType = reviewDto.PrescriptionsCountDifferenceType,
                DifferenceAmount = review.DifferenceAmount,
                DifferenceType = review.DifferenceType.ToString(),
                Differences = reviewDto.Differences
            });
        }

        private static Result ValidateReviewInput(Claim claim, bool isAccurate, decimal? correctedAmount,
            int? correctedPrescriptionsCount, IReadOnlyCollection<ClaimReviewDifferenceRequestDto>? differences,
            out decimal differenceAmount, out DifferenceType differenceType,
            out List<(decimal Value, DifferenceReason Reason, string? Notes)> parsedDifferences)
        {
            differenceAmount = 0;
            differenceType = DifferenceType.NoDifference;
            parsedDifferences = [];

            if (isAccurate)
            {
                return differences is null || differences.Count == 0
                    ? Result.Success()
                    : Result.Failure("لا يمكن إضافة فروقات عندما تكون المطالبة صحيحة.");
            }

            if (!correctedAmount.HasValue)
                return Result.Failure("يجب إدخال المبلغ الصحيح عند الإشارة إلى وجود خطأ في المطالبة.");

            if (correctedAmount.Value < 0)
                return Result.Failure("لا يمكن أن يكون المبلغ الصحيح أقل من صفر.");

            if (!correctedPrescriptionsCount.HasValue)
                return Result.Failure("يجب إدخال العدد الصحيح للوصفات عند الإشارة إلى وجود خطأ في المطالبة.");

            if (correctedPrescriptionsCount.Value < 0)
                return Result.Failure("لا يمكن أن يكون العدد الصحيح للوصفات أقل من صفر.");

            differenceAmount = Math.Abs(correctedAmount.Value - claim.ClaimAmount);
            differenceType = correctedAmount.Value > claim.ClaimAmount
                ? DifferenceType.Increase
                : correctedAmount.Value < claim.ClaimAmount
                    ? DifferenceType.Decrease
                    : DifferenceType.NoDifference;

            differences ??= [];
            foreach (var difference in differences)
            {
                if (difference.Value <= 0)
                    return Result.Failure("يجب أن تكون قيمة كل فرق أكبر من صفر.");

                if (!Enum.TryParse<DifferenceReason>(difference.Reason, ignoreCase: true, out var reason)
                    || !Enum.IsDefined(reason))
                    return Result.Failure($"سبب الفرق '{difference.Reason}' غير صالح.");

                parsedDifferences.Add((difference.Value, reason, difference.Notes));
            }

            if (parsedDifferences.Sum(d => d.Value) < differenceAmount)
                return Result.Failure($"يجب ألا يقل مجموع قيم الفروقات عن مبلغ الفرق ({differenceAmount}).");

            return Result.Success();
        }
    }
}
