namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class CreateDigitalReceiptRequestDto
{
    public Guid CompanyId { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
