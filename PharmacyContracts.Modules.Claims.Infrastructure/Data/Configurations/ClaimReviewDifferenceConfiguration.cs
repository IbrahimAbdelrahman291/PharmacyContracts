using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyContracts.Modules.Claims.Domain.Entities;

namespace PharmacyContracts.Modules.Claims.Infrastructure.Data.Configurations
{
    public class ClaimReviewDifferenceConfiguration : IEntityTypeConfiguration<ClaimReviewDifference>
    {
        public void Configure(EntityTypeBuilder<ClaimReviewDifference> builder)
        {
            builder.ToTable("ClaimReviewDifferences", "claims");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Value).HasColumnType("decimal(18,2)");
            builder.Property(d => d.Reason).HasConversion<string>().HasMaxLength(30);
            builder.Property(d => d.Notes).HasMaxLength(1000);

            builder.HasIndex(d => d.ReviewId);
            builder.HasIndex(d => d.PharmacyId);
        }
    }
}
