using System;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetMatchTests
{
    [Fact]
    public void GetMatch_WithExistingId_ReturnsRecordedMatchData()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        var started = scoreboard.StartMatch("Mexico", "Canada", scheduledAt, "Estadio Azteca");
        Assert.NotNull(started);

        var fetched = scoreboard.GetMatch(started!.Id);

        Assert.NotNull(fetched);
        Assert.Equal(started.Id, fetched!.Id);
        Assert.Equal("Mexico", fetched.HomeTeam.Name);
        Assert.Equal("Canada", fetched.AwayTeam.Name);
        Assert.Equal(0, fetched.HomeTeam.Score);
        Assert.Equal(0, fetched.AwayTeam.Score);
        Assert.Equal(scheduledAt, fetched.ScheduledAt);
        Assert.Equal("Estadio Azteca", fetched.Location);
    }

    [Fact]
    public void GetMatch_WithUnknownId_ReturnsNull()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var fetched = scoreboard.GetMatch(-1);

        Assert.Null(fetched);
    }
}
