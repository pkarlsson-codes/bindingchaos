using BindingChaos.CorePlatform.API.Validators;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.Tests.Validators;

public class AmplifySignalRequestValidatorTests
{
    public class TheReasonRule
    {
        [Fact]
        public void GivenNullReason_WhenValidated_ThenPasses()
        {
            var request = new AmplifySignalRequest(Reason: null, Commentary: null);

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(AmplifySignalRequest.Reason));
        }

        [Fact]
        public void GivenEmptyReason_WhenValidated_ThenPasses()
        {
            var request = new AmplifySignalRequest(Reason: string.Empty, Commentary: null);

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(AmplifySignalRequest.Reason));
        }

        [Fact]
        public void GivenWhitespaceOnlyReason_WhenValidated_ThenPasses()
        {
            var request = new AmplifySignalRequest(Reason: "   ", Commentary: null);

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(AmplifySignalRequest.Reason));
        }

        [Fact]
        public void GivenValidReasonDisplayName_WhenValidated_ThenPasses()
        {
            var request = new AmplifySignalRequest(Reason: "HighRelevance", Commentary: null);

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(AmplifySignalRequest.Reason));
        }

        [Fact]
        public void GivenUnrecognisedReason_WhenValidated_ThenFails()
        {
            var request = new AmplifySignalRequest(Reason: "NotARealReason", Commentary: null);

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AmplifySignalRequest.Reason));
        }
    }

    public class TheCommentaryRule
    {
        [Fact]
        public void GivenCommentaryOf2001Chars_WhenValidated_ThenFails()
        {
            var request = new AmplifySignalRequest(Reason: null, Commentary: new string('a', 2001));

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Commentary cannot exceed 2000 characters.");
        }

        [Fact]
        public void GivenNullCommentary_WhenValidated_ThenPasses()
        {
            var request = new AmplifySignalRequest(Reason: null, Commentary: null);

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void GivenCommentaryAtMaxLength_WhenValidated_ThenPasses()
        {
            var request = new AmplifySignalRequest(Reason: null, Commentary: new string('a', 2000));

            var result = new AmplifySignalRequestValidator().Validate(request);

            result.Errors.Should().NotContain(e => e.PropertyName == nameof(AmplifySignalRequest.Commentary));
        }
    }
}
