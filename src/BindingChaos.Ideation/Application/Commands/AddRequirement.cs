using BindingChaos.Ideation.Application.DTOs;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents a request to add a requirement to a specific idea, including the requirement's specifications and the
/// participant responsible for the addition.
/// </summary>
/// <param name="IdeaId">The unique identifier of the idea to which the requirement will be added.</param>
/// <param name="Specification">The specifications of the requirement, detailing its attributes and constraints.</param>
/// <param name="ActorId">The unique identifier of the participant who is adding the requirement.</param>
public sealed record AddRequirement(IdeaId IdeaId, RequirementSpecDto Specification, ParticipantId ActorId);

/// <summary>
/// Handles the <see cref="AddRequirement"/> command.
/// </summary>
public static class AddRequirementHandler
{
    /// <summary>
    /// Handles the addition of a new requirement to an existing idea using the specified command and repositories.
    /// </summary>
    /// <param name="command">The command containing the details of the requirement to add, including the target idea identifier,
    /// specification, and actor information. Cannot be null.</param>
    /// <param name="ideaRepository">The repository used to retrieve the idea by its unique identifier.</param>
    /// <param name="unitOfWork">The unit of work responsible for committing the changes to the underlying data store.</param>
    /// <returns>A task that represents the asynchronous operation of handling the add requirement command.</returns>
    public static async Task Handle(AddRequirement command, IIdeaRepository ideaRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        var idea = await ideaRepository.GetByIdOrThrowAsync(command.IdeaId).ConfigureAwait(false);

        var specification = new RequirementSpec(
            command.Specification.Label,
            command.Specification.Quantity,
            command.Specification.Unit,
            RequirementType.FromDisplayName(command.Specification.Type));
        var requirement = idea.AddRequirement(specification, command.ActorId);
        await unitOfWork.CommitAsync().ConfigureAwait(false);
    }
}