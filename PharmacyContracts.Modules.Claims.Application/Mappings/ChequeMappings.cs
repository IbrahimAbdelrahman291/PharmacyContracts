using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Application.Mappings
{
    public static class ChequeMappings
    {
        public static ChequeResponseDto ToResponseDto(this Cheque cheque)
        {
            return new ChequeResponseDto
            {
                Id = cheque.Id,
                CompanyName = cheque.CompanyName,
                DepartmentName = cheque.DepartmentName,
                Amount = cheque.Amount,
                StartDate = cheque.StartDate,
                EndDate = cheque.EndDate,
                Status = cheque.Status.ToString(),
                RemainingAmount = cheque.RemainingAmount,
                CreatedAt = cheque.CreatedAt
            };
        }
    }
}
