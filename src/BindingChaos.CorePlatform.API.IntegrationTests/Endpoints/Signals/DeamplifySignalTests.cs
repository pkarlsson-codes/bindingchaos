using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Signals;

[Collection("integration")]
public class DeamplifySignalTests(ApiFactory factory)
{
    [Fact]
    public async Task DeamplifySignal_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.DeleteAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/amplifications",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
