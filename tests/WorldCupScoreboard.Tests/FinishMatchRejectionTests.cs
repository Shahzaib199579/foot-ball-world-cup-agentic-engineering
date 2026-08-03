using System;
using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class FinishMatchRejectionTests
{
    [Fact]
    public void FinishMatch_OnAnAlreadyFinishedMatch_ThrowsMatchNotFoundException()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.FinishMatch(match!.Id);

        Assert.Throws<MatchNotFoundException>(() => scoreboard.FinishMatch(match.Id));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(MatchStatus.Finished, unchanged!.Status);
    }

    [Fact]
    public void FinishMatch_WithNonexistentMatchId_ThrowsMatchNotFoundException()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        Assert.Throws<MatchNotFoundException>(() => scoreboard.FinishMatch(-1));
    }
}
