using BindingChaos.Web.Gateway.Models;
using FluentValidation;

namespace BindingChaos.Web.Gateway.Validators;

/// <summary>
/// Validator for <see cref="AddCommitmentRequest"/>.
/// </summary>
internal sealed class AddCommitmentRequestValidator : AbstractValidator<AddCommitmentRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddCommitmentRequestValidator"/> class.
    /// </summary>
    public AddCommitmentRequestValidator()
    {
        RuleFor(x => x.CommitmentType)
            .NotEmpty()
            .WithMessage("CommitmentType is required.")
            .MaximumLength(100)
            .WithMessage("CommitmentType cannot exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => x.Notes is not null);
    }
}
