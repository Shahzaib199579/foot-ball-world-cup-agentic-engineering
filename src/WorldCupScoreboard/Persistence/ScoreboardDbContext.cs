using Microsoft.EntityFrameworkCore;

namespace WorldCupScoreboard.Persistence;

public class ScoreboardDbContext : DbContext
{
    public DbSet<Match> Matches => Set<Match>();

    public ScoreboardDbContext(DbContextOptions<ScoreboardDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>(match =>
        {
            match.HasKey(m => m.Id);
            match.Property(m => m.Id).ValueGeneratedNever();
            match.Property(m => m.ActivitySequence).ValueGeneratedNever();

            match.OwnsOne(m => m.HomeTeam, home =>
            {
                home.Property(t => t.Name).HasColumnName("HomeTeamName").IsRequired();
                home.Property(t => t.Score).HasColumnName("HomeTeamScore");
            });

            match.OwnsOne(m => m.AwayTeam, away =>
            {
                away.Property(t => t.Name).HasColumnName("AwayTeamName").IsRequired();
                away.Property(t => t.Score).HasColumnName("AwayTeamScore");
            });

            match.Navigation(m => m.HomeTeam).IsRequired();
            match.Navigation(m => m.AwayTeam).IsRequired();
        });
    }
}
