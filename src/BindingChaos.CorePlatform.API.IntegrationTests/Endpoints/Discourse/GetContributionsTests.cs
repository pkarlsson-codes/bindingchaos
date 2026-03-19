using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Discourse;

[Collection("integration")]
public class GetContributionsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetContributions_without_auth_returns_404_when_thread_not_found()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/discourse/threads/non-existent-thread/contributions",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
