namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class ClaimReviewDifferenceReportDto
{
    public string CompanyName { get; set; } = string.Empty;
    public Dictionary<string, decimal> TotalsByReason { get; set; } = [];
}
