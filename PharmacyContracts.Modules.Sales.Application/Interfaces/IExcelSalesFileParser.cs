using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.SharedKernel.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Application.Interfaces
{
    public interface IExcelSalesFileParser
    {
        // بيتأكد إن الـ headers مطابقة بالظبط للمتوقع (نفس الترتيب والنص العربي)
        Result ValidateStructure(Stream fileStream);

        // بيرجّع كل الصفوف كـ raw strings من غير أي تحقق من صحة البيانات
        List<ParsedSalesRowDto> ParseRows(Stream fileStream);
    }
}
