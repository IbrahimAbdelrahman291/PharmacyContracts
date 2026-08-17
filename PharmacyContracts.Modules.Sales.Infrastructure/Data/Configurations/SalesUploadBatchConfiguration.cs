using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Sales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Data.Configurations
{
    public class SalesUploadBatchConfiguration : IEntityTypeConfiguration<SalesUploadBatch>
    {
        public void Configure(EntityTypeBuilder<SalesUploadBatch> builder)
        {
            builder.ToTable("SalesUploadBatches", "sales");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.FileName).IsRequired().HasMaxLength(500);
            builder.Property(b => b.FileHash).IsRequired().HasMaxLength(64);
            builder.Property(b => b.LocalFilePath).IsRequired().HasMaxLength(1000);
            builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(30);
            builder.Property(b => b.ErrorLog).HasMaxLength(4000);

            // خط الدفاع الأساسي ضد إعادة معالجة نفس الملف لنفس الصيدلية
            builder.HasIndex(b => new { b.PharmacyId, b.FileHash }).IsUnique();

            // للـ Recovery Sweep - بيدور بسرعة على الحالات العالقة
            builder.HasIndex(b => b.Status);
        }
    }
}
