# Data Model: Match History

Phase 1 output for `specs/005-match-history`. This feature adds one new persisted field to
`Match` (established by `001-start-match`) and one new exception type.

## Match — updated

| Field | Type | Notes |
|---|---|---|
| `ActivitySequence` (new) | `int` | Monotonic, assigned by `Scoreboard` (not the database — mapped `ValueGeneratedNever()`, same as `Id`). Bumped on creation (`StartMatch`), score update (`UpdateScore`), and finishing (`FinishMatch`) — research.md §1-§2. Persisted as a real `INTEGER` column via a new EF Core migration (unlike `004-live-summary`'s computed, unmapped `TotalScore`). |

No other field changes. `Id`, `HomeTeam`/`AwayTeam`, `ScheduledAt`, `Location`, `Status`,
`TotalScore` are unchanged.

### Ordering rule (from Functional Requirements)

`GetHistory(int page)` includes **every** `Match` regardless of `Status` (FR-007 — no
exclusion, unlike `GetSummary`'s in-progress-only scope), ordered by `ActivitySequence`
descending, then paginated: entries `(page - 1) * 10` through `page * 10 - 1` (0-indexed into
the ordered sequence) are returned.

### Validation rules

- `page` MUST be `>= 1`; otherwise `GetHistory` throws `InvalidPageException` (FR-005).
- A `page` beyond the available data (e.g., page 5 when only 20 matches — 2 pages — exist)
  returns an empty result, not an exception (FR-004).
- `GetHistory` performs no writes — read-only (FR-006).

## New Exception

| Type | Thrown when | Notes |
|---|---|---|
| `InvalidPageException` | `GetHistory` is called with `page < 1` (FR-005) | New — no existing exception type fits this validation concern (research.md §3) |

## State/field transitions (this feature's scope)

- `Match.ActivitySequence`: assigned once at creation, then reassigned (always increasing) on
  every subsequent `UpdateScore` or `FinishMatch` call against that match. Never decreases,
  never resets.
