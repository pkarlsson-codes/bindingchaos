using BindingChaos.Reputation.Domain.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BindingChaos.Reputation.Infrastructure.Persistence.Configurations;

/// <summary>EF Core entity configuration for <see cref="SkillLocalization"/>.</summary>
internal sealed class SkillLocalizationConfiguration : IEntityTypeConfiguration<SkillLocalization>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SkillLocalization> builder)
    {
        builder.ToTable("skill_localizations");

        builder.HasKey("skill_id", nameof(SkillLocalization.Locale));
        builder.Property<Guid>("skill_id").HasColumnName("skill_id");
        builder.Property(x => x.Locale).HasColumnName("locale").IsRequired().HasMaxLength(10);
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000);
    }
}
