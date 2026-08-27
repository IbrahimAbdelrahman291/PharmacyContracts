using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.SharedKernel.Interfaces
{
    public interface ICompaniesQueryService
    {
        Task<decimal> GetDiscountPercentageAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default);
        Task<int> GetChequeSettlementPeriodInDaysAsync(Guid pharmacyId, string companyName, CancellationToken cancellationToken = default);
        Task<Dictionary<string, decimal>> GetDiscountPercentagesAsync(
        Guid pharmacyId, IEnumerable<string> companyNames, CancellationToken cancellationToken = default);
    }
}
