using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WorldCupScoreboard;
using WorldCupScoreboard.Exceptions;
using WorldCupScoreboard.Persistence;

var dbContext = new ScoreboardDbContext(
    new DbContextOptionsBuilder<ScoreboardDbContext>()
        .UseSqlite(ScoreboardDbContextFactory.DefaultConnectionString)
        .Options);
dbContext.Database.Migrate();

var scoreboard = new Scoreboard(new SqliteMatchRepository(dbContext));
var startedIds = new List<int>();

PrintWelcome();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line is null)
    {
        break;
    }

    var tokens = Tokenize(line);
    if (tokens.Count == 0)
    {
        continue;
    }

    var command = tokens[0].ToLowerInvariant();
    var commandArgs = tokens.Skip(1).ToList();

    switch (command)
    {
        case "start":
            HandleStart(commandArgs);
            break;
        case "get":
            HandleGet(commandArgs);
            break;
        case "update":
            HandleUpdate(commandArgs);
            break;
        case "finish":
            HandleFinish(commandArgs);
            break;
        case "ids":
            HandleIds();
            break;
        case "help":
            PrintHelp();
            break;
        case "exit":
        case "quit":
            dbContext.Dispose();
            return;
        default:
            Console.WriteLine($"Unknown command: {command}. Type 'help' for a list of commands.");
            break;
    }
}

dbContext.Dispose();

