using BindingChaos.Web.Gateway.Models;
using FluentValidation;

namespace BindingChaos.Web.Gateway.Validators;

/// <summary>
/// Validator for <see cref="CreateActionOpportunityRequest"/>.
/// </summary>
internal sealed class CreateActionOpportunityRequestValidator : AbstractValidator<CreateActionOpportunityRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateActionOpportunityRequestValidator"/> class.
    /// </summary>
    public CreateActionOpportunityRequestValidator()
    {
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

        RuleFor(x => x.ParentIdeaId)
            .NotEmpty()
            .WithMessage("ParentIdeaId is required.");
    }
}
