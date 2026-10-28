namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class ClaimReviewDifferenceReportDto
{
    public string CompanyName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<string> Notes { get; set; } = [];
}
