namespace BindingChaos.Stigmergy.Domain.ProjectInquiries;

/// <summary>The lifecycle status of a project inquiry.</summary>
public enum ProjectInquiryStatus
{
    /// <summary>Raised and awaiting a response from the user group.</summary>
    Open,

    /// <summary>User group has responded; awaiting raiser acceptance.</summary>
    Responded,

    /// <summary>Raiser accepted the response. Terminal state (unless reopened).</summary>
    Resolved,

    /// <summary>Auto-closed because the raiser did not act within the lapse window.</summary>
    Lapsed,
}
