using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.IdentityProfiles;

[Collection("integration")]
public class GetUserTests(ApiFactory factory)
{
    [Fact]
    public async Task GetUser_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/identity/users/some-user-id",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUser_with_unknown_id_returns_404()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.GetAsync(
            "/api/identity/users/user-that-does-not-exist",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
