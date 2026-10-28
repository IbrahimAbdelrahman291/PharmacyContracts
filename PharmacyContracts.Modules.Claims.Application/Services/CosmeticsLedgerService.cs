using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services;

public class CosmeticsLedgerService : ICosmeticsLedgerService
{
    private readonly ICosmeticsLedgerRepository _repository;

    public CosmeticsLedgerService(ICosmeticsLedgerRepository repository) => _repository = repository;

    public async Task<Result<CosmeticsCompanyDto>> CreateCompanyAsync(Guid pharmacyId, CosmeticsLookupRequestDto request, CancellationToken cancellationToken)
    {
        var name = NormalizeName(request.Name);
        if (name is null) return Result<CosmeticsCompanyDto>.Failure("اسم الشركة مطلوب ولا يمكن أن يتجاوز 200 حرف.");
        if (await _repository.FindCompanyByNameAsync(pharmacyId, name, null, cancellationToken) is not null)
            return Result<CosmeticsCompanyDto>.Failure("شركة مستحضرات التجميل موجودة بالفعل.");

        var company = new CosmeticsCompany { PharmacyId = pharmacyId, Name = name };
        await _repository.AddAsync(company, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<CosmeticsCompanyDto>.Success(new(company.Id, company.Name, 0m));
    }

    public async Task<Result<CosmeticsCompanyDto>> UpdateCompanyAsync(Guid pharmacyId, Guid id, CosmeticsLookupRequestDto request, CancellationToken cancellationToken)
    {
        var company = await _repository.GetCompanyAsync(pharmacyId, id, cancellationToken);
        if (company is null) return Result<CosmeticsCompanyDto>.Failure("الشركة غير موجودة.");
        var name = NormalizeName(request.Name);
        if (name is null) return Result<CosmeticsCompanyDto>.Failure("اسم الشركة مطلوب ولا يمكن أن يتجاوز 200 حرف.");
        if (await _repository.FindCompanyByNameAsync(pharmacyId, name, id, cancellationToken) is not null)
            return Result<CosmeticsCompanyDto>.Failure("شركة مستحضرات التجميل موجودة بالفعل.");
        company.Name = name;
        company.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        var transactions = await _repository.GetTransactionsAsync(pharmacyId, id, null, null, null, null, cancellationToken);
        return Result<CosmeticsCompanyDto>.Success(new(company.Id, company.Name, transactions.LastOrDefault()?.Balance ?? 0m));
    }

    public async Task<Result<bool>> DeleteCompanyAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken)
    {
        var company = await _repository.GetCompanyAsync(pharmacyId, id, cancellationToken);
        if (company is null) return Result<bool>.Failure("الشركة غير موجودة.");
        if (await _repository.CompanyHasTransactionsAsync(pharmacyId, id, cancellationToken))
            return Result<bool>.Failure("لا يمكن حذف شركة لديها معاملات.");
        _repository.Remove(company);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<CosmeticsCompanyDto>>> GetCompaniesAsync(Guid pharmacyId, CancellationToken cancellationToken)
    {
        var companies = await _repository.GetCompaniesAsync(pharmacyId, cancellationToken);
        var transactions = await _repository.GetTransactionsAsync(pharmacyId, null, null, null, null, null, cancellationToken);
        var balances = transactions.GroupBy(x => x.CompanyId).ToDictionary(g => g.Key, g => g.Last().Balance);
        return Result<List<CosmeticsCompanyDto>>.Success(companies.Select(x => new CosmeticsCompanyDto(x.Id, x.Name, balances.GetValueOrDefault(x.Id))).ToList());
    }

    public async Task<Result<CosmeticsNameDto>> CreateNameAsync(Guid pharmacyId, CosmeticsLookupRequestDto request, CancellationToken cancellationToken)
    {
        var value = NormalizeName(request.Name);
        if (value is null) return Result<CosmeticsNameDto>.Failure("الاسم مطلوب ولا يمكن أن يتجاوز 200 حرف.");
        if (await _repository.FindNameByNameAsync(pharmacyId, value, null, cancellationToken) is not null)
            return Result<CosmeticsNameDto>.Failure("الاسم موجود بالفعل.");
        var name = new CosmeticsName { PharmacyId = pharmacyId, Name = value };
        await _repository.AddAsync(name, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<CosmeticsNameDto>.Success(new(name.Id, name.Name));
    }

    public async Task<Result<CosmeticsNameDto>> UpdateNameAsync(Guid pharmacyId, Guid id, CosmeticsLookupRequestDto request, CancellationToken cancellationToken)
    {
        var name = await _repository.GetNameAsync(pharmacyId, id, cancellationToken);
        if (name is null) return Result<CosmeticsNameDto>.Failure("الاسم غير موجود.");
        var value = NormalizeName(request.Name);
        if (value is null) return Result<CosmeticsNameDto>.Failure("الاسم مطلوب ولا يمكن أن يتجاوز 200 حرف.");
        if (await _repository.FindNameByNameAsync(pharmacyId, value, id, cancellationToken) is not null)
            return Result<CosmeticsNameDto>.Failure("الاسم موجود بالفعل.");
        name.Name = value;
        name.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<CosmeticsNameDto>.Success(new(name.Id, name.Name));
    }

    public async Task<Result<bool>> DeleteNameAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken)
    {
        var name = await _repository.GetNameAsync(pharmacyId, id, cancellationToken);
        if (name is null) return Result<bool>.Failure("الاسم غير موجود.");
        if (await _repository.NameHasTransactionsAsync(pharmacyId, id, cancellationToken))
            return Result<bool>.Failure("لا يمكن حذف اسم مستخدم في معاملات.");
        _repository.Remove(name);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<CosmeticsNameDto>>> GetNamesAsync(Guid pharmacyId, CancellationToken cancellationToken)
    {
        var names = await _repository.GetNamesAsync(pharmacyId, cancellationToken);
        return Result<List<CosmeticsNameDto>>.Success(names.Select(x => new CosmeticsNameDto(x.Id, x.Name)).ToList());
    }

    public async Task<Result<CosmeticsTransactionDto>> CreateTransactionAsync(Guid pharmacyId, CosmeticsTransactionRequestDto request, CancellationToken cancellationToken)
    {
        var validation = await ValidateTransactionAsync(pharmacyId, request, cancellationToken);
        if (validation.Error is not null) return Result<CosmeticsTransactionDto>.Failure(validation.Error);
        var transaction = new CosmeticsTransaction
        {
            PharmacyId = pharmacyId, CompanyId = request.CompanyId, NameId = request.NameId,
            Amount = Round(request.Amount), Notes = NormalizeNotes(request.Notes), Date = request.Date,
            IsNegative = request.IsNegative
        };
        await _repository.AddAsync(transaction, cancellationToken);
        await _repository.SaveAndRecalculateBalancesAsync(pharmacyId, new[] { request.CompanyId }, cancellationToken);
        transaction.Company = validation.Company!;
        transaction.Name = validation.Name!;
        return Result<CosmeticsTransactionDto>.Success(Map(transaction));
    }

    public async Task<Result<CosmeticsTransactionDto>> UpdateTransactionAsync(Guid pharmacyId, Guid id, CosmeticsTransactionRequestDto request, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetTransactionAsync(pharmacyId, id, cancellationToken);
        if (transaction is null) return Result<CosmeticsTransactionDto>.Failure("المعاملة غير موجودة.");
        var validation = await ValidateTransactionAsync(pharmacyId, request, cancellationToken);
        if (validation.Error is not null) return Result<CosmeticsTransactionDto>.Failure(validation.Error);
        var oldCompanyId = transaction.CompanyId;
        transaction.CompanyId = request.CompanyId;
        transaction.NameId = request.NameId;
        transaction.Amount = Round(request.Amount);
        transaction.Notes = NormalizeNotes(request.Notes);
        transaction.Date = request.Date;
        transaction.IsNegative = request.IsNegative;
        transaction.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveAndRecalculateBalancesAsync(pharmacyId, new[] { oldCompanyId, request.CompanyId }, cancellationToken);
        transaction.Company = validation.Company!;
        transaction.Name = validation.Name!;
        return Result<CosmeticsTransactionDto>.Success(Map(transaction));
    }

    public async Task<Result<bool>> DeleteTransactionAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetTransactionAsync(pharmacyId, id, cancellationToken);
        if (transaction is null) return Result<bool>.Failure("المعاملة غير موجودة.");
        var companyId = transaction.CompanyId;
        _repository.Remove(transaction);
        await _repository.SaveAndRecalculateBalancesAsync(pharmacyId, new[] { companyId }, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<CosmeticsReportDto>> GetReportAsync(Guid pharmacyId, Guid? companyId, Guid? nameId, string? name, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
            return Result<CosmeticsReportDto>.Failure("تاريخ البداية يجب أن يسبق تاريخ النهاية.");
        if (companyId.HasValue && await _repository.GetCompanyAsync(pharmacyId, companyId.Value, cancellationToken) is null)
            return Result<CosmeticsReportDto>.Failure("الشركة غير موجودة.");
        var transactions = await _repository.GetTransactionsAsync(pharmacyId, companyId, nameId, name, from, to, cancellationToken);
        var balances = transactions.GroupBy(x => new { x.CompanyId, x.Company.Name })
            .Select(g => new CosmeticsCompanyBalanceDto(g.Key.CompanyId, g.Key.Name, g.Sum(x => x.IsNegative ? -x.Amount : x.Amount)))
            .ToList();
        return Result<CosmeticsReportDto>.Success(new CosmeticsReportDto { CompanyBalances = balances, Transactions = transactions.Select(Map).ToList() });
    }

    private async Task<(CosmeticsCompany? Company, CosmeticsName? Name, string? Error)> ValidateTransactionAsync(Guid pharmacyId, CosmeticsTransactionRequestDto request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0) return (null, null, "المبلغ يجب أن يكون أكبر من صفر.");
        if (request.Date == default) return (null, null, "تاريخ المعاملة مطلوب.");
        if (request.Notes?.Length > 1000) return (null, null, "الملاحظات أطول من الحد المسموح.");
        var company = await _repository.GetCompanyAsync(pharmacyId, request.CompanyId, cancellationToken);
        if (company is null) return (null, null, "الشركة غير موجودة.");
        var name = await _repository.GetNameAsync(pharmacyId, request.NameId, cancellationToken);
        if (name is null) return (null, null, "اسم المعاملة غير موجود.");
        return (company, name, null);
    }

    private static CosmeticsTransactionDto Map(CosmeticsTransaction x) =>
        new(x.Id, x.CompanyId, x.Company.Name, x.NameId, x.Name.Name, x.Amount, x.Notes, x.Date, x.Balance, x.IsNegative);
    private static string? NormalizeName(string? value) => string.IsNullOrWhiteSpace(value) || value.Trim().Length > 200 ? null : value.Trim();
    private static string? NormalizeNotes(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
