using BindingChaos.Reputation.Domain.SkillDomains;
using Microsoft.EntityFrameworkCore;

namespace BindingChaos.Reputation.Infrastructure.Persistence;

/// <summary>
/// EF Core-backed implementation of <see cref="ISkillDomainRepository"/>.
/// </summary>
public sealed class EfCoreSkillDomainRepository : ISkillDomainRepository
{
    private readonly CompetenceDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreSkillDomainRepository"/> class.
    /// </summary>
    /// <param name="db">The competence DbContext.</param>
    public EfCoreSkillDomainRepository(CompetenceDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task CreateAsync(SkillDomain domain, CancellationToken ct)
    {
        _db.SkillDomains.Add(domain);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AddLocalizationAsync(Guid domainId, SkillDomainLocalization localization, CancellationToken ct)
    {
        var domain = await _db.SkillDomains
            .Include(d => d.Localizations)
            .FirstOrDefaultAsync(d => d.Id == domainId, ct)
            .ConfigureAwait(false);

        if (domain is null)
        {
            return;
        }

        var existing = domain.Localizations.FirstOrDefault(l => l.Locale == localization.Locale);
        if (existing is not null)
        {
            domain.Localizations.Remove(existing);
        }

        domain.Localizations.Add(localization);
        await _db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<SkillDomain?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.SkillDomains
            .AsNoTracking()
            .Include(d => d.Localizations)
            .FirstOrDefaultAsync(d => d.Id == id, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<SkillDomain?> FindBySlugAsync(string slug, CancellationToken ct)
    {
        return await _db.SkillDomains
            .AsNoTracking()
            .Include(d => d.Localizations)
            .FirstOrDefaultAsync(d => d.Slug == slug, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SkillDomain>> SearchAsync(string query, string locale, CancellationToken ct)
    {
        var pattern = $"%{query}%";

        return await _db.SkillDomains
            .AsNoTracking()
            .Include(d => d.Localizations)
            .Where(d => d.Localizations.Any(l =>
                (l.Locale == locale || l.Locale == "en") &&
                EF.Functions.ILike(l.Name, pattern)))
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }
}
