using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.IdentityProfiles;

[Collection("integration")]
public class LinkTests(ApiFactory factory)
{
    [Fact]
    public async Task Link_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/identity/users/link",
            new { Provider = "google", Subject = "someone@example.com" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
