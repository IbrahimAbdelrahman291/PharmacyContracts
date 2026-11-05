using PharmacyContracts.Modules.Sales.Application.Utilities;

namespace PharmacyContracts.Modules.Sales.Application.Tests.Utilities;

public class ExcelTextNormalizerTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("\u200F[مرتجعات]\u200F", "[مرتجعات]")]
    [InlineData("\u200Eمبيعات\u061C\uFEFF", "مبيعات")]
    [InlineData("  نص عربي 123  ", "نص عربي 123")]
    public void Normalize_RemovesOnlySupportedMarksAndOuterWhitespace(string? value, string expected)
    {
        var result = ExcelTextNormalizer.Normalize(value);

        Assert.Equal(expected, result);
    }
}
