using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Projects;

[Collection("integration")]
public class GetInquiriesTests(ApiFactory factory)
{
    [Fact(Skip = "Requires a raised inquiry, which depends on Societies setup (affected society membership, SocialContract). Revisit when cross-context test helpers exist.")]
    public async Task GetInquiries_returns_raised_inquiry()
    {
        var client = factory.CreateClient().WithAuthToken();
        var projectId = await CreateInquiryTests.CreateProjectAsync(client);
        await client.PostAsJsonAsync(
            $"/api/projects/{projectId}/inquiries",
            new RaiseProjectInquiryRequest("What is the scope of this project?"),
            TestContext.Current.CancellationToken);

        var response = await factory.CreateClient().GetAsync(
            $"/api/projects/{projectId}/inquiries",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<ProjectInquiryResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.Items.Should().Contain(i => i.Body == "What is the scope of this project?");
    }
}
