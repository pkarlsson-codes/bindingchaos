using System.Security.Claims;
using BindingChaos.CorePlatform.API.Infrastructure.Extensions;
using BindingChaos.SharedKernel.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace BindingChaos.CorePlatform.API.Tests.Infrastructure.Extensions;

public class HttpContextParticipantExtensionsTests
{
    public class TheGetParticipantIdOrAnonymousMethod
    {
        [Fact]
        public void GivenParticipantIdClaim_WhenResolved_ThenReturnsParticipantId()
        {
            var context = new DefaultHttpContext();
            context.User = UserWith(new Claim("participant_id", "participant-01arz3ndektsv4rrffq69g5fav"));

            var result = context.GetParticipantIdOrAnonymous();

            result.Value.Should().Be("participant-01arz3ndektsv4rrffq69g5fav");
            result.IsAnonymous.Should().BeFalse();
        }

        [Fact]
        public void GivenNoParticipantIdClaim_WhenResolved_ThenReturnsAnonymous()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity());

            var result = context.GetParticipantIdOrAnonymous();

            result.IsAnonymous.Should().BeTrue();
        }

        [Fact]
        public void GivenParticipantIdClaimWithEmptyValue_WhenResolved_ThenReturnsAnonymous()
        {
            var context = new DefaultHttpContext();
            context.User = UserWith(new Claim("participant_id", string.Empty));

            var result = context.GetParticipantIdOrAnonymous();

            result.IsAnonymous.Should().BeTrue();
        }

        [Fact]
        public void GivenParticipantIdClaimWithWhitespace_WhenResolved_ThenReturnsAnonymous()
        {
            var context = new DefaultHttpContext();
            context.User = UserWith(new Claim("participant_id", "   "));

            var result = context.GetParticipantIdOrAnonymous();

            result.IsAnonymous.Should().BeTrue();
        }

        [Fact]
        public void GivenNullHttpContext_WhenResolved_ThenThrows()
        {
            HttpContext context = null!;

            var act = () => context.GetParticipantIdOrAnonymous();

            act.Should().Throw<ArgumentNullException>();
        }

        private static ClaimsPrincipal UserWith(params Claim[] claims)
            => new(new ClaimsIdentity(claims, authenticationType: "test"));
    }
}
