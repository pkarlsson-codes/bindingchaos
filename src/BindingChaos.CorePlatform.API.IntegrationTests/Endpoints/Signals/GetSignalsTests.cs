using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Signals;

[Collection("integration")]
public class GetSignalsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetSignals_without_auth_returns_200()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/signals", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSignals_returns_paginated_response()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/signals", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonDocument.Parse(body);

        json.RootElement.ValueKind.Should().Be(JsonValueKind.Object);
    }
}
