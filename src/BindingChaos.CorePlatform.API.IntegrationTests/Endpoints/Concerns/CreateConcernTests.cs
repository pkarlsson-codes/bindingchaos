using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Concerns;

[Collection("integration")]
public class CreateConcernTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateConcern_returns_201_with_concern_id()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/concerns",
            await ValidRaiseConcernRequestAsync(client),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var concernId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        concernId.Should().NotBeNullOrWhiteSpace();
    }

    internal static async Task<string> CreateConcernAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/concerns",
            await ValidRaiseConcernRequestAsync(client),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<RaiseConcernRequest> ValidRaiseConcernRequestAsync(HttpClient client)
    {
        var signalId = await CreateSignalAsync(client);

        return new RaiseConcernRequest(
            Name: "Test Concern",
            Tags: ["environment"],
            SignalIds: [signalId],
            Origin: ConcernOriginDto.Manual);
    }

    private static async Task<string> CreateSignalAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/signals",
            new CaptureSignalRequest(
                Title: "Signal for concern creation",
                Description: "Signal created for CreateConcern integration tests.",
                Tags: ["testing"],
                AttachmentIds: [],
                Latitude: null,
                Longitude: null),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var signalId = response.Headers.Location?.Segments.LastOrDefault()?.Trim('/');
        signalId.Should().NotBeNullOrWhiteSpace();
        return signalId!;
    }
}
