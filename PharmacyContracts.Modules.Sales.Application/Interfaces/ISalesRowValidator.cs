using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.Modules.Sales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface ISalesRowValidator
    {
        // بيرجع الـ errors لو موجودة، أو الـ entities الجاهزة لو كل الصفوف سليمة
        (List<RowValidationErrorDto> Errors, List<SalesRecord> Records) ValidateAndMap(
            List<ParsedSalesRowDto> rows, Guid pharmacyId, Guid batchId);
    }
}
