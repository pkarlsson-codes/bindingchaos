using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupDetailTests(ApiFactory factory)
{
    private const string NonExistentId = "usergroup-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetUserGroupDetail_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().WithAuthToken().GetAsync(
            $"/api/usergroups/{NonExistentId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserGroupDetail_returns_group_data()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);
        var groupId = await FormUserGroupAsync(client, commonsId, "Detail Test Group");

        var response = await client.GetAsync(
            $"/api/usergroups/{groupId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Name.Should().Be("Detail Test Group");
        body.Data.CommonsId.Should().Be(commonsId);
    }

    [Fact]
    public async Task GetUserGroupDetail_IsMember_is_true_for_founding_member()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var founderClient = factory.CreateClient().WithAuthToken(participantId);
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, "Membership Check Group");

        var response = await founderClient.GetAsync(
            $"/api/usergroups/{groupId}",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.IsMember.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserGroupDetail_IsMember_is_false_for_non_member()
    {
        var founderClient = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(founderClient);
        var groupId = await FormUserGroupAsync(founderClient, commonsId, "Non Member Group");

        // Different participant queries the group
        var outsiderClient = factory.CreateClient().WithAuthToken();
        var response = await outsiderClient.GetAsync(
            $"/api/usergroups/{groupId}",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupDetailResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.IsMember.Should().BeFalse();
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

    private static async Task<string> FormUserGroupAsync(HttpClient client, string commonsId, string name)
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
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
