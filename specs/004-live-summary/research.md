# Research: Live Summary

Phase 0 output for `specs/004-live-summary`. No open `NEEDS CLARIFICATION` markers remained
after `/speckit-clarify` (zero questions asked). The items below are design/pattern decisions
needed to move from spec to data model and contract.

## 1. `TotalScore` as a computed property, not a persisted column

- **Decision**: `Match.TotalScore` is a get-only, expression-bodied C# property
  (`public int TotalScore => HomeTeam.Score + AwayTeam.Score;`) — not backed by a field, not
  independently settable, and not mapped by EF Core.
- **Rationale**: the spec's own Assumptions section already deferred the storage question to
  plan-level. A computed property is correct by construction — there is no window where it can
  be "out of sync" with the two team scores, because it has no independent state to go out of
  sync. This also means FR-004 ("kept correct immediately after any score update") is satisfied
  automatically by `002-update-score`'s existing `UpdateScore` implementation, with zero code
  changes to it — the same "already-defensive code becomes reachable" pattern seen in
  `003-finish-match`, but here it's "already-correct code," not "already-defensive."
- **Alternatives considered**:
  - A persisted `TotalScore` column, recalculated and written by `UpdateScore`: rejected —
    would require touching `002-update-score`'s `Scoreboard.UpdateScore` and an EF Core
    migration, for a value with no independent meaning and no query-performance need at this
    scale (per plan.md's Scale/Scope, unchanged from prior features).
  - A method (`Match.GetTotalScore()`) instead of a property: rejected — a property better
    conveys "this is just a derived fact about the match," consistent with C# idiom for
    computed values with no side effects or cost beyond simple arithmetic.

## 2. EF Core does not need `[NotMapped]` or `OnModelCreating` changes

- **Decision**: no explicit exclusion annotation for `TotalScore` — verified this is unnecessary
  before relying on it.
- **Rationale**: `001-start-match`'s persistence retrofit already established (the hard way,
  via a real bug) that EF Core's model-building convention includes an entity's properties
  based on whether they have a usable setter or backing field it can write to — that's exactly
  why `Match.Location`/`ScheduledAt`/etc. needed `internal set` accessors added. `TotalScore`
  has neither a setter nor a backing field (it's a pure expression), so it fails that same
  inclusion test in the opposite direction: EF Core has nothing to write to, so it excludes the
  property from the model automatically. This is the standard, documented pattern for
  "computed, not persisted" properties in EF Core.
- **Alternatives considered**: proactively adding `[NotMapped]` "just in case": considered, but
  rejected as unnecessary — plan.md states the convention-based exclusion directly rather than
  hedging with a redundant attribute; if `/speckit-implement` finds the assumption wrong when
  actually building/running, that's exactly what Principle II's reproduce-first step is for.

## 3. Tie-break reuses `Match.Id`, no new "start order" field

- **Decision**: `GetSummary`'s tie-break is `ORDER BY TotalScore DESC, Id DESC` (in LINQ:
  `.OrderByDescending(m => m.TotalScore).ThenByDescending(m => m.Id)`).
- **Rationale**: `001-start-match`'s research.md §1 already committed to `Match.Id` as a
  monotonic in-memory sequence counter specifically "to avoid tie-break ambiguity from
  timestamp resolution" for exactly this future live-summary ordering rule. This feature is
  what actually consumes that decision — no new field, no wall-clock comparison.
- **Alternatives considered**: comparing `ScheduledAt`: rejected — `ScheduledAt` is
  caller-supplied, potentially identical across matches (`001-start-match` allows same-time
  matches at different locations), and explicitly not the "start order" concept per `001`'s own
  research.

## 4. `GetSummary` returns `Match`, not a new projection/DTO type

- **Decision**: `IEnumerable<Match> GetSummary()` — returns the same `Match` entity every other
  operation already returns.
- **Rationale**: simplicity; no caller-facing need for a narrower "summary row" shape has been
  identified, and introducing one would be speculative design ahead of an actual requirement
  (same reasoning research.md has applied consistently since `001-start-match`).
- **Alternatives considered**: a `MatchSummary` record exposing only `Id`/teams/scores/total:
  rejected for now — nothing in spec.md asks for a narrower shape, and `Match` already carries
  everything `GetSummary`'s callers need.
