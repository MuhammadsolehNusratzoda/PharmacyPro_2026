using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyPro.Infrastructure.Identity;

namespace PharmacyPro.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.Property(x => x.TokenHash).IsRequired().HasMaxLength(128);
        b.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => x.ExpiresAt);
        b.HasOne(x => x.User).WithMany()
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}