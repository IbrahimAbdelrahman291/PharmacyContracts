using PharmacyContracts.Modules.Claims.Domain.Enums;

namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class UnpaidChequeBalanceDto
{
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public decimal? RemainingAmount { get; set; }
    public ChequeStatus Status { get; set; }
}
