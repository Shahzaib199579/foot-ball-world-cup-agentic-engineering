using System.Net;
using System.Net.Http.Json;
using WorldCupScoreboard.Api.Contracts;

namespace WorldCupScoreboard.Api.Tests;

public class GetMatchEndpointTests : IClassFixture<ScoreboardApiFactory>
{
    private readonly HttpClient _client;

    public GetMatchEndpointTests(ScoreboardApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMatch_WithExistingId_Returns200WithMatchBody()
    {
        var startRequest = new StartMatchRequest("Germany", "France", DateTime.UtcNow, "Munich Arena");
        var startResponse = await _client.PostAsJsonAsync("/matches", startRequest);
        var started = await startResponse.Content.ReadFromJsonAsync<MatchResponse>();

        var response = await _client.GetAsync($"/matches/{started!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var match = await response.Content.ReadFromJsonAsync<MatchResponse>();
        Assert.NotNull(match);
        Assert.Equal(started.Id, match!.Id);
        Assert.Equal("Germany", match.HomeTeam.Name);
    }

    [Fact]
    public async Task GetMatch_WithUnknownId_Returns404WithMatchNotFoundError()
    {
        var response = await _client.GetAsync("/matches/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("match_not_found", error!.ErrorCode);
        Assert.False(string.IsNullOrEmpty(error.ErrorMessage));
    }
}
