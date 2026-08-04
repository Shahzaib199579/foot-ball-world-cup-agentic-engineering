using System.Net;
using System.Net.Http.Json;
using WorldCupScoreboard.Api.Contracts;

namespace WorldCupScoreboard.Api.Tests;

public class StartMatchEndpointTests : IClassFixture<ScoreboardApiFactory>
{
    private readonly HttpClient _client;

    public StartMatchEndpointTests(ScoreboardApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostMatches_WithValidRequest_Returns201WithMatchBody()
    {
        var request = new StartMatchRequest("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");

        var response = await _client.PostAsJsonAsync("/matches", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var match = await response.Content.ReadFromJsonAsync<MatchResponse>();
        Assert.NotNull(match);
        Assert.Equal("Mexico", match!.HomeTeam.Name);
        Assert.Equal("Canada", match.AwayTeam.Name);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task PostMatches_WithDuplicateTeamNames_Returns400WithStartRejectedError()
    {
        var request = new StartMatchRequest("Spain", "Spain", DateTime.UtcNow, "Bernabeu");

        var response = await _client.PostAsJsonAsync("/matches", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("start_rejected", error!.ErrorCode);
        Assert.False(string.IsNullOrEmpty(error.ErrorMessage));
    }
}

public record MatchResponse(int Id, TeamResponse HomeTeam, TeamResponse AwayTeam, DateTime ScheduledAt, string Location, int Status);

public record TeamResponse(string Name, int Score);
