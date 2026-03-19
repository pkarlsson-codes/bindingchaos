using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="SupportAmendmentRequest"/>.
/// </summary>
internal sealed class SupportAmendmentRequestValidator : AbstractValidator<SupportAmendmentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SupportAmendmentRequestValidator"/> class.
    /// </summary>
    public SupportAmendmentRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .MinimumLength(3)
            .WithMessage("Reason must be at least 3 characters long.")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}
