namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for a single society.
/// </summary>
/// <param name="Id">The unique identifier of the society.</param>
/// <param name="Name">The name of the society.</param>
/// <param name="Description">The description of the society.</param>
/// <param name="CreatedByPseudonym">The pseudonym of the creator.</param>
/// <param name="CreatedAt">When the society was created.</param>
/// <param name="Tags">The tags associated with the society.</param>
/// <param name="HasGeographicBounds">Whether this society has geographic bounds.</param>
/// <param name="GeographicBoundsJson">The geographic bounds as JSON, or null.</param>
/// <param name="CenterJson">The center coordinates as JSON, or null.</param>
/// <param name="Relationships">The relationships to other societies.</param>
/// <param name="ActiveMemberCount">The number of active members.</param>
/// <param name="CurrentSocialContractId">The ID of the current active social contract, or null if not yet available.</param>
public sealed record SocietyResponse(
    string Id,
    string Name,
    string Description,
    string CreatedByPseudonym,
    DateTimeOffset CreatedAt,
    string[] Tags,
    bool HasGeographicBounds,
    string? GeographicBoundsJson,
    string? CenterJson,
    SocietyRelationshipResponse[] Relationships,
    int ActiveMemberCount,
    string? CurrentSocialContractId);

/// <summary>
/// Response model for a society relationship.
/// </summary>
/// <param name="TargetSocietyId">The target society ID.</param>
/// <param name="RelationshipType">The type of relationship as a string (e.g. "PartOf", "Affiliated", "Federated").</param>
public sealed record SocietyRelationshipResponse(
    string TargetSocietyId,
    string RelationshipType);

/// <summary>
/// Response model for a society in a list.
/// </summary>
/// <param name="Id">The unique identifier of the society.</param>
/// <param name="Name">The name of the society.</param>
/// <param name="Description">The description of the society.</param>
/// <param name="CreatedAt">When the society was created.</param>
/// <param name="Tags">The tags associated with the society.</param>
/// <param name="HasGeographicBounds">Whether this society has geographic bounds.</param>
/// <param name="ActiveMemberCount">The number of active members.</param>
public sealed record SocietyListItemResponse(
    string Id,
    string Name,
    string Description,
    DateTimeOffset CreatedAt,
    string[] Tags,
    bool HasGeographicBounds,
    int ActiveMemberCount);

/// <summary>
/// Response model for a society member.
/// </summary>
/// <param name="MembershipId">The membership identifier.</param>
/// <param name="Pseudonym">The pseudonym of the member, scoped to this society.</param>
/// <param name="SocialContractId">The social contract ID agreed to at join time.</param>
/// <param name="JoinedAt">When the participant joined.</param>
public sealed record SocietyMemberResponse(
    string MembershipId,
    string Pseudonym,
    string SocialContractId,
    DateTimeOffset JoinedAt);
