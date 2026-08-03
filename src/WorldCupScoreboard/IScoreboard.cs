namespace WorldCupScoreboard;

public interface IScoreboard
{
    Match? StartMatch(string homeTeam, string awayTeam, DateTime scheduledAt, string location);

    Match? GetMatch(int matchId);
}
