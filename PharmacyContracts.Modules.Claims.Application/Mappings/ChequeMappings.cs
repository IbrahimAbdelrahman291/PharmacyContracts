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
                ChequeNumber = cheque.ChequeNumber,
                BankName = cheque.BankName,
                AmountBeforeDiscount = cheque.AmountBeforeDiscount,
                CorrectAmount = cheque.CorrectAmount,
                AmountDifference = cheque.AmountDifference,
                TaxPercentage = cheque.TaxPercentage,
                AdministrativeExpensesPercentage = cheque.AdministrativeExpensesPercentage,
                FinalAmount = cheque.FinalAmount,
                ActualAmount = cheque.ActualAmount,
                PaymentDifference = cheque.PaymentDifference,
                PaymentDifferenceType = cheque.PaymentDifferenceType?.ToString(),
                ChequeDate = cheque.ChequeDate,
                ClaimMonth = cheque.ClaimMonth,
                ClaimYear = cheque.ClaimYear,
                StartDate = cheque.StartDate,
                EndDate = cheque.EndDate,
                SettlementDays = cheque.SettlementDays,
                Status = cheque.Status.ToString(),
                RemainingAmount = cheque.RemainingAmount,
                CreatedAt = cheque.CreatedAt
            };
        }
    }
}
