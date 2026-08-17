// Parsing/ExcelSalesFileParser.cs
using ClosedXML.Excel;
using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Parsing;

public class ExcelSalesFileParser : IExcelSalesFileParser
{
    // ترتيب الأعمدة بالظبط زي ما اتفقنا عليه
    private static readonly string[] ExpectedHeaders =
    {
        "مبيعات",
        "اسم العميل",
        "التاريخ",
        "الاجمالي الفرعي",
        "خصم على الاصناف",
        "خصم على الاجمالي",
        "المجموع",
        "اجمالي الاصناف المحلية",
        "اجمالي الاصناف المستوردة",
        "الباقي",
        "الفرع"
    };

    public Result ValidateStructure(Stream fileStream)
    {
        fileStream.Position = 0;

        try
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.First();
            var headerRow = worksheet.Row(1);

            for (var i = 0; i < ExpectedHeaders.Length; i++)
            {
                var actualHeader = headerRow.Cell(i + 1).GetString().Trim();
                if (actualHeader != ExpectedHeaders[i])
                {
                    return Result.Failure(
                        $"العمود رقم {i + 1} يجب أن يكون '{ExpectedHeaders[i]}' لكن الموجود هو '{actualHeader}'.");
                }
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"تعذر قراءة الملف: {ex.Message}");
        }
    }

    public List<ParsedSalesRowDto> ParseRows(Stream fileStream)
    {
        fileStream.Position = 0;

        var rows = new List<ParsedSalesRowDto>();

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        // بنبدأ من الصف الثاني (الأول هو الـ header)
        for (var rowIndex = 2; rowIndex <= lastRow; rowIndex++)
        {
            var row = worksheet.Row(rowIndex);

            // لو الصف فاضي بالكامل، نتجاهله (مش نعتبره error)
            if (row.IsEmpty())
                continue;

            rows.Add(new ParsedSalesRowDto
            {
                RowNumber = rowIndex,
                RawStatus = row.Cell(1).GetString().Trim(),
                CustomerCompanyName = row.Cell(2).GetString().Trim(),
                RawSaleDate = row.Cell(3).GetString().Trim(),
                RawSubTotal = row.Cell(4).GetString().Trim(),
                RawDiscountOnItems = row.Cell(5).GetString().Trim(),
                RawDiscountOnTotal = row.Cell(6).GetString().Trim(),
                RawGrossTotal = row.Cell(7).GetString().Trim(),
                RawLocalItemsTotal = row.Cell(8).GetString().Trim(),
                RawImportedItemsTotal = row.Cell(9).GetString().Trim(),
                RawRemainingAmount = row.Cell(10).GetString().Trim(),
                BranchName = row.Cell(11).GetString().Trim()
            });
        }

        return rows;
    }
}