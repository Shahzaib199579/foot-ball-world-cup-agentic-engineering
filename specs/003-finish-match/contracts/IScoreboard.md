# Contract: IScoreboard (this feature's slice)

Public library contract addition for `003-finish-match`. Only documents what this feature adds
to `IScoreboard` — `StartMatch`/`GetMatch` (`001-start-match`) and `UpdateScore`
(`002-update-score`) are unchanged here.

## `FinishMatch`

```csharp
Match FinishMatch(int matchId);
```

**Preconditions checked:**

- A `Match` with the given `matchId` exists and its `Status == MatchStatus.InProgress`;
  otherwise throws `MatchNotFoundException` (reused unchanged from `002-update-score`) — this
  covers both a nonexistent match ID and a match that has already been finished.

**Postconditions on success:**

- Returns the same `Match` instance (by `Id`), with `Status == MatchStatus.Finished`.
- Every other recorded attribute (`Id`, `HomeTeam`/`AwayTeam` including their final scores,
  `ScheduledAt`, `Location`) is unchanged.
- The match remains retrievable via `GetMatch(matchId)`, exactly as before finishing.
- From this point on: `UpdateScore(matchId, ...)` throws `MatchNotFoundException` for this match
  (existing `002-update-score` behavior, now reachable); a new `StartMatch` call may reuse this
  match's teams and its exact `(Location, ScheduledAt)` pair (existing `001-start-match`
  behavior, now reachable).

**Postconditions on failure:**

- Throws `MatchNotFoundException`. No partial state change — a rejected finish attempt never
  changes `Status`.

## Notes

- Safe to call concurrently with `StartMatch`/`GetMatch`/`UpdateScore`; `Scoreboard` serializes
  access internally via the same coarse lock established by `001-start-match`.
- There is no `ReopenMatch`/`UnfinishMatch` operation — `Finished` is terminal by design
  (spec.md FR-003, Assumptions).
- This contract will gain more methods as specs `004-005` are implemented (`GetSummary`,
  `GetHistory`); each addition is that spec's own contract change.
