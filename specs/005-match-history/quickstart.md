# Quickstart: Match History

Validation guide for `specs/005-match-history`. Builds on `001`-`004`'s already-implemented
`Scoreboard` — this feature adds `GetHistory`, `Match.ActivitySequence`, and
`InvalidPageException`.

## Prerequisites

- .NET 9 SDK installed (`dotnet --version` → `9.x`).
- `001-start-match` through `004-live-summary` implemented (they are).

## Build & test

```bash
dotnet build
dotnet test
```

All tests in `tests/WorldCupScoreboard.Tests/GetHistoryPaginationTests.cs`,
`GetHistoryOrderingTests.cs`, and `GetHistoryScopeTests.cs` must pass, alongside the full
existing `001`-`004` suite (no regression).

## Manual validation walkthrough

Exercises the contract in `contracts/IScoreboard.md` directly.

1. **Pagination returns exactly 10 per page** (Acceptance Scenarios 1-2):
   ```csharp
   for (var i = 0; i < 15; i++)
   {
       scoreboard.StartMatch($"Home{i}", $"Away{i}", DateTime.UtcNow, $"Venue{i}");
   }
   var page1 = scoreboard.GetHistory(1).ToList(); // 10 matches, most recently started first
   var page2 = scoreboard.GetHistory(2).ToList(); // remaining 5 matches
   ```

2. **A score update re-ranks a match ahead of more-recently-created ones** (Acceptance
   Scenario 3):
   ```csharp
   var oldest = scoreboard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
   scoreboard.StartMatch("C", "D", DateTime.UtcNow, "Venue2"); // created after `oldest`
   scoreboard.UpdateScore(oldest!.Id, 1, 0); // bumps `oldest` back to the front
   var history = scoreboard.GetHistory(1).ToList();
   // history[0].Id == oldest.Id, even though it was created first
   ```

3. **Fewer matches than one page** (Acceptance Scenario 4):
   ```csharp
   var emptyBoard = new Scoreboard(new InMemoryMatchRepository());
   emptyBoard.StartMatch("A", "B", DateTime.UtcNow, "Venue1");
   Assert.Single(emptyBoard.GetHistory(1));
   Assert.Empty(emptyBoard.GetHistory(2));
   ```

4. **Finished matches still appear** (Acceptance Scenario 5, contrast with `004`'s
   `GetSummary`):
   ```csharp
   var match = scoreboard.StartMatch("E", "F", DateTime.UtcNow, "Venue3");
   scoreboard.FinishMatch(match!.Id);
   Assert.Contains(scoreboard.GetHistory(1), m => m.Id == match.Id);
   ```

5. **No matches at all** (Acceptance Scenario 6):
   ```csharp
   Assert.Empty(new Scoreboard(new InMemoryMatchRepository()).GetHistory(1));
   ```

6. **An invalid page number is rejected** (Edge Case, FR-005):
   ```csharp
   Assert.Throws<InvalidPageException>(() => scoreboard.GetHistory(0));
   ```

## Expected outcome

`dotnet test` reports all new tests passing (plus the full `001`-`004` suite still green), and
the manual snippets above behave as described when run in a scratch console/REPL or via the CLI
demo's `history <page>` command.
