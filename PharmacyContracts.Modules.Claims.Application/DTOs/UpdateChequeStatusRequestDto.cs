

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class UpdateChequeStatusRequestDto
    {
        public string Status { get; set; } = string.Empty;   // PaidInFull / PartiallyPaid / Deferred / Pending
        public decimal? RemainingAmount { get; set; }        // مطلوب لو Status = PartiallyPaid
        public decimal? ActualAmount { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? ChequeNumber { get; set; }
        public string? BankName { get; set; }
    }
}
