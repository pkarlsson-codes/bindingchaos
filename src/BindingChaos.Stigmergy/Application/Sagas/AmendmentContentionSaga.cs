using BindingChaos.Stigmergy.Application.Commands;
using BindingChaos.Stigmergy.Application.Messages;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Sagas;

/// <summary>
/// Saga that manages the amendment contention resolution process.
/// Started by <see cref="AmendmentContentionStarted"/>, tracks agree/disagree votes
/// over the resolution window, then resolves the contention by either rejecting
/// the amendment or restoring it to Active.
/// </summary>
/// <remarks>
/// Saga correlation: <see cref="Id"/> equals the AmendmentId. Incoming messages
/// must include a property named <c>AmendmentContentionSagaId</c> so Wolverine
/// can route them to the correct saga instance.
/// </remarks>
public sealed class AmendmentContentionSaga : Saga
{
    /// <summary>Gets or sets the saga correlation ID (equals the AmendmentId).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the identifier of the project containing the amendment.</summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>Gets or sets the rejection threshold snapshotted from the user group charter at contention time.</summary>
    public decimal RejectionThreshold { get; set; }

    /// <summary>Gets or sets the scheduled end of the resolution window.</summary>
    public DateTimeOffset ResolutionWindowEnd { get; set; }

    /// <summary>
    /// Gets or sets the votes recorded during the resolution window.
    /// Key: participant ID string. Value: true if they agree with the contention (want amendment rejected).
    /// </summary>
    public Dictionary<string, bool> Votes { get; set; } = new();

    /// <summary>
    /// Starts the contention saga when an amendment is contested.
    /// Records the contester as the first agree vote and schedules resolution.
    /// </summary>
    /// <param name="message">The start message containing contention details.</param>
    /// <param name="context">The Wolverine message context for scheduling.</param>
    /// <returns>The initialized saga instance.</returns>
    public static async Task<AmendmentContentionSaga> Start(
        AmendmentContentionStarted message,
        IMessageContext context)
    {
        ArgumentNullException.ThrowIfNull(message);

        var saga = new AmendmentContentionSaga
        {
            Id = message.AmendmentId,
            ProjectId = message.ProjectId,
            RejectionThreshold = message.RejectionThreshold,
            ResolutionWindowEnd = DateTimeOffset.UtcNow.Add(message.ResolutionWindow),
            Votes = new Dictionary<string, bool>
            {
                [message.ContesterId] = true,
            },
        };

        await context.SendAsync(
            new ResolveAmendmentContention(message.AmendmentId, message.ProjectId),
            new DeliveryOptions { ScheduleDelay = message.ResolutionWindow }).ConfigureAwait(false);

        return saga;
    }

    /// <summary>
    /// Records or updates a participant's vote on the contention.
    /// </summary>
    /// <param name="message">The interaction command containing the vote.</param>
    public void Handle(InteractWithAmendmentContention message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Votes[message.ParticipantId] = message.AgreesWithContention;
    }

    /// <summary>
    /// Resolves the contention at the end of the resolution window.
    /// Rejects the amendment if the agree ratio meets the threshold; otherwise restores it to Active.
    /// </summary>
    /// <param name="message">The timeout message.</param>
    /// <param name="bus">The Wolverine message bus for dispatching the outcome command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(ResolveAmendmentContention message, IMessageBus bus)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(bus);

        var agrees = Votes.Values.Count(v => v);
        var total = Votes.Count;
        var shouldReject = total > 0 && (decimal)agrees / total >= RejectionThreshold;

        if (shouldReject)
        {
            await bus.InvokeAsync(new RejectAmendment(message.ProjectId, Id)).ConfigureAwait(false);
        }
        else
        {
            await bus.InvokeAsync(new RestoreAmendmentToActive(message.ProjectId, Id)).ConfigureAwait(false);
        }

        MarkCompleted();
    }
}
