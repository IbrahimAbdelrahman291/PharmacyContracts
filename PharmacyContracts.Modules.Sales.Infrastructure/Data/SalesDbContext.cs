using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Sales.Domain.Entities;

namespace PharmacyContracts.Modules.Sales.Infrastructure.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<SalesRecord> SalesRecords => Set<SalesRecord>();
        public DbSet<SalesUploadBatch> SalesUploadBatches => Set<SalesUploadBatch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
            modelBuilder.Entity<SalesRecord>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<SalesUploadBatch>().HasQueryFilter(b => !b.IsDeleted);
        }
    }
}
