using BindingChaos.Reputation.Domain.SkillDomains;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to add or replace a localization on an existing skill domain.
/// </summary>
/// <param name="DomainId">The ID of the domain to localize.</param>
/// <param name="Locale">BCP 47 locale code (e.g. <c>sv</c>).</param>
/// <param name="Name">The localized name of the domain.</param>
/// <param name="Description">An optional localized description.</param>
public sealed record AddSkillDomainLocalization(Guid DomainId, string Locale, string Name, string? Description);

/// <summary>
/// Handles the <see cref="AddSkillDomainLocalization"/> command.
/// </summary>
public static class AddSkillDomainLocalizationHandler
{
    /// <summary>
    /// Adds or replaces the localization for the given locale on the domain.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill domain repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        AddSkillDomainLocalization command,
        ISkillDomainRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var localization = new SkillDomainLocalization
        {
            Locale = command.Locale,
            Name = command.Name,
            Description = command.Description,
        };

        await repository.AddLocalizationAsync(command.DomainId, localization, cancellationToken).ConfigureAwait(false);
    }
}
