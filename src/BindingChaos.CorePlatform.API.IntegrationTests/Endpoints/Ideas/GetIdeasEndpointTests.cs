using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Ideas;

[Collection("integration")]
public class GetIdeasEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task GetIdeas_without_auth_returns_200()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/ideas", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetIdeas_returns_paginated_response()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/ideas", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonDocument.Parse(body);

        json.RootElement.ValueKind.Should().Be(JsonValueKind.Object);
    }
}