using BindingChaos.IdentityProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Configurations;

internal sealed class SocietyInviteLinkConfiguration : IEntityTypeConfiguration<SocietyInviteLink>
{
    public void Configure(EntityTypeBuilder<SocietyInviteLink> builder)
    {
        builder.ToTable("society_invite_link");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).IsRequired().HasMaxLength(22);
        builder.Property(x => x.CreatedById).IsRequired().HasMaxLength(128);
        builder.Property(x => x.SocietyId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.IsRevoked).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.CreatedById);
        builder.HasIndex(x => x.SocietyId);

        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .HasPrincipalKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
