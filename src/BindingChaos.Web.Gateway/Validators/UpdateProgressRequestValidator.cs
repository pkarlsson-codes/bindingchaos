using BindingChaos.Web.Gateway.Models;
using FluentValidation;

namespace BindingChaos.Web.Gateway.Validators;

/// <summary>
/// Validator for <see cref="UpdateProgressRequest"/>.
/// </summary>
internal sealed class UpdateProgressRequestValidator : AbstractValidator<UpdateProgressRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProgressRequestValidator"/> class.
    /// </summary>
    public UpdateProgressRequestValidator()
    {
        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("ProgressPercentage must be between 0 and 100.");

        RuleFor(x => x.ProgressUpdate)
            .NotEmpty()
            .WithMessage("ProgressUpdate is required.")
            .MaximumLength(1000)
            .WithMessage("ProgressUpdate cannot exceed 1000 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.")
            .When(x => x.Notes is not null);
    }
}
