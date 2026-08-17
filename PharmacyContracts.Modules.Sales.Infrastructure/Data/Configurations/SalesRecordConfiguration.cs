using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Sales.Domain.Entities;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Data.Configurations
{
    public class SalesRecordConfiguration : IEntityTypeConfiguration<SalesRecord>
    {
        public void Configure(EntityTypeBuilder<SalesRecord> builder)
        {
            builder.ToTable("SalesRecords", "sales");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.BranchName).IsRequired().HasMaxLength(200);
            builder.Property(r => r.CustomerCompanyName).IsRequired().HasMaxLength(200);

            builder.Property(r => r.ImportedItemsTotal).HasColumnType("decimal(18,2)");
            builder.Property(r => r.LocalItemsTotal).HasColumnType("decimal(18,2)");
            builder.Property(r => r.GrossTotal).HasColumnType("decimal(18,2)");
            builder.Property(r => r.DiscountOnTotal).HasColumnType("decimal(18,2)");
            builder.Property(r => r.DiscountOnItems).HasColumnType("decimal(18,2)");
            builder.Property(r => r.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(r => r.RemainingAmount).HasColumnType("decimal(18,2)");

            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

            // للاستخدام وقت حساب المطالبات لاحقًا: تجميع حسب الشركة والتاريخ لكل صيدلية
            builder.HasIndex(r => new { r.PharmacyId, r.CustomerCompanyName, r.SaleDate })
                .IncludeProperties(r => r.RemainingAmount);

            builder.HasIndex(r => r.UploadBatchId);
        }
    }
}
