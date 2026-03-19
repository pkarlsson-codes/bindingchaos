using BindingChaos.CorePlatform.Contracts.Requests;
using FluentValidation;

namespace BindingChaos.CorePlatform.API.Validators;

/// <summary>
/// Validator for <see cref="JoinSocietyRequest"/>.
/// </summary>
internal sealed class JoinSocietyRequestValidator : AbstractValidator<JoinSocietyRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JoinSocietyRequestValidator"/> class.
    /// </summary>
    public JoinSocietyRequestValidator()
    {
        RuleFor(x => x.SocialContractId)
            .NotEmpty()
            .WithMessage("SocialContractId is required.");
    }
}
