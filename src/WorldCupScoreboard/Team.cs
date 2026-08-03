namespace WorldCupScoreboard;

public class Team
{
    public string Name { get; internal set; }
    public int Score { get; internal set; }

    public Team(string name)
    {
        Name = name;
        Score = 0;
    }

    // Parameterless constructor for EF Core materialization (Persistence/ScoreboardDbContext).
    private Team()
    {
        Name = null!;
    }
}
