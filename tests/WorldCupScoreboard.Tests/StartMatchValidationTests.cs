using System;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class StartMatchValidationTests
{
    [Theory]
    [InlineData(null, "Canada")]
    [InlineData("", "Canada")]
    [InlineData("Mexico", null)]
    [InlineData("Mexico", "")]
    public void StartMatch_WithMissingTeamName_ReturnsNull(string? homeTeam, string? awayTeam)
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var match = scoreboard.StartMatch(homeTeam!, awayTeam!, DateTime.UtcNow, "Estadio Azteca");

        Assert.Null(match);
    }

    [Fact]
    public void StartMatch_WithIdenticalTeamNames_ReturnsNull()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var match = scoreboard.StartMatch("Mexico", "Mexico", DateTime.UtcNow, "Estadio Azteca");

        Assert.Null(match);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void StartMatch_WithMissingLocation_ReturnsNull(string? location)
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, location!);

        Assert.Null(match);
    }
}
