using BindingChaos.Reputation.Domain.Skills;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to add or replace a localization entry on an existing skill.
/// </summary>
/// <param name="SkillId">The ID of the skill to localize.</param>
/// <param name="Locale">BCP 47 locale code (e.g. <c>sv</c>, <c>zh-Hans</c>).</param>
/// <param name="Name">The localized name.</param>
/// <param name="Description">An optional localized description.</param>
public sealed record AddSkillLocalization(Guid SkillId, string Locale, string Name, string? Description);

/// <summary>
/// Handles the <see cref="AddSkillLocalization"/> command.
/// </summary>
public static class AddSkillLocalizationHandler
{
    /// <summary>
    /// Adds or replaces the localization for the specified locale. No-op if the skill does not exist.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        AddSkillLocalization command,
        ISkillRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var localization = new SkillLocalization
        {
            Locale = command.Locale,
            Name = command.Name,
            Description = command.Description,
        };

        await repository.AddLocalizationAsync(command.SkillId, localization, cancellationToken).ConfigureAwait(false);
    }
}
