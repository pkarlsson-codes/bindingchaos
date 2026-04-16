using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class GetUserGroupsForParticipantTests(ApiFactory factory)
{
    [Fact]
    public async Task GetUserGroupsForParticipant_without_participantId_returns_400()
    {
        var response = await factory.CreateClient().WithAuthToken().GetAsync(
            "/api/usergroups/for-participant",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetUserGroupsForParticipant_returns_groups_for_participant()
    {
        var participantId = BindingChaos.SharedKernel.Domain.ParticipantId.Generate().Value;
        var client = factory.CreateClient().WithAuthToken(participantId);

        var commonsId = await CreateCommonsAsync(client);
        await FormUserGroupAsync(client, commonsId, "Participant Group");

        var response = await factory.CreateClient().WithAuthToken().GetAsync(
            $"/api/usergroups/for-participant?participantId={participantId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<UserGroupListItemResponse[]>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(g => g.Name == "Participant Group");
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
