

namespace PharmacyContracts.Modules.Sales.Application.DTOs
{
    public class RowValidationErrorDto
    {
        public int RowNumber { get; set; }
        public string ColumnName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
