using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;


namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations
{
    public class ClaimReviewConfiguration : IEntityTypeConfiguration<ClaimReview>
    {
        public void Configure(EntityTypeBuilder<ClaimReview> builder)
        {
            builder.ToTable("ClaimReviews", "claims");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.CorrectedAmount).HasColumnType("decimal(18,2)");
            builder.Property(r => r.DiscrepancyType).HasConversion<string>().HasMaxLength(30);
            builder.Property(r => r.Notes).HasMaxLength(1000);

            builder.HasIndex(r => r.ClaimId).IsUnique();
        }
    }
}
