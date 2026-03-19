using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Signals;

[Collection("integration")]
public class AmplifySignalTests(ApiFactory factory)
{
    [Fact]
    public async Task AmplifySignal_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/amplifications",
            new { Reason = "HighRelevance" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
