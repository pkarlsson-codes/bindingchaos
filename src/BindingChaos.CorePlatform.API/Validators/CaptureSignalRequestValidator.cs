using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for ApiRequest containing CreateSignalRequest.
/// </summary>
internal sealed class CaptureSignalRequestValidator : AbstractValidator<CaptureSignalRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CaptureSignalRequestValidator"/> class.
    /// </summary>
    public CaptureSignalRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.AttachmentIds)
            .NotNull()
            .WithMessage("AttachmentIds field is required.");
    }
}
