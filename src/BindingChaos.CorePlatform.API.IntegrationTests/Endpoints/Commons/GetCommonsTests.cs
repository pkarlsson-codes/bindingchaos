using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Commons;

[Collection("integration")]
public class GetCommonsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetCommons_returns_proposed_commons()
    {
        var client = factory.CreateClient().WithAuthToken();
        await ProposeCommonsTests.CreateCommonsAsync(client);

        var response = await client.GetAsync(
            "/api/commons",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResponse<CommonsListItemResponse>>>(
            TestContext.Current.CancellationToken);
        body!.Data!.TotalCount.Should().BeGreaterThan(0);
    }
}
