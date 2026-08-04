using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Persistence;

namespace WorldCupScoreboard;

public class Scoreboard : IScoreboard
{
    private readonly object _lock = new();
    private readonly IMatchRepository _repository;
    private int _nextId;
    private int _nextActivitySequence;

    public Scoreboard(IMatchRepository repository)
    {
        _repository = repository;
        _nextId = 1;
        _nextActivitySequence = 1;
        foreach (var existing in _repository.GetAll())
        {
            if (existing.Id >= _nextId)
            {
                _nextId = existing.Id + 1;
            }

            if (existing.ActivitySequence >= _nextActivitySequence)
            {
                _nextActivitySequence = existing.ActivitySequence + 1;
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

            var match = new Match(_nextId, new Team(homeTeam), new Team(awayTeam), scheduledAt, location)
            {
                ActivitySequence = _nextActivitySequence
            };
            _repository.Add(match);
            _nextId++;
            _nextActivitySequence++;
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

    public Match UpdateScore(int matchId, int homeScore, int awayScore)
    {
        lock (_lock)
        {
            var match = _repository.GetById(matchId);
            if (match is null || match.Status != MatchStatus.InProgress)
            {
                throw new MatchNotFoundException(matchId);
            }

            if (homeScore < 0 || homeScore < match.HomeTeam.Score)
            {
                throw new InvalidScoreException(match.HomeTeam.Name, homeScore, match.HomeTeam.Score);
            }

            if (awayScore < 0 || awayScore < match.AwayTeam.Score)
            {
                throw new InvalidScoreException(match.AwayTeam.Name, awayScore, match.AwayTeam.Score);
            }

            match.HomeTeam.Score = homeScore;
            match.AwayTeam.Score = awayScore;
            match.ActivitySequence = _nextActivitySequence;
            _nextActivitySequence++;
            _repository.Update(match);
            return match;
        }
    }

    public Match FinishMatch(int matchId)
    {
        lock (_lock)
        {
            var match = _repository.GetById(matchId);
            if (match is null || match.Status != MatchStatus.InProgress)
            {
                throw new MatchNotFoundException(matchId);
            }

            match.Status = MatchStatus.Finished;
            match.ActivitySequence = _nextActivitySequence;
            _nextActivitySequence++;
            _repository.Update(match);
            return match;
        }
    }

    public IEnumerable<Match> GetSummary()
    {
        lock (_lock)
        {
            return _repository.GetAll()
                .Where(m => m.Status == MatchStatus.InProgress)
                .OrderByDescending(m => m.TotalScore)
                .ThenByDescending(m => m.Id)
                .ToList();
        }
    }

    public IEnumerable<Match> GetHistory(int page)
    {
        if (page < 1)
        {
            throw new InvalidPageException(page);
        }

        lock (_lock)
        {
            return _repository.GetAll()
                .OrderByDescending(m => m.ActivitySequence)
                .Skip((page - 1) * 10)
                .Take(10)
                .ToList();
        }
    }
}
