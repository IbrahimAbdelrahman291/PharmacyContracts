namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class TotalBalanceResponseDto
{
    public decimal TotalClaimed { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal Balance { get; set; }
}
