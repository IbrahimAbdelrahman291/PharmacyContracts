using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface ISalesFileStorageService
    {
        Task<string> SaveAsync(Guid pharmacyId, Guid batchId, Stream fileStream, CancellationToken cancellationToken = default);
        bool Exists(string localFilePath);
        Stream OpenRead(string localFilePath);
        void Delete(string localFilePath);
    }
}
