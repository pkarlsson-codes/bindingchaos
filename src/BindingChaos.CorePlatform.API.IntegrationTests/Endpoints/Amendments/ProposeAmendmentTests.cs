using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Amendments;

[Collection("integration")]
public class ProposeAmendmentTests(ApiFactory factory)
{
    private const string NonExistentIdeaId = "idea-01aryz6s41tptwgy5wdss56yxy";

    [Fact]
    public async Task ProposeAmendment_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            $"/api/ideas/{NonExistentIdeaId}/amendments",
            ValidProposeAmendmentRequest(),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProposeAmendment_with_empty_proposed_title_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidProposeAmendmentRequest();
        request.ProposedTitle = "";

        var response = await client.PostAsJsonAsync(
            $"/api/ideas/{NonExistentIdeaId}/amendments",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ProposeAmendment_with_empty_proposed_body_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidProposeAmendmentRequest();
        request.ProposedBody = "";

        var response = await client.PostAsJsonAsync(
            $"/api/ideas/{NonExistentIdeaId}/amendments",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task ProposeAmendment_with_zero_version_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidProposeAmendmentRequest();
        request.TargetIdeaVersion = 0;

        var response = await client.PostAsJsonAsync(
            $"/api/ideas/{NonExistentIdeaId}/amendments",
            request,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private static ProposeAmendmentRequest ValidProposeAmendmentRequest() => new()
    {
        TargetIdeaVersion = 1,
        ProposedTitle = "An improved title for the idea",
        ProposedBody = "This is the improved body text for the idea, with more detail.",
        AmendmentTitle = "My improvement",
        AmendmentDescription = "This amendment improves the idea by adding more clarity.",
    };
}
