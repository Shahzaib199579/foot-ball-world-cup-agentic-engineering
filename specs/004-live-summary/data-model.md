# Data Model: Live Summary

Phase 1 output for `specs/004-live-summary`. This feature introduces no new entity — it adds
one computed, non-persisted property to `Match` (established by `001-start-match`).

## Match — updated

| Field | Type | Notes |
|---|---|---|
| `TotalScore` (new) | `int` | Computed: `HomeTeam.Score + AwayTeam.Score`. Get-only, no
  backing field, not mapped by EF Core (research.md §1-§2). Always correct — there is no state
  for it to desynchronize from, since it's recomputed from the two team scores on every access. |

No other field changes. `Id`, `HomeTeam`/`AwayTeam`, `ScheduledAt`, `Location`, `Status` are
unchanged (this feature performs no writes to any `Match` at all — FR-005, read-only).

### Ordering rule (from Functional Requirements)

The summary (this feature's `GetSummary`) includes every `Match` where `Status ==
MatchStatus.InProgress`, ordered by:

1. `TotalScore` descending (FR-002).
2. Among equal `TotalScore` values, `Id` descending — `Id` is the existing monotonic sequence
   from `001-start-match`, so higher `Id` means more-recently-started (FR-003, research.md §3).

### Validation rules

None — this feature adds no new rejection path. `GetSummary` never throws; an empty result
(when no match is in-progress) is a valid, ordinary outcome, not an error (spec.md Edge Cases).
