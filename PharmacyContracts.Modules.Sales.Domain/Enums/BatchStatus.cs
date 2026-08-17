using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Domain.Enums
{
    public enum BatchStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        RequiresManualIntervention = 5
    }
}
