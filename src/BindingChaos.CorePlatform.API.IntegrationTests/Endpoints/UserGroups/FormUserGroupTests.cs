using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.UserGroups;

[Collection("integration")]
public class FormUserGroupTests(ApiFactory factory)
{
    [Fact]
    public async Task FormUserGroup_without_token_returns_401()
    {
        var client = factory.CreateClient();
        var commonsId = await CreateCommonsAsync(factory.CreateClient().WithAuthToken());

        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            ValidFormUserGroupRequest(commonsId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FormUserGroup_with_valid_request_returns_201_with_location()
    {
        var client = factory.CreateClient().WithAuthToken();
        var commonsId = await CreateCommonsAsync(client);

        var response = await client.PostAsJsonAsync(
            "/api/usergroups",
            ValidFormUserGroupRequest(commonsId),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/usergroups/");
    }

    private static async Task<string> CreateCommonsAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/commons",
            new ProposeCommonsRequest("Test Commons", "A commons for user group tests."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<BindingChaos.Infrastructure.API.ApiResponse<string>>(
            TestContext.Current.CancellationToken);
        return body!.Data!;
    }

    private static FormUserGroupRequest ValidFormUserGroupRequest(string commonsId) => new(
        CommonsId: commonsId,
        Name: "Test User Group",
        Philosophy: "We exist to govern this commons fairly.",
        Charter: new UserGroupCharterDto(
            ContestationRules: new UserGroupContestationRulesDto(
                ResolutionWindow: TimeSpan.FromDays(3),
                RejectionThreshold: 0.4m),
            MembershipRules: new UserGroupMembershipRulesDto(
                JoinPolicy: UserGroupJoinPolicyDto.Open,
                ApprovalSettings: null,
                MaxMembers: null,
                EntryRequirements: null,
                MemberListPublic: true),
            ShunningRules: new UserGroupShunningRulesDto(ApprovalThreshold: 0.6m)));
}
