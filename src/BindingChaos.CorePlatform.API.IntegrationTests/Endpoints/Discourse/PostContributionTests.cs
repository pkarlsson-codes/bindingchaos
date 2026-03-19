using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Discourse;

[Collection("integration")]
public class PostContributionTests(ApiFactory factory)
{
    [Fact]
    public async Task PostContribution_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/discourse/threads/some-thread/contributions",
            new PostContributionRequest("Some content", null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostContribution_with_empty_content_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/discourse/threads/some-thread/contributions",
            new PostContributionRequest("", null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task PostContribution_with_short_content_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/discourse/threads/some-thread/contributions",
            new PostContributionRequest("AB", null), // min length is 3
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
