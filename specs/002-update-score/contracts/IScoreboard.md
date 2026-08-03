# Contract: IScoreboard (this feature's slice)

Public library contract addition for `002-update-score`. Only documents what this feature adds
to `IScoreboard` — `StartMatch`/`GetMatch` are `001-start-match`'s contract and unchanged here
(see `specs/001-start-match/contracts/IScoreboard.md`).

## `UpdateScore`

```csharp
Match UpdateScore(int matchId, int homeScore, int awayScore);
```

**Preconditions checked:**

- A `Match` with the given `matchId` exists and its `Status == MatchStatus.InProgress`;
  otherwise throws `MatchNotFoundException`.
- `homeScore` and `awayScore` are each non-negative integers; otherwise throws
  `InvalidScoreException`.
- `homeScore >= match.HomeTeam.Score` and `awayScore >= match.AwayTeam.Score` (per team,
  independently); otherwise throws `InvalidScoreException`. Equal is accepted — this is a
  non-decrease check, not a strict-increase check.

**Postconditions on success:**

- Returns the same `Match` instance (by `Id`), with `HomeTeam.Score == homeScore` and
  `AwayTeam.Score == awayScore`.
- Every other recorded attribute of the match (`Id`, `HomeTeam`/`AwayTeam` identity,
  `ScheduledAt`, `Location`, `Status`) is unchanged.
- The update is visible to a subsequent `GetMatch(matchId)` call.

**Postconditions on failure:**

- Throws `MatchNotFoundException` or `InvalidScoreException` (see Preconditions above). No
  exception path leaves the match's previously recorded score partially updated — both scores
  are validated before either is mutated.

## Notes

- Safe to call concurrently with `StartMatch`/`GetMatch`; `Scoreboard` serializes access
  internally via the same coarse lock established by `001-start-match` (research.md §4 there;
  reused, not re-introduced, by this feature).
- This contract will gain more methods as specs `003-005` are implemented (`FinishMatch`,
  `GetSummary`, `GetHistory`); each addition is that spec's own contract change.
