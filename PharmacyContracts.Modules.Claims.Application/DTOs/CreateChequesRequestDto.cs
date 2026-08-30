

namespace PharmacyContracts.Modules.Claims.Application.DTOs
{
    public class CreateChequesRequestDto
    {
        public DateTime StartDate { get; set; }
        public List<ChequeAllocationDto> Allocations { get; set; } = new();
    }
}
