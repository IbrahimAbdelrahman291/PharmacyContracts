// BackgroundJobs/ProcessSalesBatchJob.cs
using Hangfire;
using Microsoft.Extensions.Logging;
using PharmacyContracts.Modules.Sales.Application.DTOs;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Domain.Enums;

namespace PharmacyContracts.Modules.Sales.Infrastructure.BackgroundJobs;

public class ProcessSalesBatchJob
{
    private readonly ISalesUploadBatchRepository _batchRepository;
    private readonly ISalesFileStorageService _fileStorageService;
    private readonly IExcelSalesFileParser _fileParser;
    private readonly ISalesRowValidator _rowValidator;
    private readonly ISalesRecordBulkWriter _bulkWriter;
    private readonly ILogger<ProcessSalesBatchJob> _logger;

    public ProcessSalesBatchJob(
        ISalesUploadBatchRepository batchRepository,
        ISalesFileStorageService fileStorageService,
        IExcelSalesFileParser fileParser,
        ISalesRowValidator rowValidator,
        ISalesRecordBulkWriter bulkWriter,
        ILogger<ProcessSalesBatchJob> logger)
    {
        _batchRepository = batchRepository;
        _fileStorageService = fileStorageService;
        _fileParser = fileParser;
        _rowValidator = rowValidator;
        _bulkWriter = bulkWriter;
        _logger = logger;
    }

    // 3 محاولات، وبعدين الـ Recovery Sweep هو المسؤول عن أي محاولات إضافية
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 120, 300 })]
    public async Task ExecuteAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
        {
            _logger.LogWarning("Sales batch {BatchId} not found. Skipping.", batchId);
            return;
        }

        batch.Status = BatchStatus.Processing;
        batch.LastProcessingAttemptAt = DateTime.UtcNow;
        _batchRepository.Update(batch);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        try
        {
            // لو الملف مش موجود (اتمسح من على الـ disk بسبب app pool recycle مثلاً)
            // منعملش retry على الفاضي - نحولها مباشرة لتدخل يدوي
            if (!_fileStorageService.Exists(batch.LocalFilePath))
            {
                _logger.LogCritical("Sales batch {BatchId} file missing at {Path}. Marking for manual intervention.", batchId, batch.LocalFilePath);

                batch.Status = BatchStatus.RequiresManualIntervention;
                batch.ErrorLog = "الملف المحفوظ غير موجود على السيرفر.";
                _batchRepository.Update(batch);
                await _batchRepository.SaveChangesAsync(cancellationToken);
                return;
            }

            List<ParsedSalesRowDto> parsedRows;
            await using (var fileStream = _fileStorageService.OpenRead(batch.LocalFilePath))
            {
                parsedRows = _fileParser.ParseRows(fileStream);
            }

            var (errors, records) = _rowValidator.ValidateAndMap(parsedRows, batch.PharmacyId, batch.Id);

            if (errors.Count > 0)
            {
                // لو الملف كان سليم وقت الرفع، دا موقف غير متوقع خالص (ملف اتغير على الديسك مثلاً)
                // بنعتبره فشل حقيقي مش عادي، عشان نلفت الانتباه له
                _logger.LogError("Sales batch {BatchId} failed re-validation during processing with {Count} errors.", batchId, errors.Count);

                batch.Status = BatchStatus.Failed;
                batch.FailedRows = errors.Count;
                batch.ErrorLog = string.Join(" | ", errors.Take(20).Select(e => $"صف {e.RowNumber} - {e.ColumnName}: {e.Reason}"));
                _batchRepository.Update(batch);
                await _batchRepository.SaveChangesAsync(cancellationToken);
                return;
            }

            // نسخة معدّلة - progress بيتحدث في الـ memory بس، مش بيعمل DB call كل batch
            var lastReportedProgress = 0;

            var movedRows = await _bulkWriter.BulkInsertAsync(
                records,
                batch.Id,
                onProgressBatchCompleted: processed => lastReportedProgress = processed,
                cancellationToken);

            batch.ProcessedRows = movedRows;
            batch.Status = BatchStatus.Completed;
            batch.CompletedAt = DateTime.UtcNow;
            _batchRepository.Update(batch);
            await _batchRepository.SaveChangesAsync(cancellationToken);

            _fileStorageService.Delete(batch.LocalFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sales batch {BatchId} processing failed.", batchId);

            batch.Status = BatchStatus.Failed;
            batch.ErrorLog = ex.Message;
            _batchRepository.Update(batch);
            await _batchRepository.SaveChangesAsync(cancellationToken);

            throw; // نرمي الاستثناء تاني عشان Hangfire يعمل retry حسب الإعدادات فوق
        }
    }
}