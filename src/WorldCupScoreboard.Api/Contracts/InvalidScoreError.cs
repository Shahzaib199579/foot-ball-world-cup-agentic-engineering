namespace WorldCupScoreboard.Api.Contracts;

public class InvalidScoreError : IApiError
{
    public string TeamName { get; }

    public int AttemptedScore { get; }

    public int CurrentScore { get; }

    public InvalidScoreError(string teamName, int attemptedScore, int currentScore)
    {
        TeamName = teamName;
        AttemptedScore = attemptedScore;
        CurrentScore = currentScore;
    }

    public string ErrorCode => "invalid_score";

    public string ErrorMessage => AttemptedScore < 0
        ? $"Score update rejected for {TeamName}: {AttemptedScore} is negative."
        : $"Score update rejected for {TeamName}: {AttemptedScore} is lower than the current " +
          $"recorded score of {CurrentScore}.";
}
