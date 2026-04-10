using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Societies.Domain.SocialContracts;
using BindingChaos.Societies.Domain.Societies;

namespace BindingChaos.Societies.Application.Commands;

/// <summary>
/// Command to create a new society along with its initial social contract.
/// </summary>
/// <param name="CreatedBy">The participant creating the society.</param>
/// <param name="Name">The name of the society.</param>
/// <param name="Description">The description of the society.</param>
/// <param name="Tags">The initial tags for the society.</param>
/// <param name="RatificationThreshold">The decision protocol ratification threshold.</param>
/// <param name="ReviewWindowHours">The decision protocol review window in hours.</param>
/// <param name="AllowVeto">Whether veto is allowed in the decision protocol.</param>
/// <param name="RequiredVerificationWeight">The required verification weight for epistemic rules.</param>
/// <param name="InquiryLapseWindowHours">How long an unanswered inquiry remains open before auto-lapsing, in hours.</param>
/// <param name="GeographicBounds">Optional geographic bounds for the society.</param>
/// <param name="Center">Optional center coordinates for the society.</param>
public sealed record CreateSociety(
    ParticipantId CreatedBy,
    string Name,
    string Description,
    string[] Tags,
    double RatificationThreshold,
    double ReviewWindowHours,
    bool AllowVeto,
    double RequiredVerificationWeight,
    double InquiryLapseWindowHours,
    GeographicArea? GeographicBounds,
    Coordinates? Center);

/// <summary>
/// Handles the <see cref="CreateSociety"/> command by creating a <see cref="Society"/> and its initial <see cref="SocialContract"/>.
/// </summary>
public static class CreateSocietyHandler
{
    /// <summary>
    /// Handles a <see cref="CreateSociety"/> command, persisting both the society and its initial social contract.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="societyRepository">Repository for staging the society.</param>
    /// <param name="socialContractRepository">Repository for staging the social contract.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the newly created society.</returns>
    public static async Task<SocietyId> Handle(
        CreateSociety command,
        ISocietyRepository societyRepository,
        ISocialContractRepository socialContractRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var society = Society.Create(
            command.CreatedBy,
            command.Name,
            command.Description,
            command.Tags,
            command.GeographicBounds,
            command.Center);

        var protocol = new DecisionProtocol(
            command.RatificationThreshold,
            TimeSpan.FromHours(command.ReviewWindowHours),
            command.AllowVeto,
            TimeSpan.FromHours(command.InquiryLapseWindowHours));

        var epistemicRules = new EpistemicRules(command.RequiredVerificationWeight);

        var socialContract = SocialContract.Establish(
            society.Id,
            command.CreatedBy,
            protocol,
            epistemicRules);

        society.Join(command.CreatedBy, socialContract.Id);

        societyRepository.Stage(society);
        socialContractRepository.Stage(socialContract);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return society.Id;
    }
}
