using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for the emerging patterns feed.
/// </summary>
internal sealed class EmergingPatternsFeedViewModel
{
    /// <summary>
    /// The identified emerging patterns, ordered by cluster label.
    /// </summary>
    public IReadOnlyList<EmergingPatternResponse> Patterns { get; init; } = [];
}
