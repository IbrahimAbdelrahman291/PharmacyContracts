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

            return Result<ChequeCreationPreparationDto>.Success(new ChequeCreationPreparationDto
            {
                ClaimId = claim.Id,
                CompanyName = claim.CompanyName,
                Amount = claim.CorrectedAmount.Value,
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
            var endDate = request.StartDate.AddDays(settlementDays);

            var cheques = request.Allocations.Select(a => new Cheque
            {
                ClaimId = claim.Id,
                PharmacyId = pharmacyId,
                CompanyName = claim.CompanyName,
                ClaimMonth = claim.Month,     
                ClaimYear = claim.Year,
                DepartmentName = a.DepartmentName,
                ChequeNumber = string.IsNullOrWhiteSpace(a.ChequeNumber) ? null : a.ChequeNumber.Trim(),
                BankName = string.IsNullOrWhiteSpace(a.BankName) ? null : a.BankName.Trim(),
                Amount = a.Amount,
                StartDate = request.StartDate,
                EndDate = endDate,
                SettlementDays = settlementDays,
                Status = ChequeStatus.Pending
            }).ToList();

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
            Guid pharmacyId, int days, CancellationToken cancellationToken = default)
        {
            if (days < 0)
                return Result<List<ChequeResponseDto>>.Failure("عدد الأيام يجب ألا يكون سالبًا.");

            var cheques = await _chequeRepository.GetUpcomingDueAsync(pharmacyId, days, cancellationToken);
            return Result<List<ChequeResponseDto>>.Success(cheques.Select(c => c.ToResponseDto()).ToList());
        }

        public async Task<Result> UpdateStatusAsync(Guid pharmacyId, Guid chequeId, UpdateChequeStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            var cheque = await _chequeRepository.GetByIdAsync(chequeId, cancellationToken);
            if (cheque is null || cheque.PharmacyId != pharmacyId)
                return Result.Failure("الشيك غير موجود.");

            if (!Enum.TryParse<ChequeStatus>(request.Status, ignoreCase: true, out var status) || status == ChequeStatus.Overdue)
                return Result.Failure("حالة غير صحيحة. القيم المسموحة: Pending, PaidInFull, PartiallyPaid, Deferred.");

            var chequeNumber = request.ChequeNumber?.Trim();
            var bankName = request.BankName?.Trim();
            var updatesChequeDetails = request.ChequeNumber is not null || request.BankName is not null;

            if (updatesChequeDetails)
            {
                if (string.IsNullOrEmpty(chequeNumber) || string.IsNullOrEmpty(bankName))
                    return Result.Failure("يجب إدخال رقم الشيك واسم البنك معًا.");

                if (chequeNumber.Length > 50 || bankName.Length > 200)
                    return Result.Failure("رقم الشيك أو اسم البنك أطول من الحد المسموح.");

                if (await _chequeRepository.ExistsByChequeNumberAsync(
                        pharmacyId, chequeNumber, chequeId, cancellationToken))
                    return Result.Failure("رقم الشيك مستخدم من قبل.");
            }

            if (status == ChequeStatus.PartiallyPaid && !request.RemainingAmount.HasValue)
                return Result.Failure("يجب إدخال المبلغ المتبقي عند اختيار السداد الجزئي.");

            cheque.Status = status;
            cheque.RemainingAmount = status == ChequeStatus.PartiallyPaid ? request.RemainingAmount : null;

            if (updatesChequeDetails)
            {
                cheque.ChequeNumber = chequeNumber;
                cheque.BankName = bankName;
            }

            _chequeRepository.Update(cheque);
            await _chequeRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
