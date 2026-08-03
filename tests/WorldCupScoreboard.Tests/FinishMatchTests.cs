using System;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class FinishMatchTests
{
    [Fact]
    public void FinishMatch_OnInProgressMatch_SetsStatusToFinished()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        var finished = scoreboard.FinishMatch(match.Id);

        Assert.Equal(MatchStatus.Finished, finished.Status);
    }

    [Fact]
    public void FinishMatch_DoesNotChangeTheFinalScoreOrAnyOtherAttribute()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        var match = scoreboard.StartMatch("Mexico", "Canada", scheduledAt, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        var finished = scoreboard.FinishMatch(match.Id);

        Assert.Equal(match.Id, finished.Id);
        Assert.Equal("Mexico", finished.HomeTeam.Name);
        Assert.Equal(2, finished.HomeTeam.Score);
        Assert.Equal("Canada", finished.AwayTeam.Name);
        Assert.Equal(1, finished.AwayTeam.Score);
        Assert.Equal(scheduledAt, finished.ScheduledAt);
        Assert.Equal("Estadio Azteca", finished.Location);
    }

    [Fact]
    public void FinishMatch_IsImmediatelyVisibleViaGetMatch()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);
        scoreboard.FinishMatch(match.Id);

        var fetched = scoreboard.GetMatch(match.Id);

        Assert.NotNull(fetched);
        Assert.Equal(MatchStatus.Finished, fetched!.Status);
        Assert.Equal(2, fetched.HomeTeam.Score);
        Assert.Equal(1, fetched.AwayTeam.Score);
    }
}
