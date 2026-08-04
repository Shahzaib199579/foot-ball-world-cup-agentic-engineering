using System.Net;
using System.Net.Http.Json;
using WorldCupScoreboard.Api.Contracts;

namespace WorldCupScoreboard.Api.Tests;

public class FinishMatchEndpointTests : IClassFixture<ScoreboardApiFactory>
{
    private readonly HttpClient _client;

    public FinishMatchEndpointTests(ScoreboardApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> StartMatchAsync(string home, string away)
    {
        var startResponse = await _client.PostAsJsonAsync(
            "/matches", new StartMatchRequest(home, away, DateTime.UtcNow, $"{home}-{away} Stadium"));
        var started = await startResponse.Content.ReadFromJsonAsync<MatchResponse>();
        return started!.Id;
    }

    [Fact]
    public async Task PostFinish_WithInProgressMatch_Returns200WithFinishedMatch()
    {
        var matchId = await StartMatchAsync("Mexico", "Canada");

        var response = await _client.PostAsync($"/matches/{matchId}/finish", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var match = await response.Content.ReadFromJsonAsync<MatchResponse>();
        Assert.NotNull(match);
        Assert.Equal((int)WorldCupScoreboard.MatchStatus.Finished, match!.Status);
    }

    [Fact]
    public async Task PostFinish_WithAlreadyFinishedMatch_Returns404WithMatchNotFoundError()
    {
        var matchId = await StartMatchAsync("Spain", "Brazil");
        await _client.PostAsync($"/matches/{matchId}/finish", null);

        var response = await _client.PostAsync($"/matches/{matchId}/finish", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("match_not_found", error!.ErrorCode);
    }

    [Fact]
    public async Task PostFinish_WithUnknownMatchId_Returns404WithMatchNotFoundError()
    {
        var response = await _client.PostAsync("/matches/999999/finish", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("match_not_found", error!.ErrorCode);
    }
}
