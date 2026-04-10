using BindingChaos.Reputation.Domain.SkillDomains;
using BindingChaos.Reputation.Domain.Skills;
using BindingChaos.Reputation.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the competence catalogue (skill domains, skills, and their localizations).
/// Endorsement graph data is stored in Neo4j via <see cref="Neo4jSkillEndorsementRepository"/>.
/// </summary>
public sealed class CompetenceDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompetenceDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public CompetenceDbContext(DbContextOptions<CompetenceDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets the skill domains catalogue.</summary>
    public DbSet<SkillDomain> SkillDomains => Set<SkillDomain>();

    /// <summary>Gets the skill domain localizations.</summary>
    public DbSet<SkillDomainLocalization> SkillDomainLocalizations => Set<SkillDomainLocalization>();

    /// <summary>Gets the skills catalogue.</summary>
    public DbSet<Skill> Skills => Set<Skill>();

    /// <summary>Gets the skill localizations.</summary>
    public DbSet<SkillLocalization> SkillLocalizations => Set<SkillLocalization>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("competence");
        modelBuilder.ApplyConfiguration(new SkillDomainConfiguration());
        modelBuilder.ApplyConfiguration(new SkillDomainLocalizationConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new SkillLocalizationConfiguration());
    }
}
