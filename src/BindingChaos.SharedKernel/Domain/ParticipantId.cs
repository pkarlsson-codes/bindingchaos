using System.Diagnostics.CodeAnalysis;

namespace BindingChaos.SharedKernel.Domain;

/// <summary>
/// Unique identifier for a Participant.
/// </summary>
public class ParticipantId : EntityId<ParticipantId>
{
    private const string Prefix = "participant";

    private const string AnonymousValue = "participant-anonymous";

    /// <summary>
    /// Initializes a new instance of the ParticipantId class.
    /// </summary>
    /// <param name="value">The unique identifier value.</param>
    public ParticipantId(string value)
        : base(value, Prefix)
    {
    }

    /// <summary>
    /// Represents an anonymous participant with a predefined identifier.
    /// </summary>
    public static ParticipantId Anonymous => new(AnonymousValue);

    /// <summary>
    /// Gets a value indicating whether the current instance represents an anonymous user.
    /// </summary>
    public bool IsAnonymous => Value == AnonymousValue;
}