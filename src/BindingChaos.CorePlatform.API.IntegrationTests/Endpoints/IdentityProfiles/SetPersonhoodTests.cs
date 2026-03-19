using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.IdentityProfiles;

[Collection("integration")]
public class SetPersonhoodTests(ApiFactory factory)
{
    [Fact]
    public async Task SetPersonhood_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/identity/users/some-user-id/personhood",
            new { Verified = true },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
