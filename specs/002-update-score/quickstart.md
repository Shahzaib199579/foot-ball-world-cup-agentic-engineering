# Quickstart: Update Score

Validation guide for `specs/002-update-score`. Builds on `001-start-match`'s already-implemented
`Scoreboard`/`IMatchRepository` — this feature adds `UpdateScore` and two new exception types.

## Prerequisites

- .NET 9 SDK installed (`dotnet --version` → `9.x`).
- `001-start-match` implemented (it is — `dotnet build`/`dotnet test` green as of that feature).

## Build & test

```bash
dotnet build
dotnet test
```

All tests in `tests/WorldCupScoreboard.Tests/UpdateScoreTests.cs` must pass — one test per
Functional Requirement (FR-001..FR-007) plus the acceptance scenarios in `spec.md`'s User Story 1,
alongside the full existing `001-start-match` suite (no regression).

## Manual validation walkthrough

Exercises the contract in `contracts/IScoreboard.md` directly, mirroring the spec's acceptance
scenarios. Uses the same SQLite-backed `Scoreboard` construction shown in
`specs/001-start-match/quickstart.md` (via `ScoreboardDbContext`/`SqliteMatchRepository`) — or run
`dotnet run --project demo/ScoreboardCli` and use its `update` command once this feature adds it.

1. **Update a score upward** (Acceptance Scenario 1):
   ```csharp
   var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
   var updated = scoreboard.UpdateScore(match!.Id, 2, 1);
   // updated.HomeTeam.Score == 2; updated.AwayTeam.Score == 1
   ```

2. **One team's score stays the same while the other increases** (Acceptance Scenario 2):
   ```csharp
   var updated2 = scoreboard.UpdateScore(match.Id, 3, 1);
   // updated2.HomeTeam.Score == 3; updated2.AwayTeam.Score == 1 (unchanged)
   ```

3. **A decrease is rejected** (Acceptance Scenario 3):
   ```csharp
   Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, 1, 1));
   // match's recorded score is still 3-1 afterward — verify via GetMatch
   ```

4. **A negative score is rejected** (Acceptance Scenario 4):
   ```csharp
   Assert.Throws<InvalidScoreException>(() => scoreboard.UpdateScore(match.Id, 3, -1));
   ```

5. **A malformed (letters/special characters) score is rejected at the contract boundary**
   (Acceptance Scenario 5): not directly exercisable in C# — `homeScore`/`awayScore` are typed
   `int`, so passing `"two"` is a compile-time error, not a runtime case (spec.md Assumptions).
   `006-scoreboard-api` will need its own test for this once it parses raw HTTP/JSON input.

6. **A nonexistent match ID is rejected** (Acceptance Scenario 6):
   ```csharp
   Assert.Throws<MatchNotFoundException>(() => scoreboard.UpdateScore(-1, 1, 0));
   ```

## Expected outcome

`dotnet test` reports all `UpdateScoreTests` passing (plus the full `001-start-match` suite still
green), and the manual snippets above (steps 1-4, 6) behave as described when run in a scratch
console/REPL or via the CLI demo's `update` command.
