# Research: Match History

Phase 0 output for `specs/005-match-history`. No open `NEEDS CLARIFICATION` markers remained
after `/speckit-clarify` (zero questions asked — the user's follow-up context about page
size-vs-page-number was already correctly resolved in the spec).

## 1. `ActivitySequence`: a new persisted, monotonic field — not computed, not `DateTime`

- **Decision**: add `Match.ActivitySequence` (`int`, `internal set`), a monotonic counter
  bumped by `Scoreboard` on every create/update/finish. Persisted as a real `INTEGER` column
  via a new EF Core migration, mapped `ValueGeneratedNever()` (same reasoning as `Match.Id`).
- **Rationale**: unlike `004-live-summary`'s `TotalScore` (fully derivable from two other
  already-persisted values, hence computed and unmapped), "most recent activity" has no other
  source to derive from — it's an independent fact about *when* (in relative order) something
  last happened to a match, so it must be its own stored value. A monotonic sequence rather
  than `DateTime` follows `001-start-match`'s own established rationale for `Match.Id` (avoid
  timestamp-resolution ambiguity, keep tests deterministic) — reused a second time, after
  `004-live-summary` already reused it once for its tie-break.
- **Alternatives considered**:
  - Reusing `Match.Id` itself for this ordering: rejected — `Id` reflects only *creation* order
    and is immutable after creation (per `001-start-match`'s data-model.md); it cannot also
    represent "most recently updated," which is exactly what FR-002/Acceptance Scenario 3
    require (a match updated long after creation must rank ahead of a more-recently-created,
    never-updated match).
  - `DateTime.UtcNow` per activity: rejected for the same reason `001-start-match` rejected it
    for `Id` — timestamp-resolution ties are possible in fast automated tests, and wall-clock
    values aren't needed for anything else here.

## 2. `StartMatch`/`UpdateScore`/`FinishMatch` all need one new line each

- **Decision**: confirmed directly against current source (not assumed) — none of the three
  existing mutating methods track any activity/recency concept today. Each needs exactly one
  added line: assign the next `ActivitySequence` value before calling `repository.Add`/
  `repository.Update`.
- **Rationale**: this is the scope-expectation flag spec.md's own Assumptions already raised —
  unlike `003`/`004`, there is no pre-existing defensive code to activate here. Verifying this
  against real source before finalizing the plan avoids repeating a mistake in the opposite
  direction (assuming a "free lunch" that isn't actually there).
- **Alternatives considered**: computing "activity" purely from existing fields (e.g., some
  combination of `Id` and `Status`): rejected — there is no existing field that captures "this
  match's score was just updated," which is exactly the case FR-002 needs to detect.

## 3. New `InvalidPageException`, not a reused exception type

- **Decision**: add `Exceptions/InvalidPageException.cs`, thrown by `GetHistory` when
  `page < 1` (FR-005).
- **Rationale**: neither existing exception type fits — `MatchNotFoundException` is about match
  identity/lifecycle, `InvalidScoreException` is about score values; a page-number validation
  error is a distinct concern from both. Consistent with `002-update-score`'s original decision
  to split exception types by semantic meaning rather than reuse a generic one (research.md §2
  there).
- **Alternatives considered**: reusing a generic `ArgumentException`/`ArgumentOutOfRangeException`
  from the BCL directly: rejected for consistency — every other rejection path in this library
  (`MatchNotFoundException`, `InvalidScoreException`) is a purpose-named custom type under
  `Exceptions/`, and callers already expect to catch this project's own exception types, not
  BCL ones, per the precedent `002`/`003` established.

## 4. Pagination implemented in-memory over `IMatchRepository.GetAll()`, not a new repository method

- **Decision**: `Scoreboard.GetHistory` calls `repository.GetAll()`, orders by
  `ActivitySequence` descending, then applies `.Skip((page - 1) * 10).Take(10)`.
- **Rationale**: matches `GetSummary`'s (`004`) already-established pattern — sorting/paging in
  `Scoreboard` over an in-memory list keeps all business logic in the library layer (Principle
  IV) and avoids introducing a new, more complex `IMatchRepository` method (e.g., a paged query)
  ahead of an actual performance need. At this project's unchanged scale/scope, this is not a
  bottleneck.
- **Alternatives considered**: adding `IMatchRepository.GetPage(int page, int pageSize)` to push
  pagination down to the database query: rejected for now as premature optimization — no
  performance goal in Technical Context justifies it, and it would be a bigger interface change
  than this feature's behavior requires. Could be revisited if a real scale need ever arises.
