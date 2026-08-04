namespace WorldCupScoreboard.Api.Contracts;

public class StartRejectedError : IApiError
{
    public string ErrorCode => "start_rejected";

    public string ErrorMessage =>
        "Start did not succeed. Possible reasons: empty/duplicate team names, empty location, " +
        "a team already in another in-progress match, or another in-progress match already at " +
        "the same location and scheduled time.";
}
