using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations;

public class CosmeticsCompanyConfiguration : IEntityTypeConfiguration<CosmeticsCompany>
{
    public void Configure(EntityTypeBuilder<CosmeticsCompany> builder)
    {
        builder.ToTable("CosmeticsCompanies", "claims");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => new { x.PharmacyId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
    }
}
