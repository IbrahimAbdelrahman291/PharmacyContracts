using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Claims.Domain.Entities;


namespace PharmacyContracts.Modules.Claims.Infrastructure.Data
{
    public class ClaimsDbContext : DbContext
    {
        public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : base(options) { }

        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<ClaimReview> ClaimReviews => Set<ClaimReview>();
        public DbSet<ClaimReviewDifference> ClaimReviewDifferences => Set<ClaimReviewDifference>();
        public DbSet<Cheque> Cheques => Set<Cheque>();
        public DbSet<DigitalReceipt> DigitalReceipts => Set<DigitalReceipt>();
        public DbSet<CosmeticsCompany> CosmeticsCompanies => Set<CosmeticsCompany>();
        public DbSet<CosmeticsName> CosmeticsNames => Set<CosmeticsName>();
        public DbSet<CosmeticsTransaction> CosmeticsTransactions => Set<CosmeticsTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClaimsDbContext).Assembly);
            modelBuilder.Entity<Claim>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<ClaimReview>().HasQueryFilter(r => !r.IsDeleted);
            modelBuilder.Entity<ClaimReviewDifference>().HasQueryFilter(d => !d.IsDeleted);
            modelBuilder.Entity<Cheque>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<CosmeticsCompany>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<CosmeticsName>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<CosmeticsTransaction>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
