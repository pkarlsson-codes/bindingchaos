using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Smoke;

/// <summary>
/// Smoke tests — the bare minimum to know the app is alive.
/// These are the first tests to run and the first to fail if something fundamental breaks.
/// </summary>
[Collection("integration")]
public class AppStartupTests(ApiFactory factory)
{
    [Fact]
    public async Task Health_endpoint_returns_healthy()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Protected_endpoint_returns_401_without_token()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsync("/api/ideas", null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Anonymous_endpoint_returns_200_without_token()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/ideas", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
