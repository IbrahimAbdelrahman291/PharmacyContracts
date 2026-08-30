using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations
{
    public class ChequeConfiguration : IEntityTypeConfiguration<Cheque>
    {
        public void Configure(EntityTypeBuilder<Cheque> builder)
        {
            builder.ToTable("Cheques", "claims");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.DepartmentName).HasMaxLength(200);
            builder.Property(c => c.Amount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.RemainingAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasIndex(c => c.ClaimId);
            builder.HasIndex(c => new { c.PharmacyId, c.Status, c.EndDate });
        }
    }
}
