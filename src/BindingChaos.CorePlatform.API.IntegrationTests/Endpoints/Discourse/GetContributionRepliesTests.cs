using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Discourse;

[Collection("integration")]
public class GetContributionRepliesTests(ApiFactory factory)
{
    [Fact]
    public async Task GetContributionReplies_without_auth_returns_404_when_contribution_not_found()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/discourse/contributions/non-existent-contribution/replies",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
