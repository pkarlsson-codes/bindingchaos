using BindingChaos.IdentityProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Configurations;

internal sealed class IdentityMapConfiguration : IEntityTypeConfiguration<IdentityMap>
{
    public void Configure(EntityTypeBuilder<IdentityMap> builder)
    {
        builder.ToTable("identity_maps");
        builder.HasKey(x => new { x.Provider, x.Sub });
        builder.Property(x => x.Provider).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Sub).IsRequired().HasMaxLength(256);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.UserId).IsUnique(false);
    }
}
