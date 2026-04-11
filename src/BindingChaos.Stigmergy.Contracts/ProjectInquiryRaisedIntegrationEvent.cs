using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Integration event published when a project inquiry is raised, enabling other bounded contexts to react.
/// </summary>
/// <param name="InquiryId">The inquiry identifier.</param>
/// <param name="ProjectId">The project the inquiry is about.</param>
public sealed record ProjectInquiryRaisedIntegrationEvent(
    string InquiryId,
    string ProjectId
) : IntegrationEvent;
