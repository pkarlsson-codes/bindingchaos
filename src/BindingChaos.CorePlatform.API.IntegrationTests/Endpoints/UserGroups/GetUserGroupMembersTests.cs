using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupMembersTests(ApiFactory factory)
{
    private const string NonExistentId = "usergroup-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetUserGroupMembers_for_nonexistent_group_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/usergroups/{NonExistentId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserGroupMembers_for_public_group_returns_members()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);
        var groupId = await FormUserGroupAsync(client, commonsId, memberListPublic: true);

        var response = await factory.CreateClient().GetAsync(
            $"/api/usergroups/{groupId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserGroupMemberResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUserGroupMembers_for_private_group_and_non_member_returns_403()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, memberListPublic: false);

        var outsiderClient = factory.CreateClient().WithAuthToken();
        var response = await outsiderClient.GetAsync(
            $"/api/usergroups/{groupId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserGroupMembers_for_private_group_and_member_returns_members()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var founderClient = factory.CreateClient().WithAuthToken(participantId);
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, memberListPublic: false);

        // Founder is a member — should see the list
        var response = await founderClient.GetAsync(
            $"/api/usergroups/{groupId}/members",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<UserGroupMemberResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
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

    private static async Task<string> FormUserGroupAsync(HttpClient client, string commonsId, bool memberListPublic)
    {
        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            new FormUserGroupRequest(
                CommonsId: commonsId,
                Name: "Members Test Group",
                Philosophy: "Philosophy",
                Charter: new UserGroupCharterDto(
                    new UserGroupContestationRulesDto(TimeSpan.FromDays(3), 0.4m),
                    new UserGroupMembershipRulesDto(UserGroupJoinPolicyDto.Open, null, null, null, memberListPublic),
                    new UserGroupShunningRulesDto(0.6m))),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
