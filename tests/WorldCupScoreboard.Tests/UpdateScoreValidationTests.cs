using System;
using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class UpdateScoreValidationTests
{
    // Acceptance Scenario 5 (letters/special characters, e.g. "two", "2-1", "2.5", "2!") has no
    // runnable test here: UpdateScore's homeScore/awayScore parameters are typed `int`, so
    // passing a non-numeric value is a compile-time error, not a runtime case this library's
    // own tests can exercise (spec.md Assumptions; quickstart.md). A future 006-scoreboard-api,
    // which parses raw HTTP/JSON input, is the layer that will need this exact runtime test.

    [Fact]
    public void UpdateScore_WithNegativeHomeScore_ThrowsInvalidScoreException()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, -1, 1));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_WithNegativeAwayScore_ThrowsInvalidScoreException()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, 2, -1));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_WithLowerHomeScoreThanCurrent_ThrowsInvalidScoreExceptionAndLeavesScoreUnchanged()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, 1, 1));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_WithLowerAwayScoreThanCurrent_ThrowsInvalidScoreExceptionAndLeavesScoreUnchanged()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, 2, 0));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_WhenOneScoreIsInvalid_RejectsTheWholeUpdateEvenIfTheOtherIsValid()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        // Home score (3) is a valid increase, but away score (0) is a decrease — the whole
        // update must be rejected, including the otherwise-valid home score change.
        Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, 3, 0));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
    }

    [Fact]
    public void UpdateScore_WithScoresEqualToCurrent_Succeeds()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);

        var updated = scoreboard.UpdateScore(match.Id, 2, 1);

        Assert.Equal(2, updated.HomeTeam.Score);
        Assert.Equal(1, updated.AwayTeam.Score);
    }
}
