using System.Net;
using System.Net.Http.Json;
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

        var concernId = await CreateConcernAsync(client);

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

    private static async Task<string> CreateConcernAsync(HttpClient client)
    {
        var signalId = await CreateSignalAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/concerns",
            ValidRaiseConcernRequest(signalId),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }

    private static async Task<string> CreateSignalAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/signals",
            new CaptureSignalRequest(
                Title: "Signal for concern listing",
                Description: "Signal created for GetConcernsForCommons integration tests.",
                Tags: ["testing"],
                AttachmentIds: [],
                Latitude: null,
                Longitude: null),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var signalId = response.Headers.Location?.Segments.LastOrDefault()?.Trim('/');
        signalId.Should().NotBeNullOrWhiteSpace();
        return signalId!;
    }

    private static RaiseConcernRequest ValidRaiseConcernRequest(string signalId) => new(
        Name: "Test Concern",
        Tags: ["environment"],
        SignalIds: [signalId],
        Origin: ConcernOriginDto.Manual);
}
