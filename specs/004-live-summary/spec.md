# Feature Specification: Live Summary

**Feature Branch**: `004-live-summary`

**Created**: 2026-08-04

**Status**: Draft

**Input**: User description: "004-live-summary. Return the matches in progress ordered by:
Total score (descending). If tied → most recently started match first. Add total score in data
model. As a match's score is updated for teams then total is updated as well. Total Score =
Team A Score + Team B Score." (plus the brief's worked example — see Acceptance Scenario 1 —
and a separately-scoped pagination request; see the note at the end of this file's Assumptions
section for why pagination isn't part of this spec.)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View live standings ordered by score (Priority: P1)

As a caller of the scoreboard library, I want to retrieve a summary of all matches currently in
progress, ordered by total score (highest first) and, among ties, by which match started most
recently, so that the most exciting/relevant live matches surface first.

**Why this priority**: This is the fourth and final of the brief's required core operations, and
the one whose worked example the brief specifies exactly — it is a literal acceptance test, not
just an example.

**Independent Test**: Start several matches in a known order, update their scores to known
values, request the summary, and verify the returned order matches total-score-descending with
most-recently-started-first on ties — including reproducing the brief's own worked example
exactly.

**Acceptance Scenarios**:

1. **Given** the following matches are started in this exact order and then updated to these
   scores — Mexico 0–Canada 5, Spain 10–Brazil 2, Germany 2–France 2, Uruguay 6–Italy 6,
   Argentina 3–Australia 1 — **When** the summary is requested, **Then** it returns them in
   exactly this order: Uruguay 6–Italy 6, Spain 10–Brazil 2, Mexico 0–Canada 5, Argentina
   3–Australia 1, Germany 2–France 2.
2. **Given** two in-progress matches with different total scores, **When** the summary is
   requested, **Then** the match with the higher total score (home + away) appears first.
3. **Given** two in-progress matches with equal total scores, **When** the summary is requested,
   **Then** the match that was started more recently appears first.
4. **Given** a match's score is updated, **When** the summary is requested afterward, **Then**
   the summary reflects the match's new total score and, if that changes its relative ranking,
   its new position.
5. **Given** no matches are currently in progress, **When** the summary is requested, **Then** it
   returns an empty result.
6. **Given** a match has been finished, **When** the summary is requested, **Then** that match
   does not appear in it (only in-progress matches are included).

---

### Edge Cases

- What happens when three or more in-progress matches are all tied on total score? → All of them
  are ordered by most-recently-started-first, extending the two-way tie-break rule to any number
  of ties.
- What happens when a match's score is updated to the same total it already had (e.g., 2-1 → 1-2)?
  → Its total score is unchanged, so its position among same-total ties depends only on start
  order, unaffected by this particular update.
- What happens to a match's summary position immediately after `001-start-match`'s `StartMatch`
  creates it at 0-0? → It appears immediately with total score 0, ranked among any other
  currently-0 matches by start order, and below every match with a positive total.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a way to retrieve a summary containing every currently
  in-progress match — no finished match may appear in it.
- **FR-002**: The summary MUST order matches by total score — the sum of the home team's and
  away team's current scores — descending (highest total first).
- **FR-003**: When two or more in-progress matches in the summary have equal total scores, they
  MUST be ordered by most-recently-started first among themselves.
- **FR-004**: System MUST track each match's total score as an attribute equal to its home
  team's score plus its away team's score, correct from the moment the match is created (0 at
  0-0) and kept correct immediately after any score update (`002-update-score`) — the summary's
  ordering (FR-002) always reflects each match's latest scores.
- **FR-005**: Requesting the summary MUST NOT change any match's data — it is a read-only
  operation.
- **FR-006**: The brief's worked example (Acceptance Scenario 1) MUST produce that exact order,
  every time.

### Key Entities

- **Match** (established by `001-start-match`): gains a derived/tracked **Total Score**
  attribute — home team's score plus away team's score — used only for ordering the summary. No
  other change to `Match`'s shape.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Given the brief's exact worked example, the summary's returned order matches the
  brief's exact expected order, 100% of the time.
- **SC-002**: A caller can retrieve a correctly-ordered summary of all in-progress matches at any
  time, and it always reflects the most recent score update made before the request.
- **SC-003**: 100% of finished matches are absent from every summary request.
- **SC-004**: 100% of tied-total-score matches are ordered by most-recently-started-first among
  themselves, regardless of how many matches share that total.

## Assumptions

- **Total score is home + away, tracked, not separately settable**: the user's own wording
  ("Add total score in data model... Total Score = Team A Score + Team B Score") is treated as a
  request to track this value as part of `Match`'s data (Key Entities above), not as a value a
  caller can set independently — it is always derived from the two team scores and has no
  validation rules of its own beyond what `002-update-score` already enforces on those two
  scores. Whether it's stored as a persisted column or computed on read is a plan-level decision,
  not fixed here (spec.md stays behavior-level).
- **Tie-break reuses `001-start-match`'s existing "start order"**: "most recently started" is
  the same monotonic sequence/Id-based concept `001-start-match`'s research.md already commits
  to (not wall-clock `DateTime`), reused here rather than redecided.
- **Read-only, no new validation surface**: this feature only reads existing `Match` data in a
  new order; it introduces no new rejection/error case (FR-005).
- **Finished matches excluded, per `003-finish-match`'s own Assumptions**: that spec already
  states finished matches "stop appearing" in this feature's summary — FR-001/SC-003 make that
  explicit here rather than only in `003`.
- **Pagination is intentionally excluded from this spec — flagged, not silently dropped.** The
  user's request also included "a functionality to see all the matches saved in db through
  pagination of 10 entries on a single page... separate from live summary... most recent match
  created or updated first." The user's own wording already calls this out as separate from live
  summary, and it is a genuinely distinct, independently-testable capability (browsing *all*
  matches including finished ones, ordered by recency, paged) — bundling it into this spec would
  violate constitution Principle III (Single-Concern Features), and the `/speckit-specify`
  process itself only creates one feature per invocation. It is also not yet a numbered slot on
  CLAUDE.md's Roadmap. This spec deliberately covers only the live summary; the pagination
  request needs its own `/speckit-specify` run and a Roadmap decision (see the completion
  report/chat for options).
