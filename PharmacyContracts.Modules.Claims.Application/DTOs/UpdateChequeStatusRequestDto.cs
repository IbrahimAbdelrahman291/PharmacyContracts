

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class UpdateChequeStatusRequestDto
    {
        public string Status { get; set; } = string.Empty;   // PaidInFull / PartiallyPaid / Deferred / Pending
        public decimal? RemainingAmount { get; set; }        // مطلوب لو Status = PartiallyPaid
    }
}
