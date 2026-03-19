using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Amendments;

[Collection("integration")]
public class GetAmendmentsTests(ApiFactory factory)
{
    private const string NonExistentIdeaId = "idea-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetAmendments_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/ideas/{NonExistentIdeaId}/amendments",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
