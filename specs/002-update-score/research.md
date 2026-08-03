# Research: Update Score

Phase 0 output for `specs/002-update-score`. No open `NEEDS CLARIFICATION` markers remained in
the spec after `/speckit-clarify` (zero questions asked — full coverage on the first
`/speckit-specify` pass). The items below are design/pattern decisions needed to move from spec
to data model and contract, not unresolved requirements.

## 1. Failure-signaling shape for `UpdateScore` (FR-004, FR-005)

- **Decision**: `UpdateScore` returns the updated `Match` on success (non-nullable) and throws a
  custom exception on any rejection: `InvalidScoreException` for FR-002/FR-003 (malformed or
  decreasing score), `MatchNotFoundException` for FR-005 (no in-progress match with that ID).
- **Rationale**: the spec's own Assumptions section already settled this — rejection is via a
  raised error, following CLAUDE.md's general convention ("operating on a non-existent or
  already-finished match throws"), explicitly distinct from `001-start-match`'s own specific
  non-throwing `null` result for `StartMatch`/`GetMatch` (that carve-out was scoped to
  `001-start-match`'s clarification session, not the whole project). Returning the updated
  `Match` on success (rather than `void`) mirrors `StartMatch`'s existing pattern of handing the
  caller the resulting state directly, saving a follow-up `GetMatch` call.
- **Alternatives considered**:
  - `void UpdateScore(...)`, forcing callers to call `GetMatch` afterward to see the result:
    rejected — asymmetric with `StartMatch`'s existing "return what you just did" pattern for no
    real benefit.
  - A non-throwing `bool TryUpdateScore(...)`/nullable-return pattern matching `StartMatch`:
    rejected — the spec's Assumptions section explicitly chose the throwing convention for this
    feature; introducing a second non-throwing style alongside it would be an unforced
    inconsistency with what was already decided at spec time, not a fresh design choice for plan
    time to make.

## 2. Two exception types vs. one

- **Decision**: two distinct exception types — `MatchNotFoundException` (FR-005: match ID
  doesn't resolve to an in-progress match) and `InvalidScoreException` (FR-002/FR-003: malformed
  or decreasing score value) — both under `src/WorldCupScoreboard/Exceptions/`.
- **Rationale**: the two failure modes are semantically distinct (an identity/lifecycle problem
  vs. a value problem) and a caller may reasonably want to handle them differently (e.g., a future
  `006-scoreboard-api` layer would map `MatchNotFoundException` to HTTP 404 and
  `InvalidScoreException` to HTTP 400). Splitting them now costs nothing and avoids a later
  breaking change to widen a single generic exception's meaning.
- **Alternatives considered**:
  - One generic `ScoreboardException` for all `UpdateScore` failures: rejected — would force
    every future caller to inspect the message string to distinguish a 404-shaped problem from a
    400-shaped one.
  - Reusing a single exception type across all present and future specs
    (`003-finish-match` will need its own "not found"/"invalid state" signaling too): deferred —
    `MatchNotFoundException` is written generically enough (keyed on match ID, not on
    `UpdateScore` specifically) that `003-finish-match` can reuse it as-is when it needs the same
    "no such in-progress match" check, without this spec having to design that ahead of time.

## 3. Validation order: resolve the match before validating the new score

- **Decision**: `UpdateScore` first resolves the match by ID and confirms it's in-progress
  (throwing `MatchNotFoundException` if not), *then* validates both new scores against that
  match's current recorded scores (throwing `InvalidScoreException` if either is malformed or a
  decrease).
- **Rationale**: FR-003's decrease check is only meaningful relative to a specific match's
  current score — there is no way to evaluate "is this new score lower than the current one"
  without first resolving which match, and its current score, is in play. Checking existence
  first also matches the intuitive reading of FR-005's precondition role.
- **Alternatives considered**: validating the new score's non-negativity independently of match
  resolution first (order-independent for that one check, since FR-002 doesn't need the current
  score) — not wrong, but rejected in favor of always resolving the match first everywhere, for
  one consistent, easy-to-state rule rather than two different orderings for FR-002 vs FR-003.

## 4. Atomicity of the two-team update (FR-004)

- **Decision**: validate *both* new scores (FR-002 and FR-003, for home and away) fully before
  mutating either `Team.Score`. Only after both pass does `UpdateScore` mutate `HomeTeam.Score`
  and `AwayTeam.Score` and call `repository.Update(match)`.
- **Rationale**: this is the only implementation order that satisfies FR-004's "all-or-nothing,
  never partial" requirement without needing a rollback/undo step — validate-then-mutate is
  simpler than mutate-then-rollback-on-failure and carries no performance cost at this scale.
- **Alternatives considered**: mutate home score, then validate/mutate away score, rolling back
  the home mutation on an away-score failure: rejected — strictly more complex for the same
  outcome, and introduces a window (however small, under the same lock) where the in-memory
  `Match` object is transiently inconsistent with what's been validated.

## 5. No new persistence migration needed

- **Decision**: no EF Core migration is generated for this feature.
- **Rationale**: `001-start-match`'s `InitialCreate` migration already created the
  `HomeTeamScore`/`AwayTeamScore` columns, and `Team.Score` was already given an `internal set`
  accessor during that feature's persistence retrofit (needed for EF Core materialization, not
  originally for this feature's sake) — this feature happens to need exactly that mutability with
  no further schema change.
