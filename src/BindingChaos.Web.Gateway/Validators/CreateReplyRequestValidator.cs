using BindingChaos.Web.Gateway.Models;
using FluentValidation;

namespace BindingChaos.Web.Gateway.Validators;

/// <summary>
/// Validator for <see cref="CreateReplyRequest"/>.
/// </summary>
internal sealed class CreateReplyRequestValidator : AbstractValidator<CreateReplyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateReplyRequestValidator"/> class.
    /// </summary>
    public CreateReplyRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .MinimumLength(3)
            .WithMessage("Content must be at least 3 characters long.")
            .MaximumLength(2000)
            .WithMessage("Content cannot exceed 2000 characters.");
    }
}
