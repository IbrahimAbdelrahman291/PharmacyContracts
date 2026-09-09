namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class AgingReportResponseDto
{
    public decimal NotYetDue { get; set; }
    public decimal Overdue0To30 { get; set; }
    public decimal Overdue31To60 { get; set; }
    public decimal Overdue60Plus { get; set; }
    public decimal TotalOutstanding { get; set; }
}
