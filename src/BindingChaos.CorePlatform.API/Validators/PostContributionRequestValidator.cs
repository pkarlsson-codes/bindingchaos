using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="PostContributionRequest"/>.
/// </summary>
internal sealed class PostContributionRequestValidator : AbstractValidator<PostContributionRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostContributionRequestValidator"/> class.
    /// </summary>
    public PostContributionRequestValidator()
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