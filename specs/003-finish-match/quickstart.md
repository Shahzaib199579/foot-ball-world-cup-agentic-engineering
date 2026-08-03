# Quickstart: Finish Match

Validation guide for `specs/003-finish-match`. Builds on `001-start-match`/`002-update-score`'s
already-implemented `Scoreboard` — this feature adds `FinishMatch` and the `Finished` status.

## Prerequisites

- .NET 9 SDK installed (`dotnet --version` → `9.x`).
- `001-start-match` and `002-update-score` implemented (they are).

## Build & test

```bash
dotnet build
dotnet test
```

All tests in `tests/WorldCupScoreboard.Tests/FinishMatchTests.cs`,
`FinishMatchRejectionTests.cs`, and `FinishMatchSideEffectsTests.cs` must pass, alongside the
full existing `001`/`002` suite (no regression).

## Manual validation walkthrough

Exercises the contract in `contracts/IScoreboard.md` directly, mirroring the spec's acceptance
scenarios. Uses the same SQLite-backed `Scoreboard` construction as prior features' quickstarts
— or run `dotnet run --project demo/ScoreboardCli` and use its `finish` command once this
feature adds it.

1. **Finish an in-progress match** (Acceptance Scenario 1):
   ```csharp
   var match = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
   scoreboard.UpdateScore(match!.Id, 2, 1);
   var finished = scoreboard.FinishMatch(match.Id);
   // finished.Status == MatchStatus.Finished; finished.HomeTeam.Score == 2; still findable:
   var fetched = scoreboard.GetMatch(match.Id);
   // fetched is not null; fetched.Status == MatchStatus.Finished
   ```

2. **Finishing twice is rejected** (Acceptance Scenario 2):
   ```csharp
   Assert.Throws<MatchNotFoundException>(() => scoreboard.FinishMatch(match.Id));
   ```

3. **Finishing a nonexistent match is rejected** (Acceptance Scenario 3):
   ```csharp
   Assert.Throws<MatchNotFoundException>(() => scoreboard.FinishMatch(-1));
   ```

4. **Score updates are rejected after finishing** (Acceptance Scenario 4):
   ```csharp
   Assert.Throws<MatchNotFoundException>(() => scoreboard.UpdateScore(match.Id, 3, 1));
   // final score is still 2-1 — verify via GetMatch
   ```

5. **A finished match's location/time can be reused** (Acceptance Scenario 5):
   ```csharp
   var reused = scoreboard.StartMatch("Germany", "France", match.ScheduledAt, match.Location);
   // reused is not null — no conflict, because `match` is no longer in-progress
   ```

## Expected outcome

`dotnet test` reports all new tests passing (plus the full `001`/`002` suite still green), and
the manual snippets above behave as described when run in a scratch console/REPL or via the CLI
demo's `finish` command.
