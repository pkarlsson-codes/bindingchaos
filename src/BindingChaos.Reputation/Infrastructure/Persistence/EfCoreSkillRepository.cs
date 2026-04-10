using BindingChaos.Reputation.Domain.Skills;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// EF Core-backed implementation of <see cref="ISkillRepository"/>.
/// </summary>
public sealed class EfCoreSkillRepository : ISkillRepository
{
    private readonly CompetenceDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreSkillRepository"/> class.
    /// </summary>
    /// <param name="db">The competence DbContext.</param>
    public EfCoreSkillRepository(CompetenceDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task CreateAsync(Skill skill, CancellationToken ct)
    {
        _db.Skills.Add(skill);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AddLocalizationAsync(Guid skillId, SkillLocalization localization, CancellationToken ct)
    {
        var skill = await _db.Skills
            .Include(s => s.Localizations)
            .FirstOrDefaultAsync(s => s.Id == skillId, ct)
            .ConfigureAwait(false);

        if (skill is null)
        {
            return;
        }

        var existing = skill.Localizations.FirstOrDefault(l => l.Locale == localization.Locale);
        if (existing is not null)
        {
            skill.Localizations.Remove(existing);
        }

        skill.Localizations.Add(localization);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Skills
            .AsNoTracking()
            .Include(s => s.Domain)
            .Include(s => s.Localizations)
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Skill?> FindBySlugAsync(Guid domainId, string slug, CancellationToken ct)
    {
        return await _db.Skills
            .AsNoTracking()
            .Include(s => s.Domain)
            .Include(s => s.Localizations)
            .FirstOrDefaultAsync(s => s.DomainId == domainId && s.Slug == slug, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Skill>> SearchAsync(string query, string locale, CancellationToken ct)
    {
        var pattern = $"%{query}%";

        return await _db.Skills
            .AsNoTracking()
            .Include(s => s.Domain)
            .Include(s => s.Localizations)
            .Where(s => s.Localizations.Any(l =>
                (l.Locale == locale || l.Locale == "en") &&
                EF.Functions.ILike(l.Name, pattern)))
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
