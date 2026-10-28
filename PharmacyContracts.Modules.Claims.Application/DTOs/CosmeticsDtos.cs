namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public record CosmeticsLookupRequestDto(string Name);
public record CosmeticsCompanyDto(Guid Id, string Name, decimal Balance);
public record CosmeticsNameDto(Guid Id, string Name);

public class CosmeticsTransactionRequestDto
{
    public Guid CompanyId { get; set; }
    public Guid NameId { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime Date { get; set; }
    public bool IsNegative { get; set; }
}

public record CosmeticsTransactionDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    Guid NameId,
    string Name,
    decimal Amount,
    string? Notes,
    DateTime Date,
    decimal Balance,
    bool IsNegative);

public record CosmeticsCompanyBalanceDto(Guid CompanyId, string CompanyName, decimal Balance);

public class CosmeticsReportDto
{
    public List<CosmeticsCompanyBalanceDto> CompanyBalances { get; set; } = new();
    public List<CosmeticsTransactionDto> Transactions { get; set; } = new();
}
