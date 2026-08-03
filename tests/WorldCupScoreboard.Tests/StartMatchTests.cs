using System;
using WorldCupScoreboard.Tests.Fakes;
using System.Collections.Generic;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class StartMatchTests
{
    [Fact]
    public void StartMatch_WithValidTeamsAndDetails_CreatesInProgressMatchWithZeroScore()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = new DateTime(2026, 8, 3, 15, 0, 0, DateTimeKind.Utc);

        var match = scoreboard.StartMatch("Mexico", "Canada", scheduledAt, "Estadio Azteca");

        Assert.NotNull(match);
        Assert.Equal("Mexico", match!.HomeTeam.Name);
        Assert.Equal("Canada", match.AwayTeam.Name);
        Assert.Equal(0, match.HomeTeam.Score);
        Assert.Equal(0, match.AwayTeam.Score);
        Assert.Equal(MatchStatus.InProgress, match.Status);
        Assert.Equal(scheduledAt, match.ScheduledAt);
        Assert.Equal("Estadio Azteca", match.Location);
    }

    [Fact]
    public void StartMatch_AssignsAUniqueIdPerMatch()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var match1 = scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow, "Camp Nou");
        var match2 = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Allianz Arena");

        Assert.NotNull(match1);
        Assert.NotNull(match2);
        Assert.NotEqual(match1!.Id, match2!.Id);
    }

    [Theory]
    [MemberData(nameof(ScheduledAtVariants))]
    public void StartMatch_AcceptsAnyScheduledAtAndActivatesImmediately(DateTime scheduledAt)
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var match = scoreboard.StartMatch("Uruguay", "Italy", scheduledAt, "Centenario");

        Assert.NotNull(match);
        Assert.Equal(MatchStatus.InProgress, match!.Status);
        Assert.Equal(scheduledAt, match.ScheduledAt);
    }

    public static IEnumerable<object[]> ScheduledAtVariants()
    {
        yield return new object[] { DateTime.UtcNow.AddDays(-1) };
        yield return new object[] { DateTime.UtcNow };
        yield return new object[] { DateTime.UtcNow.AddDays(30) };
    }
}
