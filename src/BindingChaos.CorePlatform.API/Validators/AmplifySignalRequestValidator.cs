using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SignalAwareness.Domain.Signals;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="AmplifySignalRequest"/>.
/// </summary>
internal sealed class AmplifySignalRequestValidator : AbstractValidator<AmplifySignalRequest>
{
    private static readonly string AllowedReasonsDisplay = string.Join(
        ", ",
        AmplificationReason.GetAll().Select(r => r.DisplayName));

    /// <summary>
    /// Initializes a new instance of the <see cref="AmplifySignalRequestValidator"/> class.
    /// </summary>
    public AmplifySignalRequestValidator()
    {
        RuleFor(x => x.Reason)
            .Must(reason => string.IsNullOrWhiteSpace(reason) || AmplificationReason.TryFromDisplayName(reason, out _))
            .WithMessage($"Reason must be one of: {AllowedReasonsDisplay}.");

        RuleFor(x => x.Commentary)
            .MaximumLength(2000)
            .WithMessage("Commentary cannot exceed 2000 characters.");
    }
}
