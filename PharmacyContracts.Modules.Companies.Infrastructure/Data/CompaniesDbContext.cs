using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Companies.Domain.Entities;

namespace PharmacyContracts.Modules.Companies.Infrastructure.Data;

public class CompaniesDbContext : DbContext
{
    public CompaniesDbContext(DbContextOptions<CompaniesDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompaniesDbContext).Assembly);
        modelBuilder.Entity<Company>().HasQueryFilter(c => !c.IsDeleted);
    }
}