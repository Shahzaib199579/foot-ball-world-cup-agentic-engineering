# Data Model: Update Score

Phase 1 output for `specs/002-update-score`. This feature introduces no new entity — it makes an
existing field of an existing entity (`Team.Score`, from `001-start-match`) mutable through a new
business rule, and introduces two new exception types.

## Match (established by `001-start-match`, unchanged shape)

No fields added or changed. `UpdateScore` (this feature) may mutate `HomeTeam.Score`/
`AwayTeam.Score` in place; `Id`, `HomeTeam`/`AwayTeam` (identity), `ScheduledAt`, `Location`, and
`Status` are all explicitly unaffected (FR-006).

### Validation rules added by this feature

- A score update MUST resolve to a `Match` whose `Status == MatchStatus.InProgress`; otherwise
  reject with `MatchNotFoundException` (FR-005). (`001-start-match`'s data-model.md already
  reserves `Finished` for `003-finish-match` — until that lands, only a nonexistent match ID can
  trigger this branch.)
- Both `homeScore` and `awayScore` supplied to an update MUST be non-negative integers (FR-002).
- Neither `homeScore` nor `awayScore` may be lower than that team's current recorded `Score`
  (FR-003) — equal is accepted (non-decrease, not strict increase).
- Any violation of the two rules above → `UpdateScore` throws `InvalidScoreException`; the
  `Match`'s `HomeTeam.Score`/`AwayTeam.Score` are left exactly as they were (FR-004) — validated
  fully before either is mutated (research.md §4).

## Team (established by `001-start-match`, one field's mutability formalized here)

| Field | Type | Notes |
|---|---|---|
| `Name` | `string` | Unchanged. |
| `Score` | `int` | Was initialized to `0` on creation and never mutated by `001-start-match` ("Mutation is out of scope for this feature — added by spec `002-update-score`," per that feature's own data-model.md). **This feature adds that mutation**, subject to the validation rules above. The `internal set` accessor already exists (added during `001-start-match`'s persistence retrofit for EF Core materialization) — no code change to `Team.cs` itself is needed. |

## New Exceptions (this feature's first use of the `Exceptions/` folder)

| Type | Thrown when | Notes |
|---|---|---|
| `MatchNotFoundException` | `UpdateScore` is called with a match ID that doesn't resolve to an in-progress match (FR-005) | Keyed on match ID (not `UpdateScore`-specific wording) so `003-finish-match` can reuse it (research.md §2) |
| `InvalidScoreException` | Either new score is negative or lower than the team's current recorded score (FR-002/FR-003) | Carries enough detail (which team, attempted value, current value) to build a useful error message |

## State transitions (this feature's scope)

- `Match.HomeTeam.Score` / `Match.AwayTeam.Score`: `n` → `m` where `m >= n`, for an in-progress
  match only. No transition of `Match.Status` is introduced by this feature — that remains
  `003-finish-match`'s concern.
