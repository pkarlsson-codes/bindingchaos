using System.Net;
using System.Net.Http.Json;
using BindingChaos.CorePlatform.Contracts.Requests;
using FluentAssertions;

namespace BindingChaos.CorePlatform.API.IntegrationTests.Endpoints.Signals;

[Collection("integration")]
public class SuggestionActionTests(ApiFactory factory)
{

    [Fact]
    public async Task SuggestAction_without_token_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/suggested-actions",
            new SuggestActionRequest("MakeACall", "555-1234", null, null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SuggestAction_with_empty_action_type_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/suggested-actions",
            new SuggestActionRequest("", null, null, null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SuggestAction_with_unknown_action_type_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/suggested-actions",
            new SuggestActionRequest("DoSomethingElse", null, null, null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SuggestAction_MakeACall_without_phone_number_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/suggested-actions",
            new SuggestActionRequest("MakeACall", null, null, null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SuggestAction_VisitAWebpage_without_url_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/suggested-actions",
            new SuggestActionRequest("VisitAWebpage", null, null, null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SuggestAction_VisitAWebpage_with_non_absolute_url_returns_422()
    {
        var client = factory.CreateClient().WithAuthToken();

        var response = await client.PostAsJsonAsync(
            "/api/signals/signal-01aryz6s41tptwgy5wdss56yxy/suggested-actions",
            new SuggestActionRequest("VisitAWebpage", null, "not-a-url", null),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

}
