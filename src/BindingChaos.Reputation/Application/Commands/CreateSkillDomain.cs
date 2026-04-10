using BindingChaos.Reputation.Domain.SkillDomains;
using BindingChaos.SharedKernel.Domain.Services;

namespace BindingChaos.Reputation.Application.Commands;

/// <summary>
/// Command to create a new skill domain in the competence catalogue.
/// </summary>
/// <param name="Slug">The globally unique, ASCII slug (e.g. <c>engineering</c>, <c>creative-arts</c>).</param>
/// <param name="Locale">BCP 47 locale code for the initial localization (e.g. <c>en</c>).</param>
/// <param name="Name">The localized name of the domain.</param>
/// <param name="Description">An optional localized description.</param>
public sealed record CreateSkillDomain(string Slug, string Locale, string Name, string? Description);

/// <summary>
/// Handles the <see cref="CreateSkillDomain"/> command.
/// </summary>
public static class CreateSkillDomainHandler
{
    /// <summary>
    /// Creates a new skill domain with an initial localization and persists it.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="repository">The skill domain repository.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the newly created domain.</returns>
    public static async Task<Guid> Handle(
        CreateSkillDomain command,
        ISkillDomainRepository repository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var domain = new SkillDomain
        {
            Id = Guid.NewGuid(),
            Slug = command.Slug,
            CreatedAt = TimeProviderContext.Current.UtcNow,
            Localizations =
            [
                new SkillDomainLocalization
                {
                    Locale = command.Locale,
                    Name = command.Name,
                    Description = command.Description,
                },
            ],
        };

        await repository.CreateAsync(domain, cancellationToken).ConfigureAwait(false);
        return domain.Id;
    }
}
