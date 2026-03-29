using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Ideas;

[Collection("integration")]
public partial class AuthorIdeaEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task AuthorIdea_without_token_returns_401()
    {
        var client = factory.CreateClient();
        var request = ValidAuthorIdeaRequest();

        var response = await client.PostAsJsonAsync("/api/ideas", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthorIdea_with_empty_title_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidAuthorIdeaRequest() with { Title = "" };

        var response = await client.PostAsJsonAsync("/api/ideas", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task AuthorIdea_with_short_title_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidAuthorIdeaRequest() with { Title = "AB" };

        var response = await client.PostAsJsonAsync("/api/ideas", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task AuthorIdea_with_empty_description_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidAuthorIdeaRequest() with { Description = "" };

        var response = await client.PostAsJsonAsync("/api/ideas", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task AuthorIdea_with_short_description_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();
        var request = ValidAuthorIdeaRequest() with { Description = "Too short" };

        var response = await client.PostAsJsonAsync("/api/ideas", request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private static AuthorIdeaRequest ValidAuthorIdeaRequest() => new(
        Title: "A well-formed test idea title",
        Description: "This is a sufficiently long description for the test idea to pass validation.");
}
