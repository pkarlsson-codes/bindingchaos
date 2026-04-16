using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class CreateInquiryTests(ApiFactory factory)
{
    [Fact]
    public async Task CreateInquiry_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Why is this project needed?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(Skip = "RaiseProjectInquiry requires the actor to be an active member of a society affected by the project's commons, plus a SocialContract and a scheduled ScheduleInquiryLapse message. Societies setup not available in integration tests; revisit when cross-context test helpers exist.")]
    public async Task CreateInquiry_returns_201_with_inquiry_id()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateProjectAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Why is this project needed?"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var inquiryId = (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data;
        inquiryId.Should().NotBeNullOrWhiteSpace();
    }

    internal static async Task<string> CreateProjectAsync(HttpClient client)
    {
        var userGroupId = await CreateProjectTests.CreateUserGroupAsync(client);
        var response = await client.PostAsJsonAsync(
            "/api/projects",
            new CreateProjectRequest(userGroupId, "Inquiry Test Project", "Description."),
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
    }
}
