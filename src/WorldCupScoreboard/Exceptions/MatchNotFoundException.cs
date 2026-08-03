namespace WorldCupScoreboard.Exceptions;

public class MatchNotFoundException : Exception
{
    public int MatchId { get; }

    public MatchNotFoundException(int matchId)
        : base($"No in-progress match was found with Id {matchId}.")
    {
        MatchId = matchId;
    }
}
