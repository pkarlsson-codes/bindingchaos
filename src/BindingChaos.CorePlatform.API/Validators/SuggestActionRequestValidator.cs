using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="SuggestActionRequest"/>.
/// Validates the action type and enforces type-specific field requirements.
/// </summary>
internal sealed class SuggestActionRequestValidator : AbstractValidator<SuggestActionRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestActionRequestValidator"/> class.
    /// </summary>
    public SuggestActionRequestValidator()
    {
        RuleFor(x => x.ActionType)
            .NotEmpty()
            .WithMessage("An action type is required.")
            .Must(v => ActionType.TryFromDisplayName(v!, out _))
            .WithMessage("ActionType must be a valid action type.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("A phone number is required for 'Make a call' actions.")
            .Matches(@"^\+?[\d\s\-(). ]{7,20}$")
            .WithMessage("Phone number is not in a valid format.")
            .When(x => x.ActionType == nameof(ActionType.MakeACall));

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("A URL is required for 'Visit a webpage' actions.")
            .Must(v => Uri.TryCreate(v, UriKind.Absolute, out _))
            .WithMessage("URL must be a valid absolute URL.")
            .When(x => x.ActionType == nameof(ActionType.VisitAWebpage));

        RuleFor(x => x.Details)
            .MaximumLength(500)
            .WithMessage("Details cannot exceed 500 characters.")
            .When(x => x.Details is not null);
    }
}
