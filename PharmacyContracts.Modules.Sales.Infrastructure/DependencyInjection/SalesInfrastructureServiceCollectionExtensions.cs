// DependencyInjection/SalesInfrastructureServiceCollectionExtensions.cs
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
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, IConfiguration configuration, string webRootPath)
    {
        services.AddDbContext<SalesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "sales")));

        services.AddScoped<ISalesUploadBatchRepository, SalesUploadBatchRepository>();
        services.AddScoped<ISalesRecordBulkWriter, SalesRecordBulkWriter>();
        services.AddScoped<IExcelSalesFileParser, ExcelSalesFileParser>();
        services.AddScoped<ISalesBackgroundJobEnqueuer, HangfireSalesBackgroundJobEnqueuer>();

        var uploadsRootPath = Path.Combine(webRootPath, "pending-uploads");
        services.AddScoped<ISalesFileStorageService>(_ => new SalesFileStorageService(uploadsRootPath));

        // الـ Hangfire Job classes نفسها لازم تتسجل في الـ DI عشان تقدر تاخد dependencies
        services.AddScoped<ProcessSalesBatchJob>();
        services.AddScoped<SalesBatchRecoverySweepJob>();

        return services;
    }
}