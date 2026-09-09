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
        Guid pharmacyId, string companyName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return Result<CompanyBalanceResponseDto>.Failure("اسم الشركة مطلوب.");

        var normalizedCompanyName = companyName.Trim();
        var totalClaimed = await _claimRepository.GetTotalClaimedAsync(pharmacyId, normalizedCompanyName, cancellationToken);
        var totalCollected = await _chequeRepository.GetTotalCollectedAsync(pharmacyId, normalizedCompanyName, cancellationToken);

        return Result<CompanyBalanceResponseDto>.Success(CreateCompanyBalance(normalizedCompanyName, totalClaimed, totalCollected));
    }

    public async Task<Result<TotalBalanceResponseDto>> GetTotalBalanceAsync(
        Guid pharmacyId, CancellationToken cancellationToken = default)
    {
        var totalClaimed = await _claimRepository.GetTotalClaimedAsync(pharmacyId, null, cancellationToken);
        var totalCollected = await _chequeRepository.GetTotalCollectedAsync(pharmacyId, null, cancellationToken);

        return Result<TotalBalanceResponseDto>.Success(new TotalBalanceResponseDto
        {
            TotalClaimed = totalClaimed,
            TotalCollected = totalCollected,
            Balance = totalClaimed - totalCollected
        });
    }

    public async Task<Result<AgingReportResponseDto>> GetAgingReportAsync(
        Guid pharmacyId, string? companyName, CancellationToken cancellationToken = default)
    {
        var normalizedCompanyName = string.IsNullOrWhiteSpace(companyName) ? null : companyName.Trim();
        var cheques = await _chequeRepository.GetUnpaidOrPartiallyPaidChequesAsync(
            pharmacyId, normalizedCompanyName, cancellationToken);
        var today = DateTime.UtcNow.Date;
        var response = new AgingReportResponseDto();

        foreach (var cheque in cheques)
        {
            var outstanding = cheque.Status == ChequeStatus.PartiallyPaid
                ? cheque.RemainingAmount ?? 0m
                : cheque.Amount;

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
        Guid pharmacyId, int top, CancellationToken cancellationToken = default)
    {
        if (top <= 0)
            return Result<List<CompanyBalanceResponseDto>>.Failure("يجب أن تكون قيمة top أكبر من صفر.");

        var companyNames = await _claimRepository.GetDistinctCompanyNamesAsync(pharmacyId, cancellationToken);
        var balances = new List<CompanyBalanceResponseDto>(companyNames.Count);

        foreach (var companyName in companyNames)
        {
            var totalClaimed = await _claimRepository.GetTotalClaimedAsync(pharmacyId, companyName, cancellationToken);
            var totalCollected = await _chequeRepository.GetTotalCollectedAsync(pharmacyId, companyName, cancellationToken);
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
}
