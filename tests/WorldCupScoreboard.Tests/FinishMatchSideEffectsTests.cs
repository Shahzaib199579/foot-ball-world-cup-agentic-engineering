using System;
using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class FinishMatchSideEffectsTests
{
    [Fact]
    public void UpdateScore_AfterMatchIsFinished_ThrowsMatchNotFoundExceptionAndLeavesFinalScoreUnchanged()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
        scoreboard.UpdateScore(match!.Id, 2, 1);
        scoreboard.FinishMatch(match.Id);

        Assert.Throws<MatchNotFoundException>(() => scoreboard.UpdateScore(match.Id, 3, 1));

        var unchanged = scoreboard.GetMatch(match.Id);
        Assert.Equal(2, unchanged!.HomeTeam.Score);
        Assert.Equal(1, unchanged.AwayTeam.Score);
    }

    [Fact]
    public void StartMatch_ReusingAFinishedMatchsTeamsAndLocationAndScheduledAt_Succeeds()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());
        var scheduledAt = DateTime.UtcNow;
        var match = scoreboard.StartMatch("Mexico", "Canada", scheduledAt, "Estadio Azteca");
        scoreboard.FinishMatch(match!.Id);

        var reused = scoreboard.StartMatch("Mexico", "Canada", scheduledAt, "Estadio Azteca");

        Assert.NotNull(reused);
        Assert.NotEqual(match.Id, reused!.Id);
    }
}
