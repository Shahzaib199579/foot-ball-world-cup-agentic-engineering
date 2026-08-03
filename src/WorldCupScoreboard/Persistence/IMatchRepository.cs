namespace WorldCupScoreboard.Persistence;

public interface IMatchRepository
{
    void Add(Match match);

    Match? GetById(int matchId);

    IEnumerable<Match> GetAll();

    void Update(Match match);
}
