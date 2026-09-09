namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class CompanyBalanceResponseDto
{
    public string CompanyName { get; set; } = string.Empty;
    public decimal TotalClaimed { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal Balance { get; set; }
}
