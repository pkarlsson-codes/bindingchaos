using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="ProposeAmendmentRequest"/>.
/// </summary>
internal sealed class ProposeAmendmentRequestValidator : AbstractValidator<ProposeAmendmentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProposeAmendmentRequestValidator"/> class.
    /// </summary>
    public ProposeAmendmentRequestValidator()
    {
        RuleFor(x => x.TargetIdeaVersion)
            .GreaterThan(0)
            .WithMessage("TargetIdeaVersion must be a positive integer.");

        RuleFor(x => x.ProposedTitle)
            .NotEmpty()
            .WithMessage("ProposedTitle is required.")
            .MinimumLength(3)
            .WithMessage("ProposedTitle must be at least 3 characters long.")
            .MaximumLength(200)
            .WithMessage("ProposedTitle cannot exceed 200 characters.");

        RuleFor(x => x.ProposedBody)
            .NotEmpty()
            .WithMessage("ProposedBody is required.")
            .MinimumLength(10)
            .WithMessage("ProposedBody must be at least 10 characters long.")
            .MaximumLength(5000)
            .WithMessage("ProposedBody cannot exceed 5000 characters.");

        RuleFor(x => x.AmendmentTitle)
            .NotEmpty()
            .WithMessage("AmendmentTitle is required.")
            .MinimumLength(3)
            .WithMessage("AmendmentTitle must be at least 3 characters long.")
            .MaximumLength(200)
            .WithMessage("AmendmentTitle cannot exceed 200 characters.");

        RuleFor(x => x.AmendmentDescription)
            .NotEmpty()
            .WithMessage("AmendmentDescription is required.")
            .MinimumLength(10)
            .WithMessage("AmendmentDescription must be at least 10 characters long.")
            .MaximumLength(1000)
            .WithMessage("AmendmentDescription cannot exceed 1000 characters.");
    }
}
