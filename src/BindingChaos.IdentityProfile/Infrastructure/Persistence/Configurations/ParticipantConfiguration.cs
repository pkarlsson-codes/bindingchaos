using BindingChaos.IdentityProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.IdentityProfile.Infrastructure.Persistence.Configurations;

internal sealed class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("participant");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.Pseudonym).IsRequired().HasMaxLength(256);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.Pseudonym).IsUnique();
    }
}
