using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.ProjectInquiries;

/// <summary>Strongly-typed identifier for a project inquiry aggregate.</summary>
public sealed class ProjectInquiryId : EntityId<ProjectInquiryId>
{
    private const string Prefix = "inquiry";

    private ProjectInquiryId(string value)
        : base(value, Prefix)
    {
    }
}
