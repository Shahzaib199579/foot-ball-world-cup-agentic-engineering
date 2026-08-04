using System;
using System.Linq;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetSummaryOrderingTests
{
    [Fact]
    public void GetSummary_OrdersByTotalScoreDescending()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var low = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");
        var high = scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow, "Venue2");
        scoreboard.UpdateScore(low!.Id, 1, 0);
        scoreboard.UpdateScore(high!.Id, 5, 5);

        var summary = scoreboard.GetSummary().ToList();

        Assert.Equal(high.Id, summary[0].Id);
        Assert.Equal(low.Id, summary[1].Id);
    }

    [Fact]
    public void GetSummary_OnTiedTotalScore_OrdersMostRecentlyStartedFirst()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var first = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Venue1");
        var second = scoreboard.StartMatch("Uruguay", "Italy", DateTime.UtcNow, "Venue2");
        scoreboard.UpdateScore(first!.Id, 2, 2);
        scoreboard.UpdateScore(second!.Id, 3, 1);

        var summary = scoreboard.GetSummary().ToList();

        Assert.Equal(second.Id, summary[0].Id);
        Assert.Equal(first.Id, summary[1].Id);
    }

    [Fact]
    public void GetSummary_WithThreeOrMoreTiedMatches_OrdersAllByMostRecentlyStartedFirst()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var first = scoreboard.StartMatch("A1", "A2", DateTime.UtcNow, "Venue1");
        var second = scoreboard.StartMatch("B1", "B2", DateTime.UtcNow, "Venue2");
        var third = scoreboard.StartMatch("C1", "C2", DateTime.UtcNow, "Venue3");
        scoreboard.UpdateScore(first!.Id, 1, 1);
        scoreboard.UpdateScore(second!.Id, 2, 0);
        scoreboard.UpdateScore(third!.Id, 0, 2);

        var summary = scoreboard.GetSummary().ToList();

        Assert.Equal(new[] { third.Id, second.Id, first.Id }, summary.Select(m => m.Id));
    }
}
