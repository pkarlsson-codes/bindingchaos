using BindingChaos.CorePlatform.API.Validators;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.Tests.Validators;

public class ProposeAmendmentRequestValidatorTests
{
    private static ProposeAmendmentRequest ValidRequest() => new()
    {
        TargetIdeaVersion = 1,
        ProposedTitle = "Updated title here",
        ProposedBody = "Updated body content with enough length to pass validation.",
        AmendmentTitle = "My amendment",
        AmendmentDescription = "This amendment changes something meaningful.",
    };

    public class TheTargetIdeaVersionRule
    {
        [Fact]
        public void GivenVersionOfZero_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { TargetIdeaVersion = 0 };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "TargetIdeaVersion must be a positive integer.");
        }

        [Fact]
        public void GivenNegativeVersion_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { TargetIdeaVersion = -1 };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "TargetIdeaVersion must be a positive integer.");
        }

        [Fact]
        public void GivenVersionOfOne_WhenValidated_ThenNoVersionError()
        {
            var request = ValidRequest() with { TargetIdeaVersion = 1 };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(ProposeAmendmentRequest.TargetIdeaVersion));
        }
    }

    public class TheProposedTitleRule
    {
        [Fact]
        public void GivenEmptyProposedTitle_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { ProposedTitle = string.Empty };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(ProposeAmendmentRequest.ProposedTitle));
        }

        [Fact]
        public void GivenProposedTitleOfTwoChars_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { ProposedTitle = "ab" };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "ProposedTitle must be at least 3 characters long.");
        }

        [Fact]
        public void GivenProposedTitleOf201Chars_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { ProposedTitle = new string('a', 201) };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "ProposedTitle cannot exceed 200 characters.");
        }

        [Fact]
        public void GivenProposedTitleAtMinLength_WhenValidated_ThenNoTitleError()
        {
            var request = ValidRequest() with { ProposedTitle = "abc" };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(ProposeAmendmentRequest.ProposedTitle));
        }

        [Fact]
        public void GivenProposedTitleAtMaxLength_WhenValidated_ThenNoTitleError()
        {
            var request = ValidRequest() with { ProposedTitle = new string('a', 200) };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(ProposeAmendmentRequest.ProposedTitle));
        }
    }

    public class TheProposedBodyRule
    {
        [Fact]
        public void GivenProposedBodyOfNineChars_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { ProposedBody = "123456789" };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "ProposedBody must be at least 10 characters long.");
        }

        [Fact]
        public void GivenProposedBodyOf5001Chars_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { ProposedBody = new string('a', 5001) };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "ProposedBody cannot exceed 5000 characters.");
        }
    }

    public class TheAmendmentDescriptionRule
    {
        [Fact]
        public void GivenDescriptionOfNineChars_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { AmendmentDescription = "123456789" };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "AmendmentDescription must be at least 10 characters long.");
        }

        [Fact]
        public void GivenDescriptionOf1001Chars_WhenValidated_ThenFails()
        {
            var request = ValidRequest() with { AmendmentDescription = new string('a', 1001) };

            var result = new ProposeAmendmentRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "AmendmentDescription cannot exceed 1000 characters.");
        }
    }

    public class TheFullRequest
    {
        [Fact]
        public void GivenValidRequest_WhenValidated_ThenPasses()
        {
            var result = new ProposeAmendmentRequestValidator().Validate(ValidRequest());

            result.IsValid.Should().BeTrue();
        }
    }
}
