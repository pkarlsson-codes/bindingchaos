using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Concerns;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class GetConcernsForCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetConcernsForCommons_returns_linked_concerns()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await ProposeCommonsTests.CreateCommonsAsync(client);
        await FormUserGroupAsync(client, commonsId, "Commons Linkers");

        var concernId = await CreateConcernTests.CreateConcernAsync(client);

        var linkResponse = await client.PostAsJsonAsync(
            $"/api/commons/{commonsId}/concerns/{concernId}",
            new { },
            TestContext.Current.CancellationToken);
        linkResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var response = await client.GetAsync(
            $"/api/commons/{commonsId}/concerns",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<List<ConcernListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data.Should().Contain(c => c.Id == concernId);
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
