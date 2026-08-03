# Feature Specification: Update Score

**Feature Branch**: `002-update-score`

**Created**: 2026-08-03

**Status**: Draft

**Input**: User description: "002-update-score. It should be possible to update the score for
each team in the match. Score can only go up and never down."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Update the score of an in-progress match (Priority: P1)

As a caller of the scoreboard library, I want to update the recorded score for both teams in an
in-progress match, so that the scoreboard reflects the current state of play as the match
progresses.

**Why this priority**: This is the second of the brief's four required core operations. It is the
only way a match's initial 0-0 score (established by `001-start-match`) can ever change, and every
later capability that reports live standings by score (`004-live-summary`) depends on scores
actually being updatable.

**Independent Test**: Start a match (0-0), then update its score to a new home/away score pair
where each new value is greater than or equal to that team's current recorded score; verify via
`GetMatch` that the recorded scores now reflect the update, with every other recorded attribute of
the match unchanged. Attempting to set either team's score to a value lower than its current
recorded value is rejected, and the match's previously recorded score is left completely
unchanged.

**Acceptance Scenarios**:

1. **Given** an in-progress match with score 0-0, **When** the score is updated to 2-1, **Then**
   the match's recorded home score is 2 and away score is 1.
2. **Given** an in-progress match with score 2-1, **When** the score is updated again to 3-1,
   **Then** the match's recorded home score is 3 and its away score remains 1 (one team's score
   may increase while the other's stays the same).
3. **Given** an in-progress match with score 2-1, **When** an update is attempted setting the home
   score to 1 (lower than its current value of 2), **Then** the update does not succeed and the
   match's recorded score remains 2-1 for both teams.
4. **Given** an in-progress match with score 2-1, **When** an update is attempted setting the away
   score to a negative number, **Then** the update does not succeed and the match's recorded score
   remains 2-1 for both teams.
5. **Given** an in-progress match with score 2-1, **When** an update is attempted setting either
   team's new score to a value containing letters or special characters (e.g. `"two"`, `"2-1"`,
   `"2.5"`, `"2!"`), **Then** the update does not succeed and the match's recorded score remains
   2-1 for both teams.
6. **Given** a match ID that does not correspond to any existing match, **When** a score update is
   attempted, **Then** the operation raises an error and no match's score is changed.

---

### Edge Cases

- What happens when a decrease is attempted for only one of the two teams while the other team's
  new score is valid? → The entire update is rejected — no partial update where one team's score
  changes and the other's doesn't, per the attempted call.
- What happens when both new scores exactly equal the current recorded scores (no actual change)?
  → Accepted — "never down" forbids a decrease, it does not require a strict increase.
- What happens when a score update targets a match that has already finished? → Rejected with an
  error, same as a nonexistent match ID (see Assumptions — `MatchStatus.Finished` does not exist
  yet; this branch of the rule is specified now but not exercisable by an automated test until
  `003-finish-match` introduces that status).
- What happens when a score value contains letters, special characters, decimals, or is
  otherwise not a whole non-negative number? → Rejected (FR-002), same all-or-nothing handling
  as a negative number (FR-004) — the match's recorded score is left completely unchanged.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow updating the recorded score of an in-progress match, given its
  match ID and new score values for both the home team and the away team together.
- **FR-002**: System MUST reject a score update where either team's new score is not a valid
  non-negative integer — this includes negative numbers, letters, and special characters (any
  value that isn't a whole number ≥ 0).
- **FR-003**: System MUST reject a score update where either team's new score is lower than that
  team's current recorded score. A new score equal to the current value is accepted (monotonic
  non-decrease, not strict increase).
- **FR-004**: A rejected score update (FR-002 or FR-003) MUST leave the match's previously
  recorded score completely unchanged for both teams — the update is all-or-nothing, never
  partial.
- **FR-005**: System MUST raise an error when a score update is attempted against a match ID that
  does not correspond to any existing match, or that corresponds to a match that is no longer
  in-progress.
- **FR-006**: Updating a match's score MUST NOT change any of its other recorded attributes (match
  ID, teams, scheduled date/time, location, or status).
- **FR-007**: An updated score MUST be immediately visible to a subsequent retrieval of that match
  (e.g. `GetMatch` from `001-start-match`).

### Key Entities

- **Match** (established by `001-start-match`): this feature makes its two `Team` scores
  mutable, in place, subject to FR-002/FR-003. No new entity is introduced.
- **Team** (established by `001-start-match`): its `Score` attribute becomes mutable by this
  feature — previously only ever initialized to 0 at match start.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A caller can update an in-progress match's score and immediately retrieve the new
  values, with 100% of successful updates reflected exactly as submitted.
- **SC-002**: 100% of attempted score decreases or malformed score values (negative, non-numeric,
  or containing letters/special characters) are rejected without altering any part of the
  match's recorded score.
- **SC-003**: 100% of score updates targeting a nonexistent or non-in-progress match are rejected
  with a clear error, with no side effect on any other match's data.

## Assumptions

- **Update shape**: a score update supplies new *absolute* values for both teams together in a
  single call (not a per-team partial update, and not a delta/increment) — consistent with
  CLAUDE.md's project-wide "absolute values, not deltas" decision and with the brief's worked
  example, which lists whole match scores rather than score changes.
- **"Never down" means non-decrease, not strict increase**: a new score equal to the current
  value is accepted; only an actual decrease is rejected.
- **Divergence from CLAUDE.md's original Confirmed Decisions, now reconciled**: CLAUDE.md
  originally stated score validation was "Not enforcing monotonic non-decrease — keeps the
  library simple." This spec's explicit brief ("Score can only go up and never down") supersedes
  that specifically for `002-update-score`'s behavior; CLAUDE.md's Confirmed Decisions section
  has since been updated (2026-08-03) to state monotonic non-decrease IS enforced, so the two
  documents no longer contradict each other.
- **Rejection is via a raised error, not a non-throwing result**: this follows CLAUDE.md's
  general convention ("operating on a non-existent or already-finished match throws") rather than
  `001-start-match`'s own specific choice of a non-throwing `null` result for `StartMatch`/
  `GetMatch` — that carve-out was scoped to `001-start-match`'s own clarification session, not the
  whole project, as `001-start-match`'s spec itself noted when raising the question for later
  specs to settle.
- **Already-finished matches**: rejecting a score update against a finished match is specified
  now (FR-005) even though `MatchStatus.Finished` doesn't exist until `003-finish-match` lands —
  so `003-finish-match` doesn't need to re-derive this rule. Only the nonexistent-match-ID branch
  of FR-005 is testable by this feature's own automated tests.
- **Rejecting letters/special characters (FR-002) given the confirmed C# stack**: the library's
  public contract types each new score parameter as a non-negative integer, so passing letters
  or special characters is a compile-time type error for a caller writing C# directly against
  this library — there is no runtime string-parsing path to test at *this* layer. The
  requirement is still stated here (technology-agnostically) because it constrains the contract
  itself (score parameters must be integers, not strings) and because a non-C# caller one layer
  up — e.g. `006-scoreboard-api`, which will accept raw HTTP/JSON input — must perform this exact
  rejection at its own boundary; this spec's FR-002 is the rule that boundary needs to honor.
- This feature covers only *updating* a score on an already-started, in-progress match. Starting
  a match (`001-start-match`), finishing one (`003-finish-match`), live summaries
  (`004-live-summary`), and history (`005-match-history`) are separate specs and out of scope
  here.
