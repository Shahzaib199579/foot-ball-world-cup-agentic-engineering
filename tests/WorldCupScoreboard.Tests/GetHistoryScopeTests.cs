using System;
using System.Linq;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetHistoryScopeTests
{
    [Fact]
    public void GetHistory_IncludesAFinishedMatch()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        scoreboard.FinishMatch(match!.Id);

        Assert.Contains(scoreboard.GetHistory(1), m => m.Id == match.Id);
    }

    [Fact]
    public void GetHistory_IncludesBothInProgressAndFinishedMatches()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var inProgress = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        var finished = scoreboard.StartMatch("C", "D", DateTime.UtcNow, "Venue2");
        scoreboard.FinishMatch(finished!.Id);

        var history = scoreboard.GetHistory(1).Select(m => m.Id).ToList();
        Assert.Contains(inProgress!.Id, history);
        Assert.Contains(finished.Id, history);
    }

    [Fact]
    public void GetHistory_DoesNotChangeAnyMatchsData()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        scoreboard.GetHistory(1).ToList();

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
        Assert.Equal(MatchStatus.InProgress, unchanged.Status);
    }
}
