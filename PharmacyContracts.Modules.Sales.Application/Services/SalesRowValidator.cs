using System.Globalization;
using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Domain.Entities;
using PharmacyContracts.Modules.Sales.Domain.Enums;

namespace PharmacyContracts.Modules.Sales.Application.Services;

public class SalesRowValidator : ISalesRowValidator
{
    private const string DateFormat = "yyyy/MM/dd";

    public (List<RowValidationErrorDto> Errors, List<SalesRecord> Records) ValidateAndMap(
        List<ParsedSalesRowDto> rows, Guid pharmacyId, Guid batchId)
    {
        var errors = new List<RowValidationErrorDto>();
        var records = new List<SalesRecord>();

        foreach (var row in rows)
        {
            var rowErrors = new List<RowValidationErrorDto>();

            if (string.IsNullOrWhiteSpace(row.BranchName))
                rowErrors.Add(Error(row.RowNumber, "الفرع", "لا يمكن أن يكون فارغًا."));

            if (string.IsNullOrWhiteSpace(row.CustomerCompanyName))
                rowErrors.Add(Error(row.RowNumber, "اسم العميل", "لا يمكن أن يكون فارغًا."));

            DateTime saleDate = default;
            if (!DateTime.TryParseExact(row.RawSaleDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out saleDate))
                rowErrors.Add(Error(row.RowNumber, "التاريخ", $"يجب أن يكون بصيغة {DateFormat}."));

            var importedItemsTotal = ParseDecimal(row.RowNumber, "اجمالي الاصناف المستوردة", row.RawImportedItemsTotal, rowErrors, allowZero: true);
            var localItemsTotal = ParseDecimal(row.RowNumber, "اجمالي الاصناف المحلية", row.RawLocalItemsTotal, rowErrors, allowZero: true);
            var grossTotal = ParseDecimal(row.RowNumber, "المجموع", row.RawGrossTotal, rowErrors, allowZero: true);
            var discountOnTotal = ParseDecimal(row.RowNumber, "خصم على الاجمالي", row.RawDiscountOnTotal, rowErrors, allowZero: true);
            var discountOnItems = ParseDecimal(row.RowNumber, "خصم على الاصناف", row.RawDiscountOnItems, rowErrors, allowZero: true);
            var subTotal = ParseDecimal(row.RowNumber, "الاجمالي الفرعي", row.RawSubTotal, rowErrors, allowZero: true);
            var remainingAmount = ParseDecimal(row.RowNumber, "الباقي", row.RawRemainingAmount, rowErrors, allowZero: true);

            SalesRecordStatus status = default;
            var trimmedStatus = row.RawStatus.Trim();
            if (trimmedStatus == "مبيعات")
                status = SalesRecordStatus.Sale;
            else if (trimmedStatus == "مرتجعات")
                status = SalesRecordStatus.Return;
            else
                rowErrors.Add(Error(row.RowNumber, "مبيعات", "القيمة يجب أن تكون 'مبيعات' أو 'مرتجعات' فقط."));

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            records.Add(new SalesRecord
            {
                PharmacyId = pharmacyId,
                UploadBatchId = batchId,
                BranchName = row.BranchName.Trim(),
                CustomerCompanyName = row.CustomerCompanyName.Trim(),
                SaleDate = saleDate,
                ImportedItemsTotal = importedItemsTotal,
                LocalItemsTotal = localItemsTotal,
                GrossTotal = grossTotal,
                DiscountOnTotal = discountOnTotal,
                DiscountOnItems = discountOnItems,
                SubTotal = subTotal,
                RemainingAmount = remainingAmount,
                Status = status
            });
        }

        // لو فيه أي error في أي صف، نرفض كل الملف بالكامل (all-or-nothing)
        return errors.Count > 0 ? (errors, new List<SalesRecord>()) : (errors, records);
    }

    private static decimal ParseDecimal(int rowNumber, string columnName, string rawValue, List<RowValidationErrorDto> rowErrors, bool allowZero)
    {
        if (!decimal.TryParse(rawValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            rowErrors.Add(Error(rowNumber, columnName, "يجب أن يكون رقمًا صحيحًا."));
            return 0;
        }

        if (!allowZero && value <= 0)
        {
            rowErrors.Add(Error(rowNumber, columnName, "يجب أن يكون أكبر من صفر."));
        }

        return value;
    }

    private static RowValidationErrorDto Error(int rowNumber, string column, string reason)
        => new() { RowNumber = rowNumber, ColumnName = column, Reason = reason };
}