using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for the societies list, including paginated society data.
/// </summary>
internal sealed class SocietiesFeedViewModel
{
    /// <summary>
    /// The paginated societies data.
    /// </summary>
    public PaginatedResponse<SocietyListItemResponse> Societies { get; set; } = new();
}
