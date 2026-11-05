namespace PharmacyContracts.Modules.Sales.Application.Utilities;

public static class ExcelTextNormalizer
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value
            .Replace("\u200E", string.Empty, StringComparison.Ordinal)
            .Replace("\u200F", string.Empty, StringComparison.Ordinal)
            .Replace("\u061C", string.Empty, StringComparison.Ordinal)
            .Replace("\uFEFF", string.Empty, StringComparison.Ordinal)
            .Trim();
    }
}
