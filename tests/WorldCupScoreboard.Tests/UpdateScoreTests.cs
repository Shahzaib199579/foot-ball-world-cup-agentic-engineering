using System;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class UpdateScoreTests
{
    [Fact]
    public void UpdateScore_WithHigherScores_UpdatesBothTeams()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");

        var updated = scoreboard.UpdateScore(match!.Id, 2, 1);

        Assert.Equal(2, updated.HomeTeam.Score);
        Assert.Equal(1, updated.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_WhenOneTeamsScoreStaysTheSame_OnlyTheOtherChanges()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        var updated = scoreboard.UpdateScore(match.Id, 3, 1);

        Assert.Equal(3, updated.HomeTeam.Score);
        Assert.Equal(1, updated.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_DoesNotChangeAnyOtherRecordedAttribute()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        var match = scoreboard.StartMatch("Mexico", "Canada", scheduledAt, "Estadio Azteca");

        var updated = scoreboard.UpdateScore(match!.Id, 2, 1);

        Assert.Equal(match.Id, updated.Id);
        Assert.Equal("Mexico", updated.HomeTeam.Name);
        Assert.Equal("Canada", updated.AwayTeam.Name);
        Assert.Equal(scheduledAt, updated.ScheduledAt);
        Assert.Equal("Estadio Azteca", updated.Location);
        Assert.Equal(MatchStatus.InProgress, updated.Status);
    }

    [Fact]
    public void UpdateScore_IsImmediatelyVisibleViaGetMatch()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        var fetched = scoreboard.GetMatch(match.Id);

        Assert.NotNull(fetched);
        Assert.Equal(2, fetched!.HomeTeam.Score);
        Assert.Equal(1, fetched.AwayTeam.Score);
    }
}
