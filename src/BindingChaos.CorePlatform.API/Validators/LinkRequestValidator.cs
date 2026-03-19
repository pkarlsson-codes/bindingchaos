using BindingChaos.CorePlatform.API.Controllers;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="IdentityController.LinkRequest"/>.
/// </summary>
internal sealed class LinkRequestValidator : AbstractValidator<IdentityController.LinkRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkRequestValidator"/> class.
    /// </summary>
    public LinkRequestValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject is required.");
    }
}
