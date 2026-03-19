using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Signals;

[Collection("integration")]
public class AddEvidenceTests(ApiFactory factory)
{
    [Fact]
    public async Task AddEvidence_without_token_return_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/evidence",
            new { DocumentIds = Array.Empty<string>(), Description = "description" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddEvidence_with_nonexistent_signal_return_404()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/evidence",
            new { DocumentIds = Array.Empty<string>(), Description = "description" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}