using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Common;

namespace PharmacyContracts.Modules.Claims.Domain.Entities
{
    public class Cheque : BaseEntity
    {
        public Guid ClaimId { get; set; }
        public Guid PharmacyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }

        public decimal Amount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SettlementDays { get; set; }

        public ChequeStatus Status { get; set; } = ChequeStatus.Pending;
        public decimal? RemainingAmount { get; set; }
    }
}
