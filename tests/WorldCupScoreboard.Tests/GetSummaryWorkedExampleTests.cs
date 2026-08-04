using System;
using System.Linq;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

/// <summary>
/// The brief's literal worked example (Sportradar take-home exercise) — treated as an
/// acceptance test per CLAUDE.md's explicit commitment, kept in its own dedicated file so it
/// stays trivially discoverable rather than buried among ordinary ordering tests.
/// </summary>
public class GetSummaryWorkedExampleTests
{
    [Fact]
    public void GetSummary_WithTheBriefsWorkedExample_ProducesTheExactExpectedOrder()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        var mexico = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");
        var spain = scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow, "Venue2");
        var germany = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Venue3");
        var uruguay = scoreboard.StartMatch("Uruguay", "Italy", DateTime.UtcNow, "Venue4");
        var argentina = scoreboard.StartMatch("Argentina", "Australia", DateTime.UtcNow, "Venue5");

        scoreboard.UpdateScore(mexico!.Id, 0, 5);
        scoreboard.UpdateScore(spain!.Id, 10, 2);
        scoreboard.UpdateScore(germany!.Id, 2, 2);
        scoreboard.UpdateScore(uruguay!.Id, 6, 6);
        scoreboard.UpdateScore(argentina!.Id, 3, 1);

        var summary = scoreboard.GetSummary().ToList();

        Assert.Equal(
            new[] { uruguay.Id, spain.Id, mexico.Id, argentina.Id, germany.Id },
            summary.Select(m => m.Id));
    }
}
