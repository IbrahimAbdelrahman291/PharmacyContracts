using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Companies.Domain.Entities;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Data.Configurations
{
    public class CompanyDepartmentConfiguration : IEntityTypeConfiguration<CompanyDepartment>
    {
        public void Configure(EntityTypeBuilder<CompanyDepartment> builder)
        {
            builder.ToTable("CompanyDepartments", "companies");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
            builder.HasIndex(d => new { d.CompanyId, d.Name }).IsUnique();
        }
    }
}
