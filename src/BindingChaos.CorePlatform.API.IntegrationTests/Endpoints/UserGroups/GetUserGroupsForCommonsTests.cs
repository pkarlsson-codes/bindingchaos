using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupsForCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetUserGroupsForCommons_without_commonsId_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.GetAsync(
            "/api/usergroups",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetUserGroupsForCommons_returns_group_formed_for_commons()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);
        await FormUserGroupAsync(client, commonsId, "Retrievable Group");

        var response = await client.GetAsync(
            $"/api/usergroups?commonsId={commonsId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserGroupListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Items.Should().Contain(g => g.Name == "Retrievable Group");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "Commons for list tests."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task FormUserGroupAsync(HttpClient client, string commonsId, string name)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: name,
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, true),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
