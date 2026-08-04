using System;
using System.Linq;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetSummaryLiveUpdateTests
{
    [Fact]
    public void GetSummary_ReflectsAScoreUpdateThatChangesRanking()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var a = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");
        var b = scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow, "Venue2");
        scoreboard.UpdateScore(a!.Id, 1, 0);
        scoreboard.UpdateScore(b!.Id, 5, 5);

        Assert.Equal(b.Id, scoreboard.GetSummary().First().Id);

        scoreboard.UpdateScore(a.Id, 20, 0);

        Assert.Equal(a.Id, scoreboard.GetSummary().First().Id);
    }

    [Fact]
    public void GetSummary_UpdateThatDoesNotChangeTotal_DoesNotDisturbTieOrder()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var first = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Venue1");
        var second = scoreboard.StartMatch("Uruguay", "Italy", DateTime.UtcNow, "Venue2");
        scoreboard.UpdateScore(first!.Id, 2, 2);
        scoreboard.UpdateScore(second!.Id, 3, 1);

        // Same total (4) for `first` — UpdateScore enforces per-team non-decrease, so the only
        // way to resubmit an unchanged total is to repeat the exact same scores (a no-op).
        scoreboard.UpdateScore(first.Id, 2, 2);

        var summary = scoreboard.GetSummary().ToList();
        Assert.Equal(second.Id, summary[0].Id);
        Assert.Equal(first.Id, summary[1].Id);
    }

    [Fact]
    public void GetSummary_IncludesAFreshlyStartedMatchImmediatelyAtZeroTotal()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");

        var summary = scoreboard.GetSummary().ToList();

        Assert.Contains(summary, m => m.Id == match!.Id && m.TotalScore == 0);
    }
}
