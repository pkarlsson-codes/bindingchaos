using BindingChaos.IdentityProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Configurations;

internal sealed class UserTrustConfiguration : IEntityTypeConfiguration<UserTrust>
{
    public void Configure(EntityTypeBuilder<UserTrust> builder)
    {
        builder.ToTable("user_trust");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.PersonhoodVerified).IsRequired();
        builder.Property(x => x.TrustLevel).IsRequired().HasMaxLength(32);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
    }
}
