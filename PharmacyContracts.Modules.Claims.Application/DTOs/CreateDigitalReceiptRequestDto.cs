namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class CreateDigitalReceiptRequestDto
{
    public Guid CompanyId { get; set; }
    public Guid? DepartmentId { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
}
