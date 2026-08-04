using System;
using System.Linq;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetHistoryOrderingTests
{
    [Fact]
    public void GetHistory_OrdersByMostRecentlyCreatedFirst()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var first = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        var second = scoreboard.StartMatch("C", "D", DateTime.UtcNow, "Venue2");

        var history = scoreboard.GetHistory(1).ToList();

        Assert.Equal(second!.Id, history[0].Id);
        Assert.Equal(first!.Id, history[1].Id);
    }

    [Fact]
    public void GetHistory_AScoreUpdateReRanksAnOlderMatchAheadOfNewerOnes()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var oldest = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        scoreboard.StartMatch("C", "D", DateTime.UtcNow, "Venue2");

        scoreboard.UpdateScore(oldest!.Id, 1, 0);

        var history = scoreboard.GetHistory(1).ToList();
        Assert.Equal(oldest.Id, history[0].Id);
    }

    [Fact]
    public void GetHistory_FinishingAMatchReRanksItAheadOfNewerOnes()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var oldest = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        scoreboard.StartMatch("C", "D", DateTime.UtcNow, "Venue2");

        scoreboard.FinishMatch(oldest!.Id);

        var history = scoreboard.GetHistory(1).ToList();
        Assert.Equal(oldest.Id, history[0].Id);
    }

    [Fact]
    public void GetHistory_ThreeOrMoreMatches_OrdersAllByMostRecentActivity()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var a = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
        var b = scoreboard.StartMatch("C", "D", DateTime.UtcNow, "Venue2");
        var c = scoreboard.StartMatch("E", "F", DateTime.UtcNow, "Venue3");

        // Touch `a` last, then `b` — expected order: b, a, c.
        scoreboard.UpdateScore(a!.Id, 1, 0);
        scoreboard.UpdateScore(b!.Id, 1, 0);

        var history = scoreboard.GetHistory(1).ToList();
        Assert.Equal(new[] { b.Id, a.Id, c!.Id }, history.Select(m => m.Id));
    }
}
