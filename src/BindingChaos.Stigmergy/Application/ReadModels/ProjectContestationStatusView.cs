namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Tracks how many open inquiries a project currently has.
/// <see cref="IsContested"/> is true when <see cref="OpenInquiryCount"/> is greater than zero.
/// </summary>
public class ProjectContestationStatusView
{
    /// <summary>The project identifier.</summary>
    required public string Id { get; set; }

    /// <summary>The number of currently open (unresolved, unlapsed) inquiries.</summary>
    public int OpenInquiryCount { get; set; }

    /// <summary>True when at least one inquiry is open.</summary>
    public bool IsContested => OpenInquiryCount > 0;
}
