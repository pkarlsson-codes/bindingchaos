using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Ideas;

[Collection("integration")]
public class GetIdeaEndpointTests(ApiFactory factory)
{
    private const string NonExistentIdeaId = "stigmergyidea-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetIdea_with_unknown_id_returns_404()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/ideas/{NonExistentIdeaId}", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound, because: body);
    }
}
