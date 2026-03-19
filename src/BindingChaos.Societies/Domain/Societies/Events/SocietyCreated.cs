using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a new society is created.
/// </summary>
/// <param name="AggregateId">The ID of the created society.</param>
/// <param name="Version">The aggregate version when this event was raised.</param>
/// <param name="Name">The name of the society.</param>
/// <param name="Description">The description of the society.</param>
/// <param name="CreatedBy">The participant ID of the creator.</param>
/// <param name="Tags">The initial tags for the society.</param>
/// <param name="GeographicBoundsJson">The geographic bounds as a JSON string, or null if not geographic.</param>
/// <param name="CenterJson">The center coordinates as a JSON string, or null if not geographic.</param>
public sealed record SocietyCreated(
    string AggregateId,
    long Version,
    string Name,
    string Description,
    string CreatedBy,
    IReadOnlyList<string> Tags,
    string? GeographicBoundsJson,
    string? CenterJson
) : DomainEvent(AggregateId, Version);
