# Data Model: Finish Match

Phase 1 output for `specs/003-finish-match`. This feature introduces no new entity — it adds
the second reachable value of an existing enum (`MatchStatus`, from `001-start-match`) and the
one-way transition into it.

## MatchStatus (enum) — updated

| Value | Introduced by | Notes |
|---|---|---|
| `InProgress` | `001-start-match` | Unchanged. |
| `Finished` | `003-finish-match` (this feature) | Terminal — no transition out of it exists (spec.md FR-003; no "reopen" operation). Reserved by `001-start-match`'s own data-model.md ("`Finished` | `003-finish-match` (future) | Not declared yet") — this feature is what declares it. |

## Match (established by `001-start-match`, unchanged shape)

No fields added or changed. `FinishMatch` (this feature) may transition `Status` from
`InProgress` to `Finished` only; `Id`, `HomeTeam`/`AwayTeam` (identity/scores), `ScheduledAt`,
and `Location` are all explicitly unaffected (FR-007).

### Validation rules added by this feature

- A finish attempt MUST resolve to a `Match` whose `Status == MatchStatus.InProgress`;
  otherwise reject with `MatchNotFoundException` (FR-004) — reused unchanged from
  `002-update-score`, not a new exception type.
- Once `Status == MatchStatus.Finished`, no operation in this library transitions it back to
  `InProgress` (FR-003) — enforced by omission (no such method exists), not by a runtime check.

### Existing validation rules now reachable for the first time

- `Scoreboard.StartMatch`'s per-team and per-location/time conflict checks already skip any
  `Match` whose `Status != MatchStatus.InProgress` — a finished match's team and location/time
  slot become available for reuse (FR-006) the moment this feature makes `Finished` reachable.
- `Scoreboard.UpdateScore`'s existing precondition already throws `MatchNotFoundException` for
  any match whose `Status != MatchStatus.InProgress` — a finished match's score becomes
  immutable (FR-005) the moment this feature makes `Finished` reachable.

## State transitions (this feature's scope)

- `InProgress → Finished`: the only transition this feature introduces. One-way; no reverse
  transition exists anywhere in the library (FR-003).
