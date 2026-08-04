using System;
using System.Linq;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetSummaryScopeTests
{
    [Fact]
    public void GetSummary_WithNoInProgressMatches_ReturnsEmpty()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        Assert.Empty(scoreboard.GetSummary());
    }

    [Fact]
    public void GetSummary_ExcludesAFinishedMatch()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");
        scoreboard.FinishMatch(match!.Id);

        Assert.DoesNotContain(scoreboard.GetSummary(), m => m.Id == match.Id);
    }

    [Fact]
    public void GetSummary_DoesNotChangeAnyMatchsData()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        scoreboard.GetSummary().ToList();

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
        Assert.Equal(MatchStatus.InProgress, unchanged.Status);
    }
}
