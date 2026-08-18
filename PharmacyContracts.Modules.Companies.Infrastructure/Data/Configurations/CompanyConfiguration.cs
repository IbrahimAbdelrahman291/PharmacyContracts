using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Companies.Domain.Entities;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies", "companies");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.LocalDiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Property(c => c.ImportedDiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Property(c => c.TaxPercentage).HasColumnType("decimal(5,2)");
        builder.Property(c => c.AdministrativeExpensesPercentage).HasColumnType("decimal(5,2)");
        builder.Property(c => c.Discount).HasColumnType("decimal(5,2)");
        builder.HasIndex(c => new { c.PharmacyId, c.Name });
    }
}