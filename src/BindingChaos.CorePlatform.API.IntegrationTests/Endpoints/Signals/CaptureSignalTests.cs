using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Signals;

[Collection("integration")]
public class CaptureSignalTests(ApiFactory factory)
{
    [Fact]
    public async Task CaptureSignal_with_empty_title_returns_422()
    {
        var client = factory.CreateClient();
        var request = ValidCaptureSignalRequest() with { Title = "" };

        var response = await client.PostAsJsonAsync("/api/signals", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CaptureSignal_with_empty_description_returns_422()
    {
        var client = factory.CreateClient();
        var request = ValidCaptureSignalRequest() with { Description = "" };

        var response = await client.PostAsJsonAsync("/api/signals", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private static CaptureSignalRequest ValidCaptureSignalRequest() => new(
        Title: "A well-formed signal title",
        Description: "This is a sufficiently detailed description of the signal.",
        Tags: [],
        Attachments: [],
        Latitude: null,
        Longitude: null);
}
