using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class ProposeCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task ProposeCommons_without_token_returns_401()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Some Commons", "A description."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProposeCommons_returns_201_with_commons_id()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("New Commons", "A description of the commons."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var commonsId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        commonsId.Should().NotBeNullOrWhiteSpace();
    }

    internal static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
