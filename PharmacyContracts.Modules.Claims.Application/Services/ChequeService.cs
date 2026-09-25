using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Application.Mappings;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services
{
    public class ChequeService : IChequeService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IChequeRepository _chequeRepository;
        private readonly ICompaniesQueryService _companiesQueryService;

        public ChequeService(
            IClaimRepository claimRepository,
            IChequeRepository chequeRepository,
            ICompaniesQueryService companiesQueryService)
        {
            _claimRepository = claimRepository;
            _chequeRepository = chequeRepository;
            _companiesQueryService = companiesQueryService;
        }

        public async Task<Result<ChequeCreationPreparationDto>> PrepareAsync(
            Guid pharmacyId, string companyName, int month, int year, CancellationToken cancellationToken = default)
        {
            var claims = await _claimRepository.GetByPeriodAsync(pharmacyId, month, year, cancellationToken);
            var claim = claims.FirstOrDefault(c => c.CompanyName == companyName);

            if (claim is null)
                return Result<ChequeCreationPreparationDto>.Failure("لا توجد مطالبة لهذه الشركة في هذا الشهر.");

            if (!claim.CorrectedAmount.HasValue)
                return Result<ChequeCreationPreparationDto>.Failure("لم تتم مراجعة هذه المطالبة بعد.");

            var alreadyHasCheques = await _chequeRepository.ExistsForClaimAsync(claim.Id, cancellationToken);
            if (alreadyHasCheques)
                return Result<ChequeCreationPreparationDto>.Failure("تم إنشاء شيكات لهذه المطالبة بالفعل.");

            var settlementDays = await _companiesQueryService.GetChequeSettlementPeriodInDaysAsync(pharmacyId, companyName, cancellationToken);
            var departments = await _companiesQueryService.GetDepartmentNamesAsync(pharmacyId, companyName, cancellationToken);
            var financialPercentages = await _companiesQueryService.GetFinancialPercentagesAsync(
                pharmacyId, companyName, cancellationToken);

            if (financialPercentages is null)
                return Result<ChequeCreationPreparationDto>.Failure("الشركة غير موجودة أو لا تحتوي على إعدادات مالية.");

            var amountBeforeDiscount = RoundMoney(claim.ClaimAmount);
            var correctAmount = RoundMoney(claim.CorrectedAmount.Value);

            return Result<ChequeCreationPreparationDto>.Success(new ChequeCreationPreparationDto
            {
                ClaimId = claim.Id,
                CompanyName = claim.CompanyName,
                AmountBeforeDiscount = amountBeforeDiscount,
                CorrectAmount = correctAmount,
                AmountDifference = Math.Abs(RoundMoney(correctAmount - amountBeforeDiscount)),
                TaxPercentage = financialPercentages.TaxPercentage,
                AdministrativeExpensesPercentage = financialPercentages.AdministrativeExpensesPercentage,
                FinalAmount = CalculateFinalAmount(
                    correctAmount,
                    financialPercentages.TaxPercentage,
                    financialPercentages.AdministrativeExpensesPercentage),
                SettlementDays = settlementDays,
                Departments = departments
            });
        }

        public async Task<Result<List<ChequeResponseDto>>> CreateAsync(
    Guid pharmacyId, Guid claimId, CreateChequesRequestDto request, CancellationToken cancellationToken = default)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId, cancellationToken);
            if (claim is null || claim.PharmacyId != pharmacyId)
                return Result<List<ChequeResponseDto>>.Failure("المطالبة غير موجودة.");

            if (!claim.CorrectedAmount.HasValue)
                return Result<List<ChequeResponseDto>>.Failure("لم تتم مراجعة هذه المطالبة بعد.");

            var alreadyHasCheques = await _chequeRepository.ExistsForClaimAsync(claimId, cancellationToken);
            if (alreadyHasCheques)
                return Result<List<ChequeResponseDto>>.Failure("تم إنشاء شيكات لهذه المطالبة بالفعل.");

            if (request.Allocations is null || request.Allocations.Count == 0)
                return Result<List<ChequeResponseDto>>.Failure("يجب تحديد توزيع واحد على الأقل.");

            if (request.Allocations.Any(a => a.Amount <= 0))
                return Result<List<ChequeResponseDto>>.Failure("يجب أن تكون قيمة كل شيك أكبر من صفر.");

            var chequeNumbers = request.Allocations
                .Select(a => a.ChequeNumber?.Trim())
                .Where(number => !string.IsNullOrEmpty(number))
                .Select(number => number!)
                .ToList();

            if (chequeNumbers.Distinct(StringComparer.OrdinalIgnoreCase).Count() != chequeNumbers.Count)
                return Result<List<ChequeResponseDto>>.Failure("لا يمكن تكرار رقم الشيك في نفس الطلب.");

            if (chequeNumbers.Any(number => number.Length > 50) ||
                request.Allocations.Any(a => a.BankName?.Trim().Length > 200))
                return Result<List<ChequeResponseDto>>.Failure("رقم الشيك أو اسم البنك أطول من الحد المسموح.");

            foreach (var chequeNumber in chequeNumbers)
            {
                if (await _chequeRepository.ExistsByChequeNumberAsync(pharmacyId, chequeNumber, cancellationToken: cancellationToken))
                    return Result<List<ChequeResponseDto>>.Failure("رقم الشيك مستخدم من قبل.");
            }

            // تحقق جديد: لو الشركة من غير إدارات، لازم يكون allocation واحد بس بـ DepartmentName = null
            var registeredDepartments = await _companiesQueryService.GetDepartmentNamesAsync(pharmacyId, claim.CompanyName, cancellationToken);

            if (registeredDepartments.Count == 0 && request.Allocations.Count > 1)
                return Result<List<ChequeResponseDto>>.Failure("هذه الشركة ليس لديها إدارات تابعة، يجب إرسال توزيع واحد فقط.");

            if (registeredDepartments.Count == 0 && request.Allocations[0].DepartmentName is not null)
                return Result<List<ChequeResponseDto>>.Failure("هذه الشركة ليس لديها إدارات تابعة، اترك اسم الإدارة فارغًا.");

            // تحقق إضافي: لو الشركة عندها إدارات، كل الأسماء المُرسلة لازم تكون من ضمن الإدارات المسجلة فعليًا
            if (registeredDepartments.Count > 0)
            {
                var invalidNames = request.Allocations
                    .Select(a => a.DepartmentName)
                    .Where(name => name is null || !registeredDepartments.Contains(name))
                    .ToList();

                if (invalidNames.Count > 0)
                    return Result<List<ChequeResponseDto>>.Failure("يجب اختيار أسماء إدارات مسجلة فعليًا لدى هذه الشركة.");
            }

            var allocationsSum = request.Allocations.Sum(a => a.Amount);
            if (allocationsSum != claim.CorrectedAmount.Value)
                return Result<List<ChequeResponseDto>>.Failure(
                    $"إجمالي التوزيع ({allocationsSum}) لا يساوي قيمة المطالبة ({claim.CorrectedAmount.Value}).");

            var settlementDays = await _companiesQueryService.GetChequeSettlementPeriodInDaysAsync(pharmacyId, claim.CompanyName, cancellationToken);
            var financialPercentages = await _companiesQueryService.GetFinancialPercentagesAsync(
                pharmacyId, claim.CompanyName, cancellationToken);

            if (financialPercentages is null)
                return Result<List<ChequeResponseDto>>.Failure("الشركة غير موجودة أو لا تحتوي على إعدادات مالية.");

            var endDate = request.StartDate.AddDays(settlementDays);
            var cheques = new List<Cheque>(request.Allocations.Count);
            decimal allocatedBeforeDiscount = 0;

            for (var index = 0; index < request.Allocations.Count; index++)
            {
                var allocation = request.Allocations[index];
                var isLast = index == request.Allocations.Count - 1;
                var ratio = allocation.Amount / allocationsSum;
                var amountBeforeDiscount = isLast
                    ? RoundMoney(claim.ClaimAmount - allocatedBeforeDiscount)
                    : RoundMoney(claim.ClaimAmount * ratio);
                var correctAmount = RoundMoney(allocation.Amount);

                allocatedBeforeDiscount += amountBeforeDiscount;

                var finalAmount = CalculateFinalAmount(
                    correctAmount,
                    financialPercentages.TaxPercentage,
                    financialPercentages.AdministrativeExpensesPercentage);

                cheques.Add(new Cheque
                {
                    ClaimId = claim.Id,
                    PharmacyId = pharmacyId,
                    CompanyName = claim.CompanyName,
                    ClaimMonth = claim.Month,
                    ClaimYear = claim.Year,
                    DepartmentName = allocation.DepartmentName,
                    ChequeNumber = string.IsNullOrWhiteSpace(allocation.ChequeNumber) ? null : allocation.ChequeNumber.Trim(),
                    BankName = string.IsNullOrWhiteSpace(allocation.BankName) ? null : allocation.BankName.Trim(),
                    AmountBeforeDiscount = amountBeforeDiscount,
                    CorrectAmount = correctAmount,
                    AmountDifference = Math.Abs(RoundMoney(correctAmount - amountBeforeDiscount)),
                    TaxPercentage = financialPercentages.TaxPercentage,
                    AdministrativeExpensesPercentage = financialPercentages.AdministrativeExpensesPercentage,
                    FinalAmount = finalAmount,
                    StartDate = request.StartDate,
                    EndDate = endDate,
                    SettlementDays = settlementDays,
                    Status = ChequeStatus.Pending
                });
            }

            var created = await _chequeRepository.TryAddRangeForClaimAsync(claim.Id, cheques, cancellationToken);
            if (!created)
                return Result<List<ChequeResponseDto>>.Failure("تم إنشاء شيكات لهذه المطالبة بالفعل.");

            return Result<List<ChequeResponseDto>>.Success(cheques.Select(c => c.ToResponseDto()).ToList());
        }

        public async Task<Result<List<ChequeResponseDto>>> GetAsync(
            Guid pharmacyId, string? companyName, int? month, int? year, string? status, CancellationToken cancellationToken = default)
        {
            ChequeStatus? parsedStatus = null;
            if (status is not null)
            {
                if (!Enum.TryParse<ChequeStatus>(status, ignoreCase: true, out var value) ||
                    !Enum.IsDefined(value))
                    return Result<List<ChequeResponseDto>>.Failure("حالة غير صحيحة.");

                parsedStatus = value;
            }

            var cheques = await _chequeRepository.GetByPharmacyAsync(pharmacyId, companyName, month, year, parsedStatus, cancellationToken);
            return Result<List<ChequeResponseDto>>.Success(cheques.Select(c => c.ToResponseDto()).ToList());
        }

        public async Task<Result<List<ChequeResponseDto>>> GetUpcomingDueAsync(
            Guid pharmacyId, int days, int? month, int? year, CancellationToken cancellationToken = default)
        {
            if (days < 0)
                return Result<List<ChequeResponseDto>>.Failure("عدد الأيام يجب ألا يكون سالبًا.");

            if (month is < 1 or > 12)
                return Result<List<ChequeResponseDto>>.Failure("Month must be between 1 and 12.");

            if (year is < 1 or > 9999)
                return Result<List<ChequeResponseDto>>.Failure("Year must be between 1 and 9999.");

            var cheques = await _chequeRepository.GetUpcomingDueAsync(pharmacyId, days, month, year, cancellationToken);
            return Result<List<ChequeResponseDto>>.Success(cheques.Select(c => c.ToResponseDto()).ToList());
        }

        public async Task<Result<ChequeResponseDto>> UpdateStatusAsync(Guid pharmacyId, Guid chequeId, UpdateChequeStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            var cheque = await _chequeRepository.GetByIdAsync(chequeId, cancellationToken);
            if (cheque is null || cheque.PharmacyId != pharmacyId)
                return Result<ChequeResponseDto>.Failure("الشيك غير موجود.");

            if (!Enum.TryParse<ChequeStatus>(request.Status, ignoreCase: true, out var status) || status == ChequeStatus.Overdue)
                return Result<ChequeResponseDto>.Failure("حالة غير صحيحة. القيم المسموحة: Pending, PaidInFull, PartiallyPaid, Deferred.");

            var chequeNumber = request.ChequeNumber?.Trim();
            var bankName = request.BankName?.Trim();
            var updatesChequeDetails = request.ChequeNumber is not null || request.BankName is not null;

            if (updatesChequeDetails)
            {
                if (string.IsNullOrEmpty(chequeNumber) || string.IsNullOrEmpty(bankName))
                    return Result<ChequeResponseDto>.Failure("يجب إدخال رقم الشيك واسم البنك معًا.");

                if (chequeNumber.Length > 50 || bankName.Length > 200)
                    return Result<ChequeResponseDto>.Failure("رقم الشيك أو اسم البنك أطول من الحد المسموح.");

                if (await _chequeRepository.ExistsByChequeNumberAsync(
                        pharmacyId, chequeNumber, chequeId, cancellationToken))
                    return Result<ChequeResponseDto>.Failure("رقم الشيك مستخدم من قبل.");
            }

            if (status == ChequeStatus.PartiallyPaid && !request.RemainingAmount.HasValue)
                return Result<ChequeResponseDto>.Failure("يجب إدخال المبلغ المتبقي عند اختيار السداد الجزئي.");

            var isReceived = status is ChequeStatus.PaidInFull or ChequeStatus.PartiallyPaid;
            if (isReceived && (!request.ActualAmount.HasValue || !request.ChequeDate.HasValue))
                return Result<ChequeResponseDto>.Failure("يجب إدخال المبلغ الفعلي وتاريخ الشيك عند استلام الشيك.");

            if (request.ActualAmount is < 0)
                return Result<ChequeResponseDto>.Failure("لا يمكن أن يكون المبلغ الفعلي أقل من صفر.");

            if (request.RemainingAmount is < 0)
                return Result<ChequeResponseDto>.Failure("لا يمكن أن يكون المبلغ المتبقي أقل من صفر.");

            if (request.RemainingAmount > cheque.FinalAmount)
                return Result<ChequeResponseDto>.Failure("لا يمكن أن يكون المبلغ المتبقي أكبر من المبلغ النهائي.");

            cheque.Status = status;
            cheque.RemainingAmount = status == ChequeStatus.PartiallyPaid ? request.RemainingAmount : null;

            if (isReceived)
            {
                cheque.ActualAmount = RoundMoney(request.ActualAmount!.Value);
                cheque.ChequeDate = request.ChequeDate!.Value;
                var signedDifference = cheque.ActualAmount.Value - cheque.FinalAmount;
                cheque.PaymentDifference = Math.Abs(RoundMoney(signedDifference));
                cheque.PaymentDifferenceType = signedDifference > 0
                    ? PaymentDifferenceType.Increase
                    : signedDifference < 0
                        ? PaymentDifferenceType.Decrease
                        : PaymentDifferenceType.Equal;
            }
            else
            {
                cheque.ActualAmount = null;
                cheque.ChequeDate = null;
                cheque.PaymentDifference = null;
                cheque.PaymentDifferenceType = null;
            }

            if (updatesChequeDetails)
            {
                cheque.ChequeNumber = chequeNumber;
                cheque.BankName = bankName;
            }

            _chequeRepository.Update(cheque);
            await _chequeRepository.SaveChangesAsync(cancellationToken);

            return Result<ChequeResponseDto>.Success(cheque.ToResponseDto());
        }

        private static decimal CalculateFinalAmount(
            decimal correctAmount,
            decimal taxPercentage,
            decimal administrativeExpensesPercentage)
        {
            var totalDeductionPercentage = taxPercentage + administrativeExpensesPercentage;
            return RoundMoney(correctAmount * (1 - totalDeductionPercentage / 100m));
        }

        private static decimal RoundMoney(decimal amount) =>
            Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}
