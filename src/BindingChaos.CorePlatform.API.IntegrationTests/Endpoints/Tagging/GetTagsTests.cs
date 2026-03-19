using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Tagging;

[Collection("integration")]
public class GetTagsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetTags_without_auth_returns_200()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/tags?limit=10",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTags_returns_wrapped_response()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/tags?limit=10",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonDocument.Parse(body);

        json.RootElement.ValueKind.Should().Be(JsonValueKind.Object);
        json.RootElement.GetProperty("data").ValueKind.Should().Be(JsonValueKind.Array);
    }
}
