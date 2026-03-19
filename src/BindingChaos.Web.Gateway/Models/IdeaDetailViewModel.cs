using BindingChaos.CorePlatform.Contracts.Responses;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for detailed idea information including amendments and lineage.
/// </summary>
internal sealed class IdeaDetailViewModel
{
    /// <summary>
    /// The ID of the idea.
    /// </summary>
    required public IdeaResponse Idea { get; set; }
}
