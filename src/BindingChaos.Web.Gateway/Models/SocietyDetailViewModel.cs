using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for detailed society information.
/// </summary>
internal sealed class SocietyDetailViewModel
{
    /// <summary>
    /// The full society details.
    /// </summary>
    required public SocietyResponse Society { get; set; }
}
