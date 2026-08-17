using System.Security.Cryptography;
using PharmacyContracts.Modules.Sales.Application.Interfaces;

namespace PharmacyContracts.Modules.Sales.Application.Services;

public class FileIntegrityService : IFileIntegrityService
{
    private static readonly byte[] ZipSignature = { 0x50, 0x4B, 0x03, 0x04 };

    public bool IsValidXlsxSignature(Stream fileStream)
    {
        if (fileStream.Length < ZipSignature.Length)
            return false;

        var originalPosition = fileStream.Position;
        fileStream.Position = 0;

        var buffer = new byte[ZipSignature.Length];
        fileStream.ReadExactly(buffer, 0, ZipSignature.Length);

        fileStream.Position = originalPosition;

        return buffer.SequenceEqual(ZipSignature);
    }

    public async Task<string> ComputeSha256Async(Stream fileStream, CancellationToken cancellationToken = default)
    {
        var originalPosition = fileStream.Position;
        fileStream.Position = 0;

        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(fileStream, cancellationToken);

        fileStream.Position = originalPosition;

        return Convert.ToHexString(hashBytes);
    }
}