using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for the ideas feed page.
/// </summary>
internal sealed class IdeasFeedViewModel
{
    /// <summary>
    /// The paginated ideas data.
    /// </summary>
    public PaginatedResponse<IdeaListItemResponse> Ideas { get; set; } = new();
}
