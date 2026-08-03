namespace WorldCupScoreboard;

public interface IScoreboard
{
    Match? StartMatch(string homeTeam, string awayTeam, DateTime scheduledAt, string location);

    Match? GetMatch(int matchId);

    Match UpdateScore(int matchId, int homeScore, int awayScore);

    Match FinishMatch(int matchId);
}
