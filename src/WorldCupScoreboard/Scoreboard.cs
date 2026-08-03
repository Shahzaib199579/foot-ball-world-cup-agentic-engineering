using WorldCupScoreboard.Exceptions;
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
            _repository.Update(match);
            return match;
        }
    }
}
