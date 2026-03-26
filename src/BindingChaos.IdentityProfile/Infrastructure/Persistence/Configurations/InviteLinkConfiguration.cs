using BindingChaos.IdentityProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Configurations;

internal sealed class InviteLinkConfiguration : IEntityTypeConfiguration<InviteLink>
{
    public void Configure(EntityTypeBuilder<InviteLink> builder)
    {
        builder.ToTable("invite_link");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).IsRequired().HasMaxLength(22);
        builder.Property(x => x.CreatorUserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Note).HasMaxLength(255);
        builder.Property(x => x.IsRevoked).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.CreatorUserId);

        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => x.CreatorUserId)
            .HasPrincipalKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
