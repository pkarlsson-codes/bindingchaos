using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class CreateAmendmentTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAmendment_creates_amendment_and_returns_201()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var amendmentId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        amendmentId.Should().NotBeNullOrWhiteSpace();
    }

    private static async Task<string> CreateProjectAsync(HttpClient client)
    {
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);
        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Amendment Test Project", "Description."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
