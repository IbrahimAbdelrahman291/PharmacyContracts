using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations;

public class CosmeticsTransactionConfiguration : IEntityTypeConfiguration<CosmeticsTransaction>
{
    public void Configure(EntityTypeBuilder<CosmeticsTransaction> builder)
    {
        builder.ToTable("CosmeticsTransactions", "claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Balance).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.Company).WithMany(x => x.Transactions).HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Name).WithMany(x => x.Transactions).HasForeignKey(x => x.NameId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PharmacyId, x.CompanyId, x.Date });
        builder.HasIndex(x => new { x.PharmacyId, x.NameId, x.Date });
    }
}
