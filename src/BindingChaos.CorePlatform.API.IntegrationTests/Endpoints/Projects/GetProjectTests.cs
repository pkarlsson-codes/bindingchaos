using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetProjectTests(ApiFactory factory)
{
    private const string NonExistentId = "project-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetProject_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{NonExistentId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProject_returns_created_project()
    {
        var client = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);

        var createResponse = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Get Test Project", "Description."),
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var projectId = (await createResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{projectId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Title.Should().Be("Get Test Project");
        body.Data.UserGroupId.Should().Be(userGroupId);
    }
}
