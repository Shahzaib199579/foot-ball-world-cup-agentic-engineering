using System.Net;
using System.Net.Http.Json;
using WorldCupScoreboard.Api.Contracts;

namespace WorldCupScoreboard.Api.Tests;

public class UpdateScoreEndpointTests : IClassFixture<ScoreboardApiFactory>
{
    private readonly HttpClient _client;

    public UpdateScoreEndpointTests(ScoreboardApiFactory factory)
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
    public async Task PutScore_WithValidScores_Returns200WithUpdatedMatch()
    {
        var matchId = await StartMatchAsync("Uruguay", "Italy");

        var response = await _client.PutAsJsonAsync($"/matches/{matchId}/score", new UpdateScoreRequest(6, 6));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var match = await response.Content.ReadFromJsonAsync<MatchResponse>();
        Assert.NotNull(match);
        Assert.Equal(6, match!.HomeTeam.Score);
        Assert.Equal(6, match.AwayTeam.Score);
    }

    [Fact]
    public async Task PutScore_WithUnknownMatchId_Returns404WithMatchNotFoundError()
    {
        var response = await _client.PutAsJsonAsync("/matches/999999/score", new UpdateScoreRequest(1, 0));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("match_not_found", error!.ErrorCode);
    }

    [Fact]
    public async Task PutScore_WithNegativeScore_Returns400WithInvalidScoreError()
    {
        var matchId = await StartMatchAsync("Argentina", "Australia");

        var response = await _client.PutAsJsonAsync($"/matches/{matchId}/score", new UpdateScoreRequest(-1, 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("invalid_score", error!.ErrorCode);
        Assert.False(string.IsNullOrEmpty(error.ErrorMessage));
    }
}
