using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Tests.Fakes;
using Xunit;

namespace WorldCupScoreboard.Tests;

public class UpdateScoreNotFoundTests
{
    [Fact]
    public void UpdateScore_WithNonexistentMatchId_ThrowsMatchNotFoundException()
    {
        var scoreboard = new Scoreboard(new InMemoryMatchRepository());

        Assert.Throws<MatchNotFoundException>(() => scoreboard.UpdateScore(-1, 1, 0));
    }
}
