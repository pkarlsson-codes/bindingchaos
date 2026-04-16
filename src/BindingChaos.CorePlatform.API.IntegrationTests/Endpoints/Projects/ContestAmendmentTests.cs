using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class ContestAmendmentTests(ApiFactory factory)
{
    [Fact]
    public async Task ContestAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, amendmentId) = await CreateProjectWithAmendmentAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments/{amendmentId}/contests",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(Skip = "AmendmentContentionSaga schedules a message with ScheduleDelay, which is not supported in MediatorOnly (no durable transport). Revisit when saga scheduling is supported in tests.")]
    public async Task ContestAmendment_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, amendmentId) = await CreateProjectWithAmendmentAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments/{amendmentId}/contests",
            new { },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<(string projectId, string amendmentId)> CreateProjectWithAmendmentAsync(HttpClient client)
    {
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);
        var projectResponse = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Contest Test Project", "Description."),
            TestContext.Current.CancellationToken);
        projectResponse.EnsureSuccessStatusCode();
        var projectId = (await projectResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var amendmentResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/amendments",
            new { },
            TestContext.Current.CancellationToken);
        amendmentResponse.EnsureSuccessStatusCode();
        var amendmentId = (await amendmentResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        return (projectId, amendmentId);
    }
}
