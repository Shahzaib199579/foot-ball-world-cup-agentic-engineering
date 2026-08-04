# Contract: IScoreboard (this feature's slice)

Public library contract addition for `004-live-summary`. Only documents what this feature adds
to `IScoreboard` — `StartMatch`/`GetMatch` (`001`), `UpdateScore` (`002`), and `FinishMatch`
(`003`) are unchanged here.

## `GetSummary`

```csharp
IEnumerable<Match> GetSummary();
```

**Preconditions:** none — always callable, no arguments.

**Postconditions:**

- Returns every `Match` whose `Status == MatchStatus.InProgress`. No finished match is
  included.
- Ordered by `TotalScore` (`HomeTeam.Score + AwayTeam.Score`) descending. Among matches with
  equal `TotalScore`, ordered by `Id` descending (most-recently-started first).
- Returns an empty sequence (never `null`, never throws) when no match is in-progress.
- Never modifies any match's data — purely a read.

## Notes

- Safe to call concurrently with `StartMatch`/`GetMatch`/`UpdateScore`/`FinishMatch`;
  `Scoreboard` serializes access internally via the existing coarse lock, for a consistent
  snapshot at the moment of the call.
- `Match.TotalScore` (new, computed) is public on every `Match`, including those returned by
  `GetMatch` — not exclusive to `GetSummary`'s results.
- This contract will gain one more method as `005-match-history` is implemented (`GetHistory`,
  now including pagination per CLAUDE.md's Confirmed Decisions); that is that spec's own
  contract change.
