using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class CreateProjectTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateProject_without_token_returns_401()
    {
        var setupClient = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateUserGroupAsync(setupClient);

        var response = await factory.CreateClient().PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Test Project", "A project description."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProject_with_valid_request_returns_201_with_location()
    {
        var client = factory.CreateClient().WithAuthToken();
        var userGroupId = await CreateUserGroupAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Test Project", "A project description."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/projects/");
    }

    internal static async Task<string> CreateUserGroupAsync(HttpClient client)
    {
        var commonsResponse = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Project Test Commons", "A commons."),
            TestContext.Current.CancellationToken);
        commonsResponse.EnsureSuccessStatusCode();
        var commonsId = (await commonsResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var groupResponse = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: "Project Test Group",
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        groupResponse.EnsureSuccessStatusCode();
        return (await groupResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
