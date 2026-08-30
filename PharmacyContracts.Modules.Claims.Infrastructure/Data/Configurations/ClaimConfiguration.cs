using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("Claims", "claims");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.ClaimAmountAfterDiscount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CorrectedAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(30);

            builder.HasIndex(c => new { c.PharmacyId, c.CompanyName, c.Month, c.Year }).IsUnique();
            builder.HasIndex(c => new { c.PharmacyId, c.Month, c.Year });
        }
    }
}
