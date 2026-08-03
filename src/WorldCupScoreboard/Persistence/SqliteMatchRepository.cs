namespace WorldCupScoreboard.Persistence;

public class SqliteMatchRepository : IMatchRepository
{
    private readonly ScoreboardDbContext _context;

    public SqliteMatchRepository(ScoreboardDbContext context)
    {
        _context = context;
    }

    public void Add(Match match)
    {
        _context.Matches.Add(match);
        _context.SaveChanges();
    }

    public Match? GetById(int matchId)
    {
        return _context.Matches.FirstOrDefault(m => m.Id == matchId);
    }

    public IEnumerable<Match> GetAll()
    {
        return _context.Matches.ToList();
    }

    public void Update(Match match)
    {
        _context.Matches.Update(match);
        _context.SaveChanges();
    }
}
