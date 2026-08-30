

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class ChequeResponseDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? RemainingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
