using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IChequeRepository _chequeRepository;

    public BalanceService(IClaimRepository claimRepository, IChequeRepository chequeRepository)
    {
        _claimRepository = claimRepository;
        _chequeRepository = chequeRepository;
    }

    public async Task<Result<CompanyBalanceResponseDto>> GetCompanyBalanceAsync(
        Guid pharmacyId, string? companyName, int? month, int? year, CancellationToken cancellationToken = default)
    {
        var periodValidation = ValidatePeriod(month, year);
        if (!periodValidation.Succeeded)
            return Result<CompanyBalanceResponseDto>.Failure(periodValidation.Errors);

        // No company filter means the balance for every company in the pharmacy.
        var normalizedCompanyName = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim();
        var totalClaimed = await _claimRepository.GetTotalClaimedAsync(pharmacyId, normalizedCompanyName, month, year, cancellationToken);
        var totalCollected = await _chequeRepository.GetTotalCollectedAsync(pharmacyId, normalizedCompanyName, month, year, cancellationToken);

        return Result<CompanyBalanceResponseDto>.Success(CreateCompanyBalance(
            normalizedCompanyName ?? "All Companies", totalClaimed, totalCollected));
    }

    public async Task<Result<TotalBalanceResponseDto>> GetTotalBalanceAsync(
        Guid pharmacyId, int? month, int? year, CancellationToken cancellationToken = default)
    {
        var periodValidation = ValidatePeriod(month, year);
        if (!periodValidation.Succeeded)
            return Result<TotalBalanceResponseDto>.Failure(periodValidation.Errors);

        var totalClaimed = await _claimRepository.GetTotalClaimedAsync(pharmacyId, null, month, year, cancellationToken);
        var totalCollected = await _chequeRepository.GetTotalCollectedAsync(pharmacyId, null, month, year, cancellationToken);

        return Result<TotalBalanceResponseDto>.Success(new TotalBalanceResponseDto
        {
            TotalClaimed = totalClaimed,
            TotalCollected = totalCollected,
            Balance = totalClaimed - totalCollected
        });
    }

    public async Task<Result<AgingReportResponseDto>> GetAgingReportAsync(
        Guid pharmacyId, string? companyName, int? month, int? year, CancellationToken cancellationToken = default)
    {
        var periodValidation = ValidatePeriod(month, year);
        if (!periodValidation.Succeeded)
            return Result<AgingReportResponseDto>.Failure(periodValidation.Errors);

        var normalizedCompanyName = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim();
        var cheques = await _chequeRepository.GetUnpaidOrPartiallyPaidChequesAsync(
            pharmacyId, normalizedCompanyName, month, year, cancellationToken);
        var today = DateTime.UtcNow.Date;
        var response = new AgingReportResponseDto();

        foreach (var cheque in cheques)
        {
            var outstanding = cheque.Status == ChequeStatus.PartiallyPaid
                ? cheque.RemainingAmount ?? 0m
                : cheque.FinalAmount;

            if (cheque.EndDate.Date >= today)
                response.NotYetDue += outstanding;
            else
            {
                var daysOverdue = (today - cheque.EndDate.Date).Days;
                if (daysOverdue <= 30)
                    response.Overdue0To30 += outstanding;
                else if (daysOverdue <= 60)
                    response.Overdue31To60 += outstanding;
                else
                    response.Overdue60Plus += outstanding;
            }
        }

        response.TotalOutstanding = response.NotYetDue + response.Overdue0To30 +
                                    response.Overdue31To60 + response.Overdue60Plus;
        return Result<AgingReportResponseDto>.Success(response);
    }

    public async Task<Result<List<CompanyBalanceResponseDto>>> GetTopDebtorsAsync(
        Guid pharmacyId, int top, int? month, int? year, CancellationToken cancellationToken = default)
    {
        if (top <= 0)
            return Result<List<CompanyBalanceResponseDto>>.Failure("يجب أن تكون قيمة top أكبر من صفر.");

        var periodValidation = ValidatePeriod(month, year);
        if (!periodValidation.Succeeded)
            return Result<List<CompanyBalanceResponseDto>>.Failure(periodValidation.Errors);

        var companyNames = await _claimRepository.GetDistinctCompanyNamesAsync(pharmacyId, month, year, cancellationToken);
        var balances = new List<CompanyBalanceResponseDto>(companyNames.Count);

        foreach (var companyName in companyNames)
        {
            var totalClaimed = await _claimRepository.GetTotalClaimedAsync(pharmacyId, companyName, month, year, cancellationToken);
            var totalCollected = await _chequeRepository.GetTotalCollectedAsync(pharmacyId, companyName, month, year, cancellationToken);
            balances.Add(CreateCompanyBalance(companyName, totalClaimed, totalCollected));
        }

        return Result<List<CompanyBalanceResponseDto>>.Success(
            balances.OrderByDescending(b => b.Balance).Take(top).ToList());
    }

    private static CompanyBalanceResponseDto CreateCompanyBalance(
        string companyName, decimal totalClaimed, decimal totalCollected) => new()
        {
            CompanyName = companyName,
            TotalClaimed = totalClaimed,
            TotalCollected = totalCollected,
            Balance = totalClaimed - totalCollected
        };

    private static Result ValidatePeriod(int? month, int? year)
    {
        if (month is < 1 or > 12)
            return Result.Failure("Month must be between 1 and 12.");

        if (year is < 1 or > 9999)
            return Result.Failure("Year must be between 1 and 9999.");

        return Result.Success();
    }
}
