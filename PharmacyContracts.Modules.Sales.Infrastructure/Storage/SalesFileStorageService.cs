// Storage/SalesFileStorageService.cs
using PharmacyContracts.Modules.Sales.Application.Interfaces;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Storage;

public class SalesFileStorageService : ISalesFileStorageService
{
    private readonly string _rootPath;

    // الـ rootPath بييجي من الـ Host وقت الـ registration (wwwroot/pending-uploads)
    public SalesFileStorageService(string rootPath) => _rootPath = rootPath;

    public async Task<string> SaveAsync(Guid pharmacyId, Guid batchId, Stream fileStream, CancellationToken cancellationToken = default)
    {
        var folderPath = Path.Combine(_rootPath, pharmacyId.ToString());
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, $"{batchId}.xlsx");

        fileStream.Position = 0;
        await using var fileOutputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fileOutputStream, cancellationToken);

        return filePath;
    }

    public bool Exists(string localFilePath) => File.Exists(localFilePath);

    public Stream OpenRead(string localFilePath) => new FileStream(localFilePath, FileMode.Open, FileAccess.Read);

    public void Delete(string localFilePath)
    {
        if (File.Exists(localFilePath))
            File.Delete(localFilePath);
    }
}