using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.SharedKernel.Interfaces
{
    public interface ICompaniesQueryService
    {
        Task<decimal> GetDiscountPercentageAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default);
        Task<int> GetChequeSettlementPeriodInDaysAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default);
    }
}
