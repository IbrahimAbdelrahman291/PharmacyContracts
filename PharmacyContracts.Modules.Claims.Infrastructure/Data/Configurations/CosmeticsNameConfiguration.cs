using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations;

public class CosmeticsNameConfiguration : IEntityTypeConfiguration<CosmeticsName>
{
    public void Configure(EntityTypeBuilder<CosmeticsName> builder)
    {
        builder.ToTable("CosmeticsNames", "claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => new { x.PharmacyId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
