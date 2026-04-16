using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetInquiryTests(ApiFactory factory)
{
    private const string NonExistentInquiryId = "inquiry-01aryz6s41tptwgy5wdss56yxy";
    private const string SomProjectId = "project-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task GetInquiry_for_nonexistent_id_returns_404()
    {
        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{SomProjectId}/inquiries/{NonExistentInquiryId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires a raised inquiry, which depends on Societies setup (affected society membership, SocialContract). Revisit when cross-context test helpers exist.")]
    public async Task GetInquiry_returns_raised_inquiry()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        var createResponse = await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("Specific inquiry body."),
            TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var inquiryId = (await createResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(TestContext.Current.CancellationToken))!.Data!;

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{projectId}/inquiries/{inquiryId}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectInquiryResponse>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Body.Should().Be("Specific inquiry body.");
    }
}
