namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Request model for adding or withdrawing a commitment to an action opportunity.
/// </summary>
public sealed class AddCommitmentRequest
{
    /// <summary>
    /// The type of commitment being made.
    /// </summary>
    public string CommitmentType { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes about the commitment (optional).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this is a withdrawal of an existing commitment.
    /// </summary>
    public bool IsWithdrawal { get; set; }
}
