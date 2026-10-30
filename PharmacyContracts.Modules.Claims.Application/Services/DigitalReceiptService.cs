using PharmacyContracts.Modules.Claims.Application.DTOs;
using PharmacyContracts.Modules.Claims.Application.Interfaces;
using PharmacyContracts.Modules.Claims.Domain.Entities;
using PharmacyContracts.Modules.Claims.Domain.Enums;
using PharmacyContracts.SharedKernel.Interfaces;
using PharmacyContracts.SharedKernel.Wrappers;

namespace PharmacyContracts.Modules.Claims.Application.Services;

public class DigitalReceiptService : IDigitalReceiptService
{
    private readonly IDigitalReceiptRepository _repository;
    private readonly IDigitalReceiptFileStorage _fileStorage;
    private readonly ICompaniesQueryService _companies;

    public DigitalReceiptService(IDigitalReceiptRepository repository, IDigitalReceiptFileStorage fileStorage, ICompaniesQueryService companies)
    {
        _repository = repository;
        _fileStorage = fileStorage;
        _companies = companies;
    }

    public async Task<Result<DigitalReceiptResponseDto>> CreateAsync(Guid pharmacyId, CreateDigitalReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Amount < 0) return Result<DigitalReceiptResponseDto>.Failure("Amount cannot be negative.");
        if (request.Month is < 1 or > 12) return Result<DigitalReceiptResponseDto>.Failure("Month must be between 1 and 12.");
        if (request.Year < 1) return Result<DigitalReceiptResponseDto>.Failure("Year is invalid.");

        var period = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var utcNow = DateTime.UtcNow;
        var currentPeriod = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        if (period > currentPeriod) return Result<DigitalReceiptResponseDto>.Failure("Future receipt requests are not allowed.");

        var companyName = await _companies.GetCompanyNameByIdAsync(pharmacyId, request.CompanyId, cancellationToken);
        if (companyName is null) return Result<DigitalReceiptResponseDto>.Failure("Company not found for this pharmacy.");
        string? departmentName = null;
        if (request.DepartmentId.HasValue)
        {
            departmentName = await _companies.GetDepartmentNameByIdAsync(pharmacyId, request.CompanyId, request.DepartmentId.Value, cancellationToken);
            if (departmentName is null) return Result<DigitalReceiptResponseDto>.Failure("Department not found for this company and pharmacy.");
        }
        if (await _repository.ExistsForPeriodAsync(pharmacyId, request.CompanyId, request.DepartmentId, request.Month, request.Year, cancellationToken))
            return Result<DigitalReceiptResponseDto>.Failure($"A receipt request already exists for this {(request.DepartmentId.HasValue ? "department" : "company")} and period.");

        var receipt = new DigitalReceipt
        {
            PharmacyId = pharmacyId,
            CompanyId = request.CompanyId,
            CompanyName = companyName,
            DepartmentId = request.DepartmentId,
            DepartmentName = departmentName,
            Amount = request.Amount,
            Month = request.Month,
            Year = request.Year
        };
        await _repository.AddAsync(receipt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<DigitalReceiptResponseDto>.Success(Map(receipt));
    }

    public async Task<Result<List<DigitalReceiptResponseDto>>> GetAllAsync(Guid pharmacyId, CancellationToken cancellationToken = default)
        => Result<List<DigitalReceiptResponseDto>>.Success((await _repository.GetByPharmacyAsync(pharmacyId, cancellationToken)).Select(Map).ToList());

    public async Task<Result<DigitalReceiptResponseDto>> UploadAsync(Guid pharmacyId, Guid id, Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var receipt = await _repository.GetByIdAsync(id, cancellationToken);
        if (receipt is null || receipt.PharmacyId != pharmacyId) return Result<DigitalReceiptResponseDto>.Failure("Receipt request not found.");
        if (receipt.Status == DigitalReceiptStatus.Expired || receipt.ExpiresAt <= DateTime.UtcNow) return Result<DigitalReceiptResponseDto>.Failure("Receipt request has expired.");

        if (receipt.FilePath is not null && !await _fileStorage.DeleteAsync(receipt.FilePath, cancellationToken))
            return Result<DigitalReceiptResponseDto>.Failure("The existing file could not be deleted; re-upload was rejected.");

        var path = await _fileStorage.SaveAsync(receipt.Id, content, cancellationToken);
        var now = DateTime.UtcNow;
        receipt.FilePath = path;
        receipt.OriginalFileName = Path.GetFileName(fileName);
        receipt.UploadedAt = now;
        receipt.ExpiresAt ??= now.AddDays(60);
        receipt.Status = DigitalReceiptStatus.Available;
        _repository.Update(receipt);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result<DigitalReceiptResponseDto>.Success(Map(receipt));
    }

    public async Task<Result<DigitalReceiptDownloadDto>> DownloadAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken = default)
    {
        var receipt = await _repository.GetByIdAsync(id, cancellationToken);
        if (receipt is null || receipt.PharmacyId != pharmacyId) return Result<DigitalReceiptDownloadDto>.Failure("Receipt request not found.");
        if (receipt.Status != DigitalReceiptStatus.Available || receipt.ExpiresAt <= DateTime.UtcNow || receipt.FilePath is null)
            return Result<DigitalReceiptDownloadDto>.Failure("Receipt file is not available.");
        var stream = await _fileStorage.OpenReadAsync(receipt.FilePath, cancellationToken);
        if (stream is null) return Result<DigitalReceiptDownloadDto>.Failure("Receipt file was not found.");
        return Result<DigitalReceiptDownloadDto>.Success(new DigitalReceiptDownloadDto { Content = stream, FileName = receipt.OriginalFileName ?? "receipt.pdf" });
    }

    public async Task<Result> CancelAsync(Guid pharmacyId, Guid id, CancellationToken cancellationToken = default)
    {
        var receipt = await _repository.GetByIdAsync(id, cancellationToken);
        if (receipt is null || receipt.PharmacyId != pharmacyId) return Result.Failure("Receipt request not found.");
        if (receipt.Status != DigitalReceiptStatus.PendingUpload) return Result.Failure("Only pending receipt requests can be cancelled.");
        _repository.Remove(receipt);
        await _repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static DigitalReceiptResponseDto Map(DigitalReceipt r) => new()
    {
        Id = r.Id, CompanyId = r.CompanyId, PharmacyId = r.PharmacyId, CompanyName = r.CompanyName,
        DepartmentId = r.DepartmentId, DepartmentName = r.DepartmentName,
        Amount = r.Amount, Month = r.Month, Year = r.Year, Status = r.Status,
        OriginalFileName = r.OriginalFileName, UploadedAt = r.UploadedAt, ExpiresAt = r.ExpiresAt, CreatedAt = r.CreatedAt
    };
}
