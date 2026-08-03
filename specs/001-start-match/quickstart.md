# Quickstart: Start New Match

Validation guide for `specs/001-start-match`. `WorldCupScoreboard.sln`, `src/WorldCupScoreboard/`,
and `tests/WorldCupScoreboard.Tests/` exist and are implemented — `Scoreboard` now depends on
`IMatchRepository` (constitution Principle IV, amended) rather than an in-process `Dictionary`;
see plan.md's "Amendment (post-implementation, pre-commit)" note for why.

## Prerequisites

- .NET 9 SDK installed (`dotnet --version` → `9.x`).

## Build & test

```bash
dotnet build
dotnet test
```

All tests in `tests/WorldCupScoreboard.Tests/StartMatchTests.cs` must pass — one test per
Functional Requirement (FR-001..FR-008) plus the acceptance scenarios in `spec.md`'s User Stories
1 and 2. Unit tests construct `Scoreboard` against the fake
`tests/WorldCupScoreboard.Tests/Fakes/InMemoryMatchRepository.cs`, never a real database.

## Manual validation walkthrough

Exercises the contract in `contracts/IScoreboard.md` directly, mirroring the spec's acceptance
scenarios. The snippets below wire `Scoreboard` to a real SQLite-backed `IMatchRepository`, the
same way `demo/ScoreboardCli/Program.cs` does — run `dotnet run --project demo/ScoreboardCli` for
an interactive REPL covering the same scenarios instead of pasting these into a scratch console:

```csharp
var dbContext = new ScoreboardDbContext(
    new DbContextOptionsBuilder<ScoreboardDbContext>()
        .UseSqlite(ScoreboardDbContextFactory.DefaultConnectionString)
        .Options);
dbContext.Database.Migrate();
var scoreboard = new Scoreboard(new SqliteMatchRepository(dbContext));
```

1. **Start a match** (User Story 1, Scenario 1):
   ```csharp
   var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
   // match is not null; match.HomeTeam.Score == 0; match.AwayTeam.Score == 0; match.Status == MatchStatus.InProgress
   ```

2. **Reject a team already in-progress elsewhere** (Scenario 2):
   ```csharp
   scoreboard.StartMatch("Mexico", "Spain", DateTime.UtcNow, "Different Venue");
   // returns null — Mexico is already in the match started above
   ```

3. **Reject a duplicate location+time** (Scenario 3):
   ```csharp
   var t = DateTime.UtcNow;
   scoreboard.StartMatch("Germany", "France", t, "Estadio Azteca");
   scoreboard.StartMatch("Uruguay", "Italy", t, "Estadio Azteca");
   // second call returns null — same location and same instant as the first
   ```

4. **A finished match frees its slot** (Scenario 4 — depends on `003-finish-match`; not yet
   implementable in this feature, included here only to document the eventual expectation):
   this scenario cannot be exercised until `FinishMatch` exists. Leave as a forward reference, not
   a task of this feature.

5. **Retrieve by ID** (User Story 2):
   ```csharp
   var fetched = scoreboard.GetMatch(match!.Id);
   // fetched is not null and equals match's recorded data
   var missing = scoreboard.GetMatch(-1);
   // missing is null
   ```

## Expected outcome

`dotnet test` reports all `StartMatchTests` passing, and the manual snippets above (steps 1-3, 5)
behave as described when run in a scratch console/REPL against the built library.
