using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Domain.Entities;


namespace PharmacyContracts.Modules.Claims.Infrastructure.Data
{
    public class ClaimsDbContext : DbContext
    {
        public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options) { }

        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<ClaimReview> ClaimReviews => Set<ClaimReview>();
        public DbSet<Cheque> Cheques => Set<Cheque>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClaimsDbContext).Assembly);
            modelBuilder.Entity<Claim>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ClaimReview>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<Cheque>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
