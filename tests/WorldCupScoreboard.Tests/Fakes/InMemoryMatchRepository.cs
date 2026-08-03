using WorldCupScoreboard.Persistence;

namespace WorldCupScoreboard.Tests.Fakes;

public class InMemoryMatchRepository : IMatchRepository
{
    private readonly Dictionary<int, Match> _matches = new();

    public void Add(Match match)
    {
        _matches[match.Id] = match;
    }

    public Match? GetById(int matchId)
    {
        return _matches.TryGetValue(matchId, out var match) ? match : null;
    }

    public IEnumerable<Match> GetAll()
    {
        return _matches.Values;
    }

    public void Update(Match match)
    {
        _matches[match.Id] = match;
    }
}
