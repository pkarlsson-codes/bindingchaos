using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetProjectsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetProjects_without_userGroupId_returns_400()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/projects",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetProjects_returns_projects_scoped_to_user_group()
    {
        var client = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);

        await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Group Scoped Project", "Description."),
            TestContext.Current.CancellationToken);

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects?userGroupId={userGroupId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProjectListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Items.Should().Contain(p => p.Title == "Group Scoped Project");
    }
}
