using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyContracts.Modules.Sales.Application.Interfaces;
using PharmacyContracts.Modules.Sales.Infrastructure.BackgroundJobs;
using PharmacyContracts.Modules.Sales.Infrastructure.BulkOperations;
using PharmacyContracts.Modules.Sales.Infrastructure.Data;
using PharmacyContracts.Modules.Sales.Infrastructure.Parsing;
using PharmacyContracts.Modules.Sales.Infrastructure.Repositories;
using PharmacyContracts.Modules.Sales.Infrastructure.Storage;

namespace PharmacyContracts.Modules.Sales.Infrastructure.DependencyInjection;

public static class SalesInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, IConfiguration configuration, string? webRootPath)
    {
        services.AddDbContext<SalesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "sales")));

        services.AddScoped<ISalesUploadBatchRepository, SalesUploadBatchRepository>();
        services.AddScoped<ISalesRecordBulkWriter, SalesRecordBulkWriter>();
        services.AddScoped<IExcelSalesFileParser, ExcelSalesFileParser>();
        services.AddScoped<ISalesBackgroundJobEnqueuer, HangfireSalesBackgroundJobEnqueuer>();

        // مهم: webRootPath ممكن تيجي null على بعض بيئات الاستضافة
        var effectiveWebRootPath = string.IsNullOrWhiteSpace(webRootPath)
            ? Path.Combine(AppContext.BaseDirectory, "wwwroot")
            : webRootPath;

        var uploadsRootPath = Path.Combine(effectiveWebRootPath, "pending-uploads");

        try
        {
            Directory.CreateDirectory(uploadsRootPath);
        }
        catch (Exception)
        {
            // منوقفش التطبيق كله بسبب مشكلة صلاحيات على فولدر واحد
            // هنحاول تاني وقت أول رفع فعلي في SalesFileStorageService.SaveAsync
        }

        services.AddScoped<ISalesFileStorageService>(_ => new SalesFileStorageService(uploadsRootPath));

        services.AddScoped<ProcessSalesBatchJob>();
        services.AddScoped<SalesBatchRecoverySweepJob>();

        return services;
    }
}