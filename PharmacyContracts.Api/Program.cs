using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PharmacyContracts.Api.Hangfire;
using PharmacyContracts.Api.Services;
using PharmacyContracts.Modules.Auth.Api.Controllers;
using PharmacyContracts.Modules.Auth.Application.DependencyInjection;
using PharmacyContracts.Modules.Auth.Application.Interfaces;
using PharmacyContracts.Modules.Auth.Infrastructure.Data;
using PharmacyContracts.Modules.Auth.Infrastructure.DependencyInjection;
using PharmacyContracts.Modules.Auth.Infrastructure.Seeding;
using PharmacyContracts.Modules.Companies.Api.Controllers;
using PharmacyContracts.Modules.Companies.Application.DependencyInjection;
using PharmacyContracts.Modules.Companies.Infrastructure.DependencyInjection;
using PharmacyContracts.Modules.Sales.Api.Controllers;
using PharmacyContracts.Modules.Sales.Application.DependencyInjection;
using PharmacyContracts.Modules.Sales.Infrastructure.BackgroundJobs;
using PharmacyContracts.Modules.Sales.Infrastructure.DependencyInjection;
using PharmacyContracts.SharedKernel.Interfaces;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(AuthController).Assembly)
            .AddApplicationPart(typeof(CompaniesController).Assembly)
            .AddApplicationPart(typeof(SalesBatchesController).Assembly);

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        // Cross-cutting: current user
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Auth Module
        builder.Services.AddAuthInfrastructure(builder.Configuration);
        builder.Services.AddAuthApplication();

        // Companies Module
        builder.Services.AddCompaniesInfrastructure(builder.Configuration);
        builder.Services.AddCompaniesApplication();

        // Sales Module
        builder.Services.AddSalesInfrastructure(builder.Configuration, builder.Environment.WebRootPath);
        builder.Services.AddSalesApplication();

        // Hangfire
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
            {
                // shared hosting - نخلي القيم متحفظة عشان منستهلكش موارد كتير
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                PrepareSchemaIfNecessary = false
            }));

        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 2; // shared hosting - نتجنب استهلاك موارد زيادة
        });

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
            };
        });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Seed SuperAdmin
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await AuthSeeder.SeedAsync(dbContext, passwordHasher, app.Configuration);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // منع الوصول المباشر لملفات pending-uploads عن طريق URL
        // (لازم يكون قبل UseStaticFiles، عشان يمنع الـ request قبل ما يوصل للـ middleware اللي بيقدم الملفات)
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/pending-uploads"))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            await next();
        });

        app.UseStaticFiles();

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Hangfire Dashboard - محمي بـ SuperAdmin فقط
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthorizationFilter(app.Configuration) }
        });

        // Recurring Job: Recovery Sweep - كل 60 ثانية
        RecurringJob.AddOrUpdate<SalesBatchRecoverySweepJob>(
            "sales-batch-recovery-sweep",
            job => job.ExecuteAsync(CancellationToken.None),
            "*/5 * * * *"); // كل دقيقة (أقل قيمة ممكنة في cron syntax عادي)

        app.Run();
    }
}