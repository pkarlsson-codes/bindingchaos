using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="CreateSocietyRequest"/>.
/// </summary>
internal sealed class CreateSocietyRequestValidator : AbstractValidator<CreateSocietyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSocietyRequestValidator"/> class.
    /// </summary>
    public CreateSocietyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.RatificationThreshold)
            .ExclusiveBetween(0.0, 1.0)
            .WithMessage("RatificationThreshold must be greater than 0.0 and less than 1.0.");

        RuleFor(x => x.ReviewWindowHours)
            .GreaterThan(0.0)
            .WithMessage("ReviewWindowHours must be greater than 0.");

        RuleFor(x => x.RequiredVerificationWeight)
            .GreaterThanOrEqualTo(0.0)
            .WithMessage("RequiredVerificationWeight must be greater than or equal to 0.0.");
    }
}
