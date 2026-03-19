using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Amendments;

[Collection("integration")]
public class SupportAmendmentTests(ApiFactory factory)
{
    private const string NonExistentAmendmentId = "amendment-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task SupportAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            $"/api/amendments/{NonExistentAmendmentId}/supporters",
            new SupportAmendmentRequest("I agree with this amendment"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
