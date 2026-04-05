using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="CreateProjectRequest"/>.
/// </summary>
internal sealed class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProjectRequestValidator"/> class.
    /// </summary>
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.UserGroupId)
            .NotEmpty()
            .WithMessage("UserGroupId is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MinimumLength(3)
            .WithMessage("Title must be at least 3 characters long.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MinimumLength(10)
            .WithMessage("Description must be at least 10 characters long.")
            .MaximumLength(5000)
            .WithMessage("Description cannot exceed 5000 characters.");
    }
}
