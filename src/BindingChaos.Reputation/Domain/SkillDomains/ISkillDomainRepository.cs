namespace BindingChaos.Reputation.Domain.SkillDomains;

/// <summary>
/// Repository for the skill domain catalogue.
/// </summary>
public interface ISkillDomainRepository
{
    /// <summary>Persists a new skill domain. Throws if the slug is already taken.</summary>
    /// <param name="domain">The domain to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(SkillDomain domain, CancellationToken ct);

    /// <summary>
    /// Adds or replaces the localization for the given locale on an existing domain.
    /// No-op if the domain does not exist.
    /// </summary>
    /// <param name="domainId">The domain to localize.</param>
    /// <param name="localization">The locale entry to add or replace.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLocalizationAsync(Guid domainId, SkillDomainLocalization localization, CancellationToken ct);

    /// <summary>Returns the domain with the given ID, or <see langword="null"/> if not found.</summary>
    /// <param name="id">The domain ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The domain, or <see langword="null"/>.</returns>
    Task<SkillDomain?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Returns the domain with the given slug, or <see langword="null"/> if not found.</summary>
    /// <param name="slug">The domain slug (e.g. <c>engineering</c>).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The domain, or <see langword="null"/>.</returns>
    Task<SkillDomain?> FindBySlugAsync(string slug, CancellationToken ct);

    /// <summary>
    /// Returns all domains whose localized name in <paramref name="locale"/> contains <paramref name="query"/>.
    /// Falls back to the <c>en</c> locale when no match exists for the requested locale.
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <param name="locale">The preferred locale (BCP 47).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The matching domains.</returns>
    Task<IReadOnlyList<SkillDomain>> SearchAsync(string query, string locale, CancellationToken ct);
}
