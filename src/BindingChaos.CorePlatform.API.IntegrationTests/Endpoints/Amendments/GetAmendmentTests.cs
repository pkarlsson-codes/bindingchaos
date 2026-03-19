using System.Net;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Amendments;

[Collection("integration")]
public class GetAmendmentTests(ApiFactory factory)
{
    private const string NonExistentAmendmentId = "amendment-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/amendments/{NonExistentAmendmentId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
