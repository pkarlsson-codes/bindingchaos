using BindingChaos.Reputation.Domain.Skills;
using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to create a new skill in the competence catalogue.
/// </summary>
/// <param name="DomainId">The ID of the domain this skill belongs to.</param>
/// <param name="Slug">The ASCII slug, unique within the domain (e.g. <c>python</c>).</param>
/// <param name="Locale">BCP 47 locale code for the initial localization (e.g. <c>en</c>).</param>
/// <param name="Name">The localized name of the skill.</param>
/// <param name="Description">An optional localized description.</param>
public sealed record CreateSkill(Guid DomainId, string Slug, string Locale, string Name, string? Description);

/// <summary>
/// Handles the <see cref="CreateSkill"/> command.
/// </summary>
public static class CreateSkillHandler
{
    /// <summary>
    /// Creates a new skill with an initial localization and persists it.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the newly created skill.</returns>
    public static async Task<Guid> Handle(
        CreateSkill command,
        ISkillRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            DomainId = command.DomainId,
            Slug = command.Slug,
            CreatedAt = TimeProviderContext.Current.UtcNow,
            Localizations =
            [
                new SkillLocalization
                {
                    Locale = command.Locale,
                    Name = command.Name,
                    Description = command.Description,
                },
            ],
        };

        await repository.CreateAsync(skill, cancellationToken).ConfigureAwait(false);
        return skill.Id;
    }
}
