namespace WorldCupScoreboard;

public class Match
{
    public int Id { get; internal set; }
    public Team HomeTeam { get; internal set; }
    public Team AwayTeam { get; internal set; }
    public DateTime ScheduledAt { get; internal set; }
    public string Location { get; internal set; }
    public MatchStatus Status { get; internal set; }

    public int TotalScore => HomeTeam.Score + AwayTeam.Score;

    public Match(int id, Team homeTeam, Team awayTeam, DateTime scheduledAt, string location)
    {
        Id = id;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;
        ScheduledAt = scheduledAt;
        Location = location;
        Status = MatchStatus.InProgress;
    }

    // Parameterless constructor for EF Core materialization (Persistence/ScoreboardDbContext) —
    // the public constructor's owned-navigation parameters (homeTeam, awayTeam) can't be bound by
    // EF's constructor injection, so EF uses this one and sets every property via its internal setter.
    private Match()
    {
        HomeTeam = null!;
        AwayTeam = null!;
        Location = null!;
    }
}
