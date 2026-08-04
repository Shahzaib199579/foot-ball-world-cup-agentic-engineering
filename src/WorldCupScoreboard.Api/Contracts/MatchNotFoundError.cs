namespace WorldCupScoreboard.Api.Contracts;

public class MatchNotFoundError : IApiError
{
    public int MatchId { get; }

    public MatchNotFoundError(int matchId)
    {
        MatchId = matchId;
    }

    public string ErrorCode => "match_not_found";

    public string ErrorMessage => $"No in-progress match was found with Id {MatchId}.";
}
