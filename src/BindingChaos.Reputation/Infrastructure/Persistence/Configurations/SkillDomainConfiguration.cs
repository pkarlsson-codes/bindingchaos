using BindingChaos.Reputation.Domain.SkillDomains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.Reputation.Infrastructure.Persistence.Configurations;

/// <summary>EF Core entity configuration for <see cref="SkillDomain"/>.</summary>
internal sealed class SkillDomainConfiguration : IEntityTypeConfiguration<SkillDomain>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SkillDomain> builder)
    {
        builder.ToTable("skill_domains");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Slug).HasColumnName("slug").IsRequired().HasMaxLength(200);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.HasMany(x => x.Localizations)
            .WithOne()
            .HasForeignKey("domain_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
