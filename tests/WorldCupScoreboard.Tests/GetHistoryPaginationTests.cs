using System;
using System.Linq;
using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class GetHistoryPaginationTests
{
    [Fact]
    public void GetHistory_Page1_ReturnsAtMostTenMatches()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        for (var i = 0; i < 15; i++)
        {
            scoreboard.StartMatch($"Home{i}", $"Away{i}", DateTime.UtcNow, $"Venue{i}");
        }

        var page1 = scoreboard.GetHistory(1).ToList();

        Assert.Equal(10, page1.Count);
    }

    [Fact]
    public void GetHistory_Page2_ReturnsRemainingMatches()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        for (var i = 0; i < 15; i++)
        {
            scoreboard.StartMatch($"Home{i}", $"Away{i}", DateTime.UtcNow, $"Venue{i}");
        }

        var page2 = scoreboard.GetHistory(2).ToList();

        Assert.Equal(5, page2.Count);
    }

    [Fact]
    public void GetHistory_WithFewerMatchesThanOnePage_ReturnsAllOnPage1AndEmptyOnPage2()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");

        Assert.Single(scoreboard.GetHistory(1));
        Assert.Empty(scoreboard.GetHistory(2));
    }

    [Fact]
    public void GetHistory_WithNoMatchesAtAll_ReturnsEmpty()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        Assert.Empty(scoreboard.GetHistory(1));
    }

    [Fact]
    public void GetHistory_WithPageOutOfRange_ReturnsEmptyNotAnError()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");

        Assert.Empty(scoreboard.GetHistory(100));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetHistory_WithPageLessThanOne_ThrowsInvalidPageException(int page)
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        Assert.Throws<InvalidPageException>(() => scoreboard.GetHistory(page));
    }
}
