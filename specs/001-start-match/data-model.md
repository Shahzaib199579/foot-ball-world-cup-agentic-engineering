# Data Model: Start New Match

Phase 1 output for `specs/001-start-match`. Reflects only what this feature needs; `MatchStatus`
and `IScoreboard` are established here as a skeleton and grow in later specs (002-005) — see
`research.md` §3 for why unused members/values are deliberately not pre-declared.

## Match

A single football match between two teams, created directly in an active (in-progress) state.

| Field | Type | Notes |
|---|---|---|
| `Id` | `int` | Unique; assigned from the monotonic sequence counter at creation (research.md §1). Immutable after creation. |
| `HomeTeam` | `Team` | See below. |
| `AwayTeam` | `Team` | See below. |
| `ScheduledAt` | `DateTime` | Required. May be past, present, or future (FR-003) — purely descriptive; does not gate activation. No timezone normalization is modeled (kata scope). |
| `Location` | `string` | Required, non-empty (FR-002). Plain text identifier (e.g., venue or city name); no structured venue registry (per spec Assumptions). |
| `Status` | `MatchStatus` | Always `InProgress` on creation in this feature. |

### Validation rules (from Functional Requirements)

- `HomeTeam.Name` and `AwayTeam.Name` MUST each be non-null/non-empty (FR-004).
- `HomeTeam.Name` MUST NOT equal `AwayTeam.Name` — a team cannot play itself (FR-004).
- `Location` MUST be non-null/non-empty (FR-002).
- No other **in-progress** match may already contain `HomeTeam` or `AwayTeam` (FR-005).
- No other **in-progress** match may already have the same `(Location, ScheduledAt)` pair,
  compared by exact instant equality — no overlapping-window/duration concept (FR-006). A
  finished match's `(Location, ScheduledAt)` no longer counts (Clarifications, session
  2026-08-03).
- Any violation above → `StartMatch` returns `null` (FR-008); no `Match` is created.

### State transitions (this feature's scope)

- `(none)` → `InProgress`: created directly by `StartMatch`. No other transition is modeled by
  this feature. `InProgress` → `Finished` is out of scope here — added by spec `003-finish-match`,
  which will extend the `MatchStatus` enum and add the corresponding validation/transition rules.

## Team

One side in a match. Not a standalone, globally-tracked entity — each `Team` instance is owned by
exactly one `Match` (per spec Key Entities: "its current score **within that match**"). The same
country name can appear in a different `Team` instance in a different match, subject to the
one-in-progress-match-per-team rule above.

| Field | Type | Notes |
|---|---|---|
| `Name` | `string` | Required, non-empty. The team/country identifier. |
| `Score` | `int` | Initialized to `0` on creation (FR-001). Mutation is out of scope for this feature — added by spec `002-update-score`. |

## MatchStatus (enum)

| Value | Introduced by | Notes |
|---|---|---|
| `InProgress` | 001-start-match (this feature) | The only value this feature needs. |
| `Finished` | 003-finish-match (future) | Not declared yet — see research.md §3. |