void HandleStart(List<string> args)
{
    if (args.Count < 3)
    {
        Console.WriteLine("Usage: start <homeTeam> <awayTeam> <location> [scheduledAt]");
        Console.WriteLine("  scheduledAt: ISO-8601 (e.g. 2026-08-03T15:00:00Z) or 'now' — defaults to 'now'.");
        return;
    }

    var homeTeam = args[0];
    var awayTeam = args[1];
    var location = args[2];
    var scheduledAt = DateTime.UtcNow;

    if (args.Count >= 4 && !string.Equals(args[3], "now", StringComparison.OrdinalIgnoreCase))
    {
        if (!DateTime.TryParse(args[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out scheduledAt))
        {
            Console.WriteLine($"Could not parse scheduledAt '{args[3]}'. Use ISO-8601 (e.g. 2026-08-03T15:00:00Z) or 'now'.");
            return;
        }
    }

    var match = scoreboard.StartMatch(homeTeam, awayTeam, scheduledAt, location);

    if (match is null)
    {
        Console.WriteLine("REJECTED — start did not succeed. Possible reasons: empty/duplicate team names, " +
            "empty location, a team already in another in-progress match, or another in-progress match " +
            "already at the same location and scheduledAt. No match was created.");
        return;
    }

    startedIds.Add(match.Id);
    PrintMatch(match, "STARTED");
}

void HandleGet(List<string> args)
{
    if (args.Count < 1 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: get <matchId>");
        return;
    }

    var match = scoreboard.GetMatch(id);

    if (match is null)
    {
        Console.WriteLine($"No match found with Id {id}.");
        return;
    }

    PrintMatch(match, "FOUND");
}

void HandleUpdate(List<string> args)
{
    if (args.Count < 3 || !int.TryParse(args[0], out var matchId)
        || !int.TryParse(args[1], out var homeScore) || !int.TryParse(args[2], out var awayScore))
    {
        Console.WriteLine("Usage: update <matchId> <homeScore> <awayScore>");
        return;
    }

    try
    {
        var updated = scoreboard.UpdateScore(matchId, homeScore, awayScore);
        PrintMatch(updated, "UPDATED");
    }
    catch (MatchNotFoundException ex)
    {
        Console.WriteLine($"REJECTED — {ex.Message}");
    }
    catch (InvalidScoreException ex)
    {
        Console.WriteLine($"REJECTED — {ex.Message}");
    }
}

void HandleFinish(List<string> args)
{
    if (args.Count < 1 || !int.TryParse(args[0], out var matchId))
    {
        Console.WriteLine("Usage: finish <matchId>");
        return;
    }

    try
    {
        var finished = scoreboard.FinishMatch(matchId);
        PrintMatch(finished, "FINISHED");
    }
    catch (MatchNotFoundException ex)
    {
        Console.WriteLine($"REJECTED — {ex.Message}");
    }
}

void HandleIds()
{
    if (startedIds.Count == 0)
    {
        Console.WriteLine("No matches started yet this session.");
        return;
    }

    Console.WriteLine("Match Ids started this session: " + string.Join(", ", startedIds));
}

void PrintMatch(Match match, string label)
{
    Console.WriteLine(
        $"[{label}] Id={match.Id} | {match.HomeTeam.Name} {match.HomeTeam.Score}-{match.AwayTeam.Score} " +
        $"{match.AwayTeam.Name} | Status={match.Status} | ScheduledAt={match.ScheduledAt:O} | " +
        $"Location={match.Location}");
}

List<string> Tokenize(string line)
{
    var result = new List<string>();
    var current = new System.Text.StringBuilder();
    var inQuotes = false;
    var hasCurrent = false;

    foreach (var c in line)
    {
        if (c == '"')
        {
            inQuotes = !inQuotes;
            hasCurrent = true;
            continue;
        }

        if (char.IsWhiteSpace(c) && !inQuotes)
        {
            if (hasCurrent)
            {
                result.Add(current.ToString());
                current.Clear();
                hasCurrent = false;
            }
            continue;
        }

        current.Append(c);
        hasCurrent = true;
    }

    if (hasCurrent)
    {
        result.Add(current.ToString());
    }

    return result;
}

void PrintWelcome()
{
    Console.WriteLine("WorldCupScoreboard demo CLI");
    Console.WriteLine("Covers: 001-start-match (start, get), 002-update-score (update), 003-finish-match (finish). Later specs add more commands here.");
    Console.WriteLine("Type 'help' for commands and manual test scenarios, 'exit' to quit.");
    Console.WriteLine();
}

void PrintHelp()
{
    Console.WriteLine(
        """
        Commands:
          start <home> <away> <location> [scheduledAt]   Start a new match. scheduledAt: ISO-8601 or 'now' (default).
          get <matchId>                                   Retrieve a match by its Id.
          update <matchId> <homeScore> <awayScore>        Update a match's score (must not decrease).
          finish <matchId>                                 Mark a match as finished (one-way, terminal).
          ids                                              List match Ids started this session.
          help                                             Show this help.
          exit | quit                                      Quit.

        Quote multi-word arguments, e.g.: start Mexico Canada "Estadio Azteca"
        Use "" for an empty argument, e.g.: start "" Canada "Estadio Azteca"

        Manual test scenarios (spec 001-start-match):

        1. Successful start (FR-001, FR-002, FR-003):
             start Mexico Canada "Estadio Azteca"
           -> STARTED, score 0-0, Status=InProgress.

        2. Missing or duplicate team name is rejected (FR-004):
             start "" Canada "Estadio Azteca"
             start Mexico Mexico "Estadio Azteca"
           -> REJECTED both times.

        3. Missing location is rejected (FR-002):
             start Spain Brazil ""
           -> REJECTED.

        4. A team already in another in-progress match is rejected (FR-005):
             start Mexico Canada "Estadio Azteca"
             start Mexico Spain "Camp Nou"
           -> second REJECTED (Mexico already in-progress).

        5. Same location AND same scheduledAt is rejected (FR-006) — use an explicit
           timestamp so both calls match exactly:
             start Germany France "Estadio Azteca" 2026-08-03T15:00:00Z
             start Uruguay Italy "Estadio Azteca" 2026-08-03T15:00:00Z
           -> second REJECTED.

        6. Same location with a different time, or same time at a different location,
           both succeed (FR-006):
             start Germany France "Estadio Azteca" 2026-08-03T15:00:00Z
             start Uruguay Italy "Estadio Azteca" 2026-08-03T17:00:00Z
             start Argentina Australia "Camp Nou" 2026-08-03T15:00:00Z
           -> all three STARTED.

        7. Past/present/future scheduledAt are all accepted and active immediately (FR-003):
             start TeamA TeamB Venue1 2020-01-01T00:00:00Z
             start TeamC TeamD Venue2 now
             start TeamE TeamF Venue3 2030-01-01T00:00:00Z
           -> all three STARTED with Status=InProgress regardless of date.

        8. Retrieve a started match by Id (FR-007):
             ids
             get <one of the Ids printed above>
           -> FOUND, same details as when it was started.

        9. Retrieve an unknown Id (FR-007):
             get 9999
           -> "No match found".

        10. A rejected start has no side effect (FR-008) — Ids stay sequential across a
            rejection:
             start Netherlands Belgium "Johan Cruyff Arena"
             start Netherlands Portugal "Different Venue"
             start Denmark Sweden Parken
             ids
           -> Denmark/Sweden's Id is exactly one more than Netherlands/Belgium's — the
              rejected Netherlands/Portugal attempt never consumed an Id.

        Note: the CLI can only ever pass an empty string, never a true null, for a missing
        argument — the null-argument branch of FR-002/FR-004 is covered by the automated
        tests (StartMatchValidationTests.cs), not manually here.

        Manual test scenarios (spec 002-update-score):

        11. A score update upward succeeds (FR-001, FR-006, FR-007):
              start Mexico Canada "Estadio Azteca"
              ids
              update <matchId> 2 1
            -> UPDATED, score 2-1; get <matchId> confirms it.

        12. A decrease is rejected, score left unchanged (FR-003, FR-004):
              update <matchId> 1 1
            -> REJECTED (home score 1 is lower than current 2); get <matchId> still shows 2-1.

        13. A negative score is rejected (FR-002):
              update <matchId> 2 -1
            -> REJECTED; score unchanged.

        14. An update against a nonexistent match Id is rejected (FR-005):
              update 9999 1 0
            -> REJECTED — no such in-progress match.

        Note: a malformed value (letters/special characters) can't be passed here either —
        update's arguments are parsed as integers by this CLI itself (see HandleUpdate), so a
        non-numeric argument fails Console-side parsing before ever reaching the library, same
        rationale as spec 002-update-score's Assumptions section.

        Manual test scenarios (spec 003-finish-match):

        15. Finishing an in-progress match succeeds and its data survives (FR-001, FR-002,
            FR-007):
              start Mexico Canada "Estadio Azteca"
              ids
              update <matchId> 2 1
              finish <matchId>
              get <matchId>
            -> FINISHED, then get still shows Status=Finished, score 2-1 unchanged.

        16. Finishing an already-finished match is rejected (FR-004):
              finish <matchId>
            -> REJECTED — no such in-progress match.

        17. Finishing a nonexistent match Id is rejected (FR-004):
              finish 9999
            -> REJECTED.

        18. A score update after finishing is rejected, final score never changes (FR-005):
              update <matchId> 5 5
            -> REJECTED; get <matchId> still shows 2-1.

        19. A finished match's location and scheduledAt become reusable (FR-006):
              start Germany France "Camp Nou" 2026-08-04T15:00:00Z
              finish <thatMatchId>
              start Uruguay Italy "Camp Nou" 2026-08-04T15:00:00Z
            -> the second start SUCCEEDS — while Germany/France was in-progress this would
               have been REJECTED (see scenario 5 above); finishing frees the slot.
        """);
}
