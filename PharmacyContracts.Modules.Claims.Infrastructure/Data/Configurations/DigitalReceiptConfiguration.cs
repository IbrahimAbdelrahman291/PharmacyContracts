using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations;

public class DigitalReceiptConfiguration : IEntityTypeConfiguration<DigitalReceipt>
{
    public void Configure(EntityTypeBuilder<DigitalReceipt> builder)
    {
        builder.ToTable("DigitalReceipts", "claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.FilePath).HasMaxLength(500);
        builder.Property(x => x.OriginalFileName).HasMaxLength(255);
        builder.HasIndex(x => new { x.PharmacyId, x.CompanyId, x.Month, x.Year }).IsUnique();
        builder.HasIndex(x => new { x.Status, x.ExpiresAt });
    }
}
