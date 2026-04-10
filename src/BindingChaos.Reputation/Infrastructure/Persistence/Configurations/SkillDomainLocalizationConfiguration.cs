using BindingChaos.Reputation.Domain.SkillDomains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.Reputation.Infrastructure.Persistence.Configurations;

/// <summary>EF Core entity configuration for <see cref="SkillDomainLocalization"/>.</summary>
internal sealed class SkillDomainLocalizationConfiguration : IEntityTypeConfiguration<SkillDomainLocalization>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SkillDomainLocalization> builder)
    {
        builder.ToTable("skill_domain_localizations");

        builder.HasKey("domain_id", nameof(SkillDomainLocalization.Locale));
        builder.Property<Guid>("domain_id").HasColumnName("domain_id");
        builder.Property(x => x.Locale).HasColumnName("locale").IsRequired().HasMaxLength(10);
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000);
    }
}
