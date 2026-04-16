using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class PostInquiryResponseTests(ApiFactory factory)
{
    [Fact(Skip = "Setup requires raising an inquiry, which depends on Societies setup (affected society membership, SocialContract). Revisit when cross-context test helpers exist.")]
    public async Task PostInquiryResponse_without_token_returns_401()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, inquiryId) = await CreateProjectWithInquiryAsync(client);

        var response = await factory.CreateClient().PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/responses",
            new RespondToProjectInquiryRequest("Here is the group's response."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(Skip = "Setup requires raising an inquiry, which depends on Societies setup (affected society membership, SocialContract). Revisit when cross-context test helpers exist.")]
    public async Task PostInquiryResponse_returns_204()
    {
        var client = factory.CreateClient().WithAuthToken();
        var (projectId, inquiryId) = await CreateProjectWithInquiryAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}/responses",
            new RespondToProjectInquiryRequest("Here is the group's response."),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static async Task<(string projectId, string inquiryId)> CreateProjectWithInquiryAsync(HttpClient client)
    {
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        var inquiryResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("What is the timeline?"),
            TestContext.Current.CancellationToken);
        inquiryResponse.EnsureSuccessStatusCode();
        var inquiryId = (await inquiryResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;
        return (projectId, inquiryId);
    }
}
