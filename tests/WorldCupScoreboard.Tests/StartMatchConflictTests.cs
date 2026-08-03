using System;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class StartMatchConflictTests
{
    [Fact]
    public void StartMatch_WhenTeamAlreadyInAnotherInProgressMatch_ReturnsNull()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");

        var conflicting = scoreboard.StartMatch("Mexico", "Spain", DateTime.UtcNow, "Different Venue");

        Assert.Null(conflicting);
    }

    [Fact]
    public void StartMatch_WhenSameLocationAndSameScheduledAtAsAnInProgressMatch_ReturnsNull()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        scoreboard.StartMatch("Germany", "France", scheduledAt, "Estadio Azteca");

        var conflicting = scoreboard.StartMatch("Uruguay", "Italy", scheduledAt, "Estadio Azteca");

        Assert.Null(conflicting);
    }

    [Fact]
    public void StartMatch_WithSameLocationButDifferentScheduledAt_Succeeds()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        scoreboard.StartMatch("Germany", "France", scheduledAt, "Estadio Azteca");

        var match = scoreboard.StartMatch("Uruguay", "Italy", scheduledAt.AddHours(2), "Estadio Azteca");

        Assert.NotNull(match);
    }

    [Fact]
    public void StartMatch_WithSameScheduledAtButDifferentLocation_Succeeds()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        scoreboard.StartMatch("Germany", "France", scheduledAt, "Estadio Azteca");

        var match = scoreboard.StartMatch("Uruguay", "Italy", scheduledAt, "Camp Nou");

        Assert.NotNull(match);
    }

    [Fact]
    public void StartMatch_AfterARejectedAttempt_NextSuccessfulMatchGetsTheNextSequentialId()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var first = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        Assert.NotNull(first);

        var rejected = scoreboard.StartMatch("Mexico", "Spain", DateTime.UtcNow, "Different Venue");
        Assert.Null(rejected);

        var second = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Allianz Arena");

        Assert.NotNull(second);
        Assert.Equal(first!.Id + 1, second!.Id);
    }
}
