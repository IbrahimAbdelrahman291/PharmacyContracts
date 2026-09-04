using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Domain.Entities;
using PharmacyContracts.Modules.Sales.Domain.Enums;
using PharmacyContracts.SharedKernel.Wrappers;
using Microsoft.AspNetCore.Http;

namespace PharmacyContracts.Modules.Sales.Application.Services;

public class SalesUploadService : ISalesUploadService
{
    private readonly IFileIntegrityService _fileIntegrityService;
    private readonly IExcelSalesFileParser _fileParser;
    private readonly ISalesRowValidator _rowValidator;
    private readonly ISalesFileStorageService _fileStorageService;
    private readonly ISalesUploadBatchRepository _batchRepository;
    private readonly ISalesBackgroundJobEnqueuer _jobEnqueuer;

    public SalesUploadService(
        IFileIntegrityService fileIntegrityService,
        IExcelSalesFileParser fileParser,
        ISalesRowValidator rowValidator,
        ISalesFileStorageService fileStorageService,
        ISalesUploadBatchRepository batchRepository,
        ISalesBackgroundJobEnqueuer jobEnqueuer)
    {
        _fileIntegrityService = fileIntegrityService;
        _fileParser = fileParser;
        _rowValidator = rowValidator;
        _fileStorageService = fileStorageService;
        _batchRepository = batchRepository;
        _jobEnqueuer = jobEnqueuer;
    }

    public async Task<Result<UploadBatchResponseDto>> UploadAsync(Guid pharmacyId, Guid createdBy, IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var stream = file.OpenReadStream();

        // 1) Magic bytes
        if (!_fileIntegrityService.IsValidXlsxSignature(stream))
            return Result<UploadBatchResponseDto>.Failure("الملف المرفوع ليس ملف Excel صالح.");

        // 2) Hash + duplicate check
        var fileHash = await _fileIntegrityService.ComputeSha256Async(stream, cancellationToken);
        var existingBatch = await _batchRepository.GetByPharmacyAndHashAsync(pharmacyId, fileHash, cancellationToken);
        if (existingBatch is not null)
            return Result<UploadBatchResponseDto>.Failure($"هذا الملف تم رفعه من قبل. BatchId: {existingBatch.Id}");

        // 3) Structural validation
        var structureResult = _fileParser.ValidateStructure(stream);
        if (!structureResult.Succeeded)
            return Result<UploadBatchResponseDto>.Failure(structureResult.Errors);

        // 4) Parse + row-level validation (all-or-nothing)
        var parsedRows = _fileParser.ParseRows(stream);
        var batchId = Guid.NewGuid();
        var (rowErrors, records) = _rowValidator.ValidateAndMap(parsedRows, pharmacyId, batchId);

        if (rowErrors.Count > 0)
        {
            var errorMessages = rowErrors
                .Select(e => $"صف {e.RowNumber} - {e.ColumnName}: {e.Reason}")
                .ToArray();
            return Result<UploadBatchResponseDto>.Failure(errorMessages);
        }

        // 5) Persist file (بعد نجاح كل التحقق فقط)
        var localFilePath = await _fileStorageService.SaveAsync(pharmacyId, batchId, stream, cancellationToken);

        // 6) Create batch record
        var batch = new SalesUploadBatch
        {
            Id = batchId,
            PharmacyId = pharmacyId,
            FileName = file.FileName,
            FileHash = fileHash,
            LocalFilePath = localFilePath,
            TotalRows = records.Count,
            ProcessedRows = 0,
            FailedRows = 0,
            Status = BatchStatus.Pending,
            CreatedBy = createdBy
        };

        await _batchRepository.AddAsync(batch, cancellationToken);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        // 7) Enqueue background job
        _jobEnqueuer.EnqueueProcessBatch(batchId);

        return Result<UploadBatchResponseDto>.Success(new UploadBatchResponseDto
        {
            BatchId = batchId,
            Status = batch.Status.ToString()
        });
    }

    // Services/SalesUploadService.cs
    public async Task<Result<BatchStatusResponseDto>> GetStatusAsync(Guid pharmacyId, Guid batchId, CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);

        if (batch is null || batch.PharmacyId != pharmacyId)
            return Result<BatchStatusResponseDto>.Failure("Batch not found.");

        return Result<BatchStatusResponseDto>.Success(new BatchStatusResponseDto
        {
            Id = batch.Id,
            Status = batch.Status.ToString(),
            TotalRows = batch.TotalRows,
            ProcessedRows = batch.ProcessedRows,
            FailedRows = batch.FailedRows,
            ErrorLog = batch.ErrorLog,
            CreatedAt = batch.CreatedAt,
            CompletedAt = batch.CompletedAt
        });
    }
}