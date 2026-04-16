using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetMyUserGroupsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetMyUserGroups_without_token_returns_401()
    {
        var response = await factory.CreateClient().GetAsync(
            "/api/usergroups/mine",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyUserGroups_returns_only_groups_founded_by_caller()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var callerClient = factory.CreateClient().WithAuthToken(participantId);
        var otherClient = factory.CreateClient().WithAuthToken();

        // Caller forms a group
        var commonsId = await CreateCommonsAsync(callerClient);
        await FormUserGroupAsync(callerClient, commonsId, "Caller Group");

        // Another participant forms a different group
        var otherCommonsId = await CreateCommonsAsync(otherClient);
        await FormUserGroupAsync(otherClient, otherCommonsId, "Other Group");

        var response = await callerClient.GetAsync(
            "/api/usergroups/mine",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupListItemResponse[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(g => g.Name == "Caller Group");
        body.Data.Should().NotContain(g => g.Name == "Other Group");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons."),
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
