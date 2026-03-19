namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Request model for updating progress on an action opportunity.
/// </summary>
public sealed class UpdateProgressRequest
{
    /// <summary>
    /// The progress percentage (0-100).
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// A description of the progress update.
    /// </summary>
    public string ProgressUpdate { get; set; } = string.Empty;

    /// <summary>
    /// Whether the action opportunity is now complete.
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Additional notes about the progress (optional).
    /// </summary>
    public string? Notes { get; set; }
}
