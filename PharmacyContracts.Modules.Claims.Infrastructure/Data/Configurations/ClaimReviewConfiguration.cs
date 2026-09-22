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
            builder.Property(r => r.DifferenceAmount).HasColumnType("decimal(18,2)");
            builder.Property(r => r.DifferenceType).HasConversion<string>().HasMaxLength(20);
            builder.Property(r => r.Notes).HasMaxLength(1000);

            builder.HasMany(r => r.Differences)
                .WithOne(d => d.Review)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.ClaimId).IsUnique();
        }
    }
}
