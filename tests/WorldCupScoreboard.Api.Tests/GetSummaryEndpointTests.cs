using System.Net;
using System.Net.Http.Json;
using WorldCupScoreboard.Api.Contracts;

namespace WorldCupScoreboard.Api.Tests;

public class GetSummaryEndpointTests
{
    [Fact]
    public async Task GetSummary_WithNoInProgressMatches_Returns200WithEmptyArray()
    {
        using var factory = new ScoreboardApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/matches/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var matches = await response.Content.ReadFromJsonAsync<MatchResponse[]>();
        Assert.NotNull(matches);
        Assert.Empty(matches!);
    }

    [Fact]
    public async Task GetSummary_WithBriefWorkedExample_ReturnsTotalScoreDescendingThenMostRecentFirst()
    {
        using var factory = new ScoreboardApiFactory();
        var client = factory.CreateClient();

        async Task<int> Start(string home, string away, int homeScore, int awayScore)
        {
            var startResponse = await client.PostAsJsonAsync(
                "/matches", new StartMatchRequest(home, away, DateTime.UtcNow, $"{home}-{away} Stadium"));
            var started = await startResponse.Content.ReadFromJsonAsync<MatchResponse>();
            await client.PutAsJsonAsync($"/matches/{started!.Id}/score", new UpdateScoreRequest(homeScore, awayScore));
            return started.Id;
        }

        await Start("Mexico", "Canada", 0, 5);
        await Start("Spain", "Brazil", 10, 2);
        await Start("Germany", "France", 2, 2);
        await Start("Uruguay", "Italy", 6, 6);
        await Start("Argentina", "Australia", 3, 1);

        var response = await client.GetAsync("/matches/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var matches = await response.Content.ReadFromJsonAsync<MatchResponse[]>();
        Assert.NotNull(matches);
        var order = matches!.Select(m => (m.HomeTeam.Name, m.AwayTeam.Name)).ToArray();
        Assert.Equal(
            new[]
            {
                ("Uruguay", "Italy"),
                ("Spain", "Brazil"),
                ("Mexico", "Canada"),
                ("Argentina", "Australia"),
                ("Germany", "France"),
            },
            order);
    }
}
