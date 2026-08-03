# Contract: IScoreboard (this feature's slice)

Public library contract exposed to callers (e.g., a future API layer, Phase 2). This is the
feature's external interface — see `research.md` §3 for why it only covers this feature's two
operations, not all five core operations from the brief.

## `StartMatch`

```csharp
Match? StartMatch(string homeTeam, string awayTeam, DateTime scheduledAt, string location);
```

**Preconditions checked (all non-throwing — see below):**
- `homeTeam` and `awayTeam` are each non-null and non-empty.
- `homeTeam` and `awayTeam` are different.
- `location` is non-null and non-empty.
- Neither `homeTeam` nor `awayTeam` is already part of another in-progress match.
- No other in-progress match already exists at the exact same `(location, scheduledAt)`.

**Postconditions on success:**
- Returns a new `Match` with a freshly assigned unique `Id`, `Status = InProgress`,
  `HomeTeam.Score = 0`, `AwayTeam.Score = 0`, and the given `scheduledAt`/`location` recorded.
- The match becomes visible to a subsequent `GetMatch(Id)` call.

**Postconditions on failure:**
- Returns `null`. No exception is thrown (FR-008). No match is created — the store is left
  unchanged.

## `GetMatch`

```csharp
Match? GetMatch(int matchId);
```

**Preconditions:** none (any `int` is a valid argument).

**Postconditions:**
- Returns the `Match` with the given `Id` if one exists (in-progress or otherwise, once later
  specs introduce other statuses).
- Returns `null` if no match with that ID exists. No exception is thrown.

## Notes

- Both methods are safe to call concurrently; `Scoreboard` serializes access internally
  (research.md §4). Callers do not need their own external locking.
- This contract will gain more methods as specs 002-005 are implemented (`UpdateScore`,
  `FinishMatch`, `GetSummary`, `GetHistory`); each addition is that spec's own contract change,
  not retrofitted here.
