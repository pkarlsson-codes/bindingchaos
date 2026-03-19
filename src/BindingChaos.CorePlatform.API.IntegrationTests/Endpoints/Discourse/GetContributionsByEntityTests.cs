using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Discourse;

[Collection("integration")]
public class GetContributionsByEntityTests(ApiFactory factory)
{
    [Fact]
    public async Task GetContributionsByEntity_without_auth_returns_404_when_thread_not_found()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/discourse/threads/by-entity/idea/idea-01aryz6s41tptwgy5wdss56yxy/contributions",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
