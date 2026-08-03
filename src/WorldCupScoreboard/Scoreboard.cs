using WorldCupScoreboard.Persistence;

namespace WorldCupScoreboard;

public class Scoreboard : IScoreboard
{
    private readonly object _lock = new();
    private readonly IMatchRepository _repository;
    private int _nextId;

    public Scoreboard(IMatchRepository repository)
    {
        _repository = repository;
        _nextId = 1;
        foreach (var existing in _repository.GetAll())
        {
            if (existing.Id >= _nextId)
            {
                _nextId = existing.Id + 1;
            }
        }
    }

    public Match? StartMatch(string homeTeam, string awayTeam, DateTime scheduledAt, string location)
    {
        lock (_lock)
        {
            if (string.IsNullOrEmpty(homeTeam) || string.IsNullOrEmpty(awayTeam) || homeTeam == awayTeam)
            {
                return null;
            }

            if (string.IsNullOrEmpty(location))
            {
                return null;
            }

            foreach (var existing in _repository.GetAll())
            {
                if (existing.Status != MatchStatus.InProgress)
                {
                    continue;
                }

                var involvesSameTeam = existing.HomeTeam.Name == homeTeam
                    || existing.AwayTeam.Name == homeTeam
                    || existing.HomeTeam.Name == awayTeam
                    || existing.AwayTeam.Name == awayTeam;

                if (involvesSameTeam)
                {
                    return null;
                }

                var sameLocationAndTime = existing.Location == location && existing.ScheduledAt == scheduledAt;

                if (sameLocationAndTime)
                {
                    return null;
                }
            }

            var match = new Match(_nextId, new Team(homeTeam), new Team(awayTeam), scheduledAt, location);
            _repository.Add(match);
            _nextId++;
            return match;
        }
    }

    public Match? GetMatch(int matchId)
    {
        lock (_lock)
        {
            return _repository.GetById(matchId);
        }
    }
}
