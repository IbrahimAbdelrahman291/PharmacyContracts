namespace PharmacyContracts.Modules.Claims.Application.DTOs;

public class DigitalReceiptDownloadDto
{
    public Stream Content { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
}
