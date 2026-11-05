using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.Modules.Sales.Application.Services;
using PharmacyContracts.Modules.Sales.Domain.Enums;

namespace PharmacyContracts.Modules.Sales.Application.Tests.Services;

public class SalesRowValidatorTests
{
    private readonly SalesRowValidator _validator = new();

    [Theory]
    [InlineData("مبيعات", SalesRecordStatus.Sale)]
    [InlineData("[مرتجعات]", SalesRecordStatus.Return)]
    [InlineData("\u200F[مرتجعات]\u200F", SalesRecordStatus.Return)]
    [InlineData("\u200Eمبيعات", SalesRecordStatus.Sale)]
    [InlineData("\u061C[مرتجعات]\uFEFF", SalesRecordStatus.Return)]
    [InlineData(" [مرتجعات] ", SalesRecordStatus.Return)]
    public void ValidateAndMap_ValidStatus_MapsExpectedStatus(string rawStatus, SalesRecordStatus expected)
    {
        var result = _validator.ValidateAndMap([CreateValidRow(rawStatus)], Guid.NewGuid(), Guid.NewGuid());

        Assert.Empty(result.Errors);
        Assert.Single(result.Records);
        Assert.Equal(expected, result.Records[0].Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("مرتجعات")]
    [InlineData("Invalid")]
    public void ValidateAndMap_InvalidStatus_ReturnsExistingValidationError(string? rawStatus)
    {
        var row = CreateValidRow(rawStatus!);

        var result = _validator.ValidateAndMap([row], Guid.NewGuid(), Guid.NewGuid());

        Assert.Empty(result.Records);
        var error = Assert.Single(result.Errors);
        Assert.Equal("مبيعات", error.ColumnName);
        Assert.Equal("القيمة يجب أن تكون 'مبيعات' أو '[مرتجعات]' فقط.", error.Reason);
    }

    private static ParsedSalesRowDto CreateValidRow(string rawStatus) => new()
    {
        RowNumber = 2,
        RawStatus = rawStatus,
        BranchName = "الفرع الرئيسي",
        CustomerCompanyName = "شركة اختبار",
        RawSaleDate = "2026/11/05",
        RawImportedItemsTotal = "1",
        RawLocalItemsTotal = "2",
        RawGrossTotal = "3",
        RawDiscountOnTotal = "0",
        RawDiscountOnItems = "0",
        RawSubTotal = "3",
        RawRemainingAmount = "0"
    };
}
