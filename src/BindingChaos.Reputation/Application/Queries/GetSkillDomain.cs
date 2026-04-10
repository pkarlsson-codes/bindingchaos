using BindingChaos.Reputation.Application.ReadModels;
using BindingChaos.Reputation.Domain.SkillDomains;

namespace BindingChaos.Reputation.Application.Queries;

/// <summary>
/// Query to retrieve a skill domain by ID with its localized name.
/// Falls back to <c>en</c> if no localization exists for the requested locale.
/// </summary>
/// <param name="DomainId">The domain ID.</param>
/// <param name="Locale">Preferred BCP 47 locale (e.g. <c>sv</c>). Defaults to <c>en</c>.</param>
public sealed record GetSkillDomain(Guid DomainId, string Locale = "en");

/// <summary>
/// Handles the <see cref="GetSkillDomain"/> query.
/// </summary>
public static class GetSkillDomainHandler
{
    /// <summary>
    /// Returns the domain view for the given ID, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="repository">The skill domain repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The domain view, or <see langword="null"/>.</returns>
    public static async Task<SkillDomainView?> Handle(
        GetSkillDomain query,
        ISkillDomainRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var domain = await repository.GetByIdAsync(query.DomainId, cancellationToken).ConfigureAwait(false);
        if (domain is null)
        {
            return null;
        }

        var localization = domain.Localizations.FirstOrDefault(l => l.Locale == query.Locale)
            ?? domain.Localizations.FirstOrDefault(l => l.Locale == "en")
            ?? domain.Localizations.FirstOrDefault();

        if (localization is null)
        {
            return null;
        }

        return new SkillDomainView(domain.Id, domain.Slug, localization.Name, localization.Description, localization.Locale);
    }
}
