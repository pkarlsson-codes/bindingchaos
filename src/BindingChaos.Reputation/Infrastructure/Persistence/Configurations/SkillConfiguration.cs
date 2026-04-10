using BindingChaos.Reputation.Domain.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.Reputation.Infrastructure.Persistence.Configurations;

/// <summary>EF Core entity configuration for <see cref="Skill"/>.</summary>
internal sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.DomainId).HasColumnName("domain_id").IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").IsRequired().HasMaxLength(200);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(x => new { x.DomainId, x.Slug }).IsUnique();

        builder.HasOne(x => x.Domain)
            .WithMany()
            .HasForeignKey(x => x.DomainId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Localizations)
            .WithOne()
            .HasForeignKey("skill_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
