using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WorldCupScoreboard.Persistence;

public class ScoreboardDbContextFactory : IDesignTimeDbContextFactory<ScoreboardDbContext>
{
    public const string DefaultConnectionString = "Data Source=scoreboard.db";

    public ScoreboardDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ScoreboardDbContext>();
        optionsBuilder.UseSqlite(DefaultConnectionString);
        return new ScoreboardDbContext(optionsBuilder.Options);
    }
}
