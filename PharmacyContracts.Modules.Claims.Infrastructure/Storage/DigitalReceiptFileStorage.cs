using PharmacyContracts.Modules.Claims.Application.Interfaces;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Storage;

public class DigitalReceiptFileStorage : IDigitalReceiptFileStorage
{
    private readonly string _root;
    public DigitalReceiptFileStorage(string webRootPath) { _root = Path.Combine(webRootPath, "digital-receipts"); Directory.CreateDirectory(_root); }
    public async Task<string> SaveAsync(Guid receiptId, Stream content, CancellationToken cancellationToken = default)
    {
        var relativePath = Path.Combine("digital-receipts", $"{receiptId:N}-{Guid.NewGuid():N}.pdf");
        await using var output = new FileStream(Resolve(relativePath), FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
        await content.CopyToAsync(output, cancellationToken);
        return relativePath.Replace('\\', '/');
    }
    public Task<bool> DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        try { var path = Resolve(relativePath); if (File.Exists(path)) File.Delete(path); return Task.FromResult(!File.Exists(path)); }
        catch (IOException) { return Task.FromResult(false); }
        catch (UnauthorizedAccessException) { return Task.FromResult(false); }
    }
    public Task<Stream?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var path = Resolve(relativePath);
        Stream? stream = File.Exists(path) ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true) : null;
        return Task.FromResult(stream);
    }
    private string Resolve(string relativePath)
    {
        var path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(_root)!, relativePath));
        if (!path.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Invalid receipt path.");
        return path;
    }
}
