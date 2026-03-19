using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Pseudonymity.Application.Services;

/// <summary>
/// Contract for generating deterministic pseudonyms using HMAC-SHA256.
/// Pseudonyms are generated on-the-fly with no caching or storage required.
/// </summary>
public interface IPseudonymService
{
    /// <summary>
    /// Generates pseudonyms for the specified users under the provided scope.
    /// Pseudonyms are deterministically generated using HMAC-SHA256, so the same 
    /// combination of aggregate and user will always produce the same pseudonym.
    /// </summary>
    /// <param name="aggregateId">The aggregate identifier.</param>
    /// <param name="userIds">User identifiers to resolve.</param>
    /// <returns>Mapping of userId to pseudonym.</returns>
    IReadOnlyDictionary<string, string> Generate<TAggregateId>(
        TAggregateId aggregateId,
        IEnumerable<string> userIds)
        where TAggregateId : EntityId;

    /// <summary>
    /// Generates a pseudonym for a single user under the specified aggregate type and ID.
    /// Pseudonyms are deterministically generated using HMAC-SHA256, so the same 
    /// combination of aggregate and user will always produce the same pseudonym.
    /// </summary>
    /// <param name="aggregateId">The aggregate identifier.</param>
    /// <param name="userId">User identifier to resolve.</param>
    /// <returns>The pseudonym.</returns>
    string Generate<TAggregateId>(
        TAggregateId aggregateId,
        string userId)
        where TAggregateId : EntityId;
}
