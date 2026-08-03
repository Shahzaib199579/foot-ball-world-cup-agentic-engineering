namespace WorldCupScoreboard.Exceptions;

public class InvalidScoreException : Exception
{
    public string TeamName { get; }

    public int AttemptedScore { get; }

    public int CurrentScore { get; }

    public InvalidScoreException(string teamName, int attemptedScore, int currentScore)
        : base(BuildMessage(teamName, attemptedScore, currentScore))
    {
        TeamName = teamName;
        AttemptedScore = attemptedScore;
        CurrentScore = currentScore;
    }

    private static string BuildMessage(string teamName, int attemptedScore, int currentScore)
    {
        if (attemptedScore < 0)
        {
            return $"Score update rejected for {teamName}: {attemptedScore} is negative.";
        }

        return $"Score update rejected for {teamName}: {attemptedScore} is lower than the " +
            $"current recorded score of {currentScore}.";
    }
}
