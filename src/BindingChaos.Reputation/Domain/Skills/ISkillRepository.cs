namespace BindingChaos.Reputation.Domain.Skills;

/// <summary>
/// Repository for the skill reference catalogue.
/// </summary>
public interface ISkillRepository
{
    /// <summary>Persists a new skill. Throws if the slug is already taken within the domain.</summary>
    /// <param name="skill">The skill to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(Skill skill, CancellationToken ct);

    /// <summary>
    /// Adds or replaces the localization for the given locale on an existing skill.
    /// No-op if the skill does not exist.
    /// </summary>
    /// <param name="skillId">The skill to localize.</param>
    /// <param name="localization">The locale entry to add or replace.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddLocalizationAsync(Guid skillId, SkillLocalization localization, CancellationToken ct);

    /// <summary>Returns the skill with the given ID, or <see langword="null"/> if not found.</summary>
    /// <param name="id">The skill ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The skill, or <see langword="null"/>.</returns>
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Returns the skill with the given domain ID and slug, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="domainId">The domain ID.</param>
    /// <param name="slug">The skill slug within the domain (e.g. <c>python</c>).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The skill, or <see langword="null"/>.</returns>
    Task<Skill?> FindBySlugAsync(Guid domainId, string slug, CancellationToken ct);

    /// <summary>
    /// Returns all skills whose localized name in <paramref name="locale"/> contains <paramref name="query"/>.
    /// Falls back to the <c>en</c> locale when no match exists for the requested locale.
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <param name="locale">The preferred locale (BCP 47).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The matching skills.</returns>
    Task<IReadOnlyList<Skill>> SearchAsync(string query, string locale, CancellationToken ct);
}
