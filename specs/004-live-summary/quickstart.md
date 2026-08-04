# Quickstart: Live Summary

Validation guide for `specs/004-live-summary`. Builds on `001`-`003`'s already-implemented
`Scoreboard` — this feature adds `GetSummary` and `Match.TotalScore`.

## Prerequisites

- .NET 9 SDK installed (`dotnet --version` → `9.x`).
- `001-start-match`, `002-update-score`, `003-finish-match` implemented (they are).

## Build & test

```bash
dotnet build
dotnet test
```

All tests in `tests/WorldCupScoreboard.Tests/GetSummaryOrderingTests.cs`,
`GetSummaryLiveUpdateTests.cs`, `GetSummaryScopeTests.cs`, and
`GetSummaryWorkedExampleTests.cs` must pass, alongside the full existing `001`-`003` suite (no
regression).

## Manual validation walkthrough

Exercises the contract in `contracts/IScoreboard.md` directly — this is the brief's own worked
example (Acceptance Scenario 1), the literal acceptance test CLAUDE.md commits to.

```csharp
var mexico = scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Venue1");
var spain = scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow, "Venue2");
var germany = scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Venue3");
var uruguay = scoreboard.StartMatch("Uruguay", "Italy", DateTime.UtcNow, "Venue4");
var argentina = scoreboard.StartMatch("Argentina", "Australia", DateTime.UtcNow, "Venue5");

scoreboard.UpdateScore(mexico!.Id, 0, 5);
scoreboard.UpdateScore(spain!.Id, 10, 2);
scoreboard.UpdateScore(germany!.Id, 2, 2);
scoreboard.UpdateScore(uruguay!.Id, 6, 6);
scoreboard.UpdateScore(argentina!.Id, 3, 1);

var summary = scoreboard.GetSummary().ToList();
// summary, in order: Uruguay 6-6, Spain 10-2, Mexico 0-5, Argentina 3-1, Germany 2-2
```

1. **The worked example produces the exact expected order** (Acceptance Scenario 1): as above.
2. **Higher total score ranks first** (Acceptance Scenario 2): already demonstrated (Spain/
   Uruguay's total of 12 rank above Mexico's 5, which ranks above Germany/Argentina's 4).
3. **Ties broken by most-recently-started** (Acceptance Scenario 3): Uruguay (started 4th)
   ranks above Spain (started 2nd) despite equal totals; Argentina (5th) ranks above Germany
   (3rd) likewise.
4. **A score update changes the summary** (Acceptance Scenario 4):
   ```csharp
   scoreboard.UpdateScore(germany.Id, 20, 0);
   var updated = scoreboard.GetSummary().ToList();
   // Germany now has total 20 and ranks first
   ```
5. **No in-progress matches → empty summary** (Acceptance Scenario 5):
   ```csharp
   var emptyBoard = new Scoreboard(new InMemoryMatchRepository());
   Assert.Empty(emptyBoard.GetSummary());
   ```
6. **A finished match disappears from the summary** (Acceptance Scenario 6):
   ```csharp
   scoreboard.FinishMatch(mexico.Id);
   var afterFinish = scoreboard.GetSummary();
   // afterFinish does not contain Mexico's match
   ```

## Expected outcome

`dotnet test` reports all new tests passing (plus the full `001`-`003` suite still green), and
the manual snippets above behave as described when run in a scratch console/REPL or via the CLI
demo's `summary` command.
