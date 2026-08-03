# Research: Finish Match

Phase 0 output for `specs/003-finish-match`. No open `NEEDS CLARIFICATION` markers remained
after `/speckit-clarify` (zero questions asked — full coverage on the first `/speckit-specify`
pass). The items below are design/pattern decisions needed to move from spec to data model and
contract.

## 1. Failure-signaling shape and exception reuse (FR-004)

- **Decision**: `FinishMatch` returns the updated `Match` on success (non-nullable) and throws
  `MatchNotFoundException` (from `002-update-score`, unchanged) when the match ID doesn't
  resolve to an in-progress match — covering both "doesn't exist" and "already finished" with
  one exception type, same as `UpdateScore` already does.
- **Rationale**: spec.md's Assumptions already settled this — `FinishMatch` behaves like
  `UpdateScore`, not like `StartMatch`'s non-throwing pattern. `MatchNotFoundException` was
  deliberately written generically (keyed on match ID, not `UpdateScore`-specific) during
  `002-update-score` specifically so this feature could reuse it — confirming that earlier
  design bet was correct rather than premature.
- **Alternatives considered**: a new `MatchAlreadyFinishedException` distinct from "doesn't
  exist at all": rejected — the spec doesn't ask callers to distinguish the two cases (FR-004
  treats them identically), and `UpdateScore`'s precedent already established one exception
  covering "no such in-progress match" for any reason.

## 2. `MatchStatus.Finished` requires no new persistence migration

- **Decision**: add `Finished` as a second `MatchStatus` enum member; generate no new EF Core
  migration.
- **Rationale**: `Match.Status` has been an `INTEGER` column since `001-start-match`'s
  `InitialCreate` migration, storing the enum's underlying numeric value. Adding a second enum
  member changes which integer values the *application* considers valid — it doesn't change the
  column's type or add a column, so there's nothing for EF Core to migrate.
- **Alternatives considered**: none seriously — this is a direct, low-risk consequence of how
  enums are already mapped, not a genuine design fork.

## 3. `StartMatch`/`UpdateScore` need no code changes to satisfy FR-005/FR-006

- **Decision**: do not modify `Scoreboard.StartMatch` or `Scoreboard.UpdateScore`. Both already
  check `existing.Status != MatchStatus.InProgress` (or equivalent) and were written during
  `001-start-match`/`002-update-score` anticipating a second status value. Once `Finished`
  exists, those checks become reachable for the first time and already produce the behavior
  FR-005 (reject score update on a finished match) and FR-006 (finished match's location/time
  becomes reusable) describe.
- **Rationale**: verified directly against the current source (`Scoreboard.cs`) before writing
  this plan — both checks are present exactly as described, not just recalled from memory.
  Re-implementing this logic here would be pure duplication.
- **Alternatives considered**: none — this is a verification step, not a design choice with
  real alternatives.

## 4. Test placement for FR-005/FR-006 (cross-feature side effects)

- **Decision**: add a dedicated `FinishMatchSideEffectsTests.cs` (not folded into
  `UpdateScoreValidationTests.cs` or `StartMatchConflictTests.cs`) covering "score update
  rejected after finish" and "location/time freed after finish."
- **Rationale**: these are `003-finish-match`'s own acceptance criteria (Acceptance Scenarios
  4-5) even though the code under test lives in `StartMatch`/`UpdateScore` — placing the tests
  under this feature's own test file keeps the traceability from FR to test file consistent
  with every prior feature, rather than retroactively editing `001`/`002`'s test files for a
  behavior that is really `003`'s to claim.
- **Alternatives considered**: adding cases to the existing `001`/`002` test files: rejected —
  would blur which feature's `/speckit-tasks` run is responsible for which test, and those
  files' existing scope (documented in their own features' plans) doesn't cover `Finished`.
