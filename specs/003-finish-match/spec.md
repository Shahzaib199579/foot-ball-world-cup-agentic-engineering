# Feature Specification: Finish Match

**Feature Branch**: `003-finish-match`

**Created**: 2026-08-04

**Status**: Draft

**Input**: User description: "003-finish-match. A match's data if marked finished still exists
in db. A finish match's status can't be changed to in-progress. One a match is marked finished
then score can't be updated."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Finish an in-progress match (Priority: P1)

As a caller of the scoreboard library, I want to mark an in-progress match as finished, so that
its final result is locked in and the match stops appearing as live while its full record is
still available for later reference.

**Why this priority**: This is the third of the brief's four required core operations, and the
only way a match ever leaves the "in progress" state. `004-live-summary` (matches in progress
only) and `005-match-history` (in-progress *and* finished matches) both depend on a match being
able to reach this state.

**Independent Test**: Start a match, update its score, then finish it; verify via `GetMatch` that
its status is now finished and every other recorded attribute (teams, final score, scheduled
date/time, location) is unchanged and still retrievable. Attempting to finish it again, or to
update its score, is rejected; a new match may now be started at the same location and date/time
the finished match used.

**Acceptance Scenarios**:

1. **Given** an in-progress match with a recorded score, **When** it is marked finished, **Then**
   its status becomes finished and it remains retrievable by its match ID with its final score
   and every other recorded attribute unchanged.
2. **Given** a match that has already been finished, **When** finishing is attempted again,
   **Then** the operation raises an error and the match's status remains finished.
3. **Given** a match ID that does not correspond to any existing match, **When** finishing is
   attempted, **Then** the operation raises an error.
4. **Given** a match that has been finished, **When** a score update is attempted against it,
   **Then** the operation raises an error and the match's final score remains unchanged.
5. **Given** a match that was finished at Location X for date/time T, **When** a new match is
   started at that same Location X and date/time T, **Then** the new match starts successfully.

---

### Edge Cases

- What happens when a match is finished immediately after starting, with no score updates made?
  → Allowed — finishing has no minimum-score or minimum-duration precondition; the match finishes
  with whatever score it currently has (0-0 if never updated).
- What happens to a team that was in a match that has just been finished — can it start a new
  match right away? → Yes — the existing "a team cannot be in more than one in-progress match"
  rule (`001-start-match`) only considers in-progress matches; a finished match no longer counts.
- Is there any way to move a finished match back to in-progress (e.g., to correct a mistake)? →
  No — this is a deliberate one-way transition (FR-003); no "reopen" or "unfinish" operation is
  provided by this feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow marking an in-progress match as finished, given its match ID.
- **FR-002**: A finished match's full recorded data (teams, final score, scheduled date/time,
  location) MUST remain retrievable by its match ID exactly as an in-progress match's data is —
  finishing MUST NOT delete, archive, or otherwise remove a match's data from where it's stored.
- **FR-003**: System MUST NOT provide any operation that changes a finished match's status back
  to in-progress — finishing is a one-way, terminal transition.
- **FR-004**: System MUST raise an error when finishing is attempted against a match ID that does
  not correspond to any existing match, or that corresponds to a match that has already been
  finished.
- **FR-005**: Once a match is finished, System MUST reject any subsequent attempt to update its
  score, leaving its final score unchanged (extends `002-update-score`'s existing rejection of
  score updates against a non-in-progress match).
- **FR-006**: A finished match's location and scheduled date/time MUST become available for reuse
  by a new match — the "no two in-progress matches share a location and date/time" rule
  (`001-start-match`) only ever applied to in-progress matches.
- **FR-007**: Finishing a match MUST NOT change any of its other recorded attributes (match ID,
  teams, score, scheduled date/time, location).

### Key Entities

- **Match** (established by `001-start-match`): gains a new reachable value for its `Status`
  field, `Finished`, via the one-way transition `InProgress → Finished` introduced by this
  feature. No other change to its shape.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A caller can finish an in-progress match and immediately retrieve its full recorded
  data, including its final score, by the same match ID.
- **SC-002**: 100% of attempts to finish a nonexistent or already-finished match are rejected with
  a clear error, with no change to any match's data.
- **SC-003**: 100% of score-update attempts against a finished match are rejected, and that
  match's final score never changes afterward.
- **SC-004**: 100% of new-match start attempts that reuse a finished match's exact location and
  date/time succeed, where they would have been rejected while that match was still in progress.

## Assumptions

- **Rejection is via a raised error, not a non-throwing result**: consistent with
  `002-update-score`'s own choice (following CLAUDE.md's general "operating on a non-existent or
  already-finished match throws" convention), not `001-start-match`'s specific non-throwing
  `null` result for `StartMatch`. `FinishMatch` behaves like `UpdateScore` here, not like
  `StartMatch`.
- **Reuses `002-update-score`'s `MatchNotFoundException`**: that exception was deliberately
  written generically (keyed on match ID, not `UpdateScore`-specific) specifically so this
  feature could reuse it for "no such in-progress match" without introducing a near-duplicate
  type.
- **This feature is what makes several already-written checks in `001-start-match` and
  `002-update-score` reachable for the first time**: `Scoreboard.StartMatch`'s conflict checks
  and `Scoreboard.UpdateScore`'s in-progress check were both written against
  `existing.Status != MatchStatus.InProgress`/similar, anticipating a `Finished` value that
  didn't exist until now. This feature's own scope is therefore narrower than it might first
  appear: add `Finished` to the `MatchStatus` enum, and add the `FinishMatch` operation itself —
  `StartMatch` and `UpdateScore`'s existing logic should not need to change to satisfy FR-005/
  FR-006 above.
- **No "reopen"/"unfinish" operation**: deliberately not introduced, per constitution
  Single-Concern Features — a genuine correction/reopen workflow is out of scope and would be its
  own future spec if ever needed.
- **"Still exists in db" (the user's own phrasing)**: a finished match is not deleted, moved to a
  separate table/store, or excluded from persistence — it simply carries `Status = Finished` in
  the same storage as every other match, retrievable the same way via `GetMatch`.
- This feature covers only *finishing* an in-progress match. Live summaries (`004-live-summary`,
  in-progress matches only, so finished matches stop appearing there) and history
  (`005-match-history`, in-progress *and* finished matches) are separate specs and out of scope
  here.
