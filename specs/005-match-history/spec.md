# Feature Specification: Match History

**Feature Branch**: `005-match-history`

**Created**: 2026-08-04

**Status**: Draft

**Input**: User description: derived from CLAUDE.md's Confirmed Decisions — this project's
chosen "additional operation of choice" (per the brief), reconciled this session to fold in a
separately-requested pagination capability: `GetHistory()` returns every match ever started
(in-progress *and* finished), paginated at 10 entries per page, ordered by most recently
created-or-updated first.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Browse the full match history, a page at a time (Priority: P1)

As a caller of the scoreboard library, I want to retrieve matches — both in-progress and
finished — in pages of at most 10, most-recently active first, so that I can browse the
complete history of matches without loading the entire dataset at once.

**Why this priority**: This is the project's chosen extra feature (beyond the brief's four
required core operations). It fits a "Data & Odds Platform" framing — historical results have
standalone value beyond the live board — and pagination is what makes browsing that history
practical rather than dumping every match at once.

**Independent Test**: Create more matches than fit on one page, update and finish some of them,
request page 1 and verify it contains exactly the 10 most-recently-active matches in the right
order; request further pages and verify they continue correctly, with an out-of-range page
returning an empty result rather than an error.

**Acceptance Scenarios**:

1. **Given** more than 10 matches exist (a mix of in-progress and finished), **When** history
   page 1 is requested, **Then** exactly the 10 most-recently created-or-updated matches are
   returned, in that order.
2. **Given** exactly 15 matches exist, **When** history page 2 is requested, **Then** the next 5
   most-recently created-or-updated matches (ranked 11-15) are returned.
3. **Given** a match is updated (a score change, or being finished) some time after it was
   created, **When** history is requested afterward, **Then** that match is ranked by the time
   of its most recent update, not its original creation time.
4. **Given** fewer matches exist than fit on one page, **When** history page 1 is requested,
   **Then** all of them are returned, and requesting page 2 returns an empty result.
5. **Given** a match has been finished, **When** history is requested, **Then** it still appears
   in the results — unlike the live summary (`004-live-summary`), finished matches are never
   excluded here.
6. **Given** no matches exist at all, **When** history page 1 is requested, **Then** an empty
   result is returned.

---

### Edge Cases

- What happens when a page number beyond the available data is requested (e.g., page 100 when
  only 2 pages' worth of matches exist)? → An empty result, not an error — same treatment as
  "no matches at all."
- What happens when a page number less than 1 is requested? → Rejected — pages are 1-based, and
  there is no reasonable data for page 0 or a negative page number.
- What happens if a match is created and then updated multiple times before history is
  requested? → Only its most recent activity (whichever happened last: creation, a score
  update, or being finished) determines its rank; no history of its intermediate states is kept
  — this feature reflects current match data, not a change log.
- What happens if new matches are started or existing ones updated between two separate history
  requests? → Each request reflects the live, current match set at the moment it's made — page
  contents can shift between requests, the same way `004-live-summary`'s results already do.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a way to retrieve matches — both in-progress and finished —
  in pages of exactly 10 entries per page.
- **FR-002**: Results MUST be ordered by most recent activity first, where "activity" means the
  match being created, having its score updated, or being finished — whichever happened most
  recently for that match.
- **FR-003**: System MUST support requesting a specific page number, returning only that page's
  matches.
- **FR-004**: Requesting a page beyond the available data MUST return an empty result, not an
  error.
- **FR-005**: System MUST reject a request for a page number less than 1.
- **FR-006**: Retrieving match history MUST NOT change any match's data — it is a read-only
  operation.
- **FR-007**: Every match ever started MUST eventually be retrievable via this history,
  regardless of its current status — none are permanently excluded.

### Key Entities

- **Match** (established by `001-start-match`): gains a tracked "last activity" marker, updated
  whenever the match is created, has its score updated (`002-update-score`), or is finished
  (`003-finish-match`). Used only for ordering history results — no other change to `Match`'s
  shape.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A caller can retrieve any page of match history and receive exactly the matches
  ranked in that page's range by most-recent activity, up to 10 per page.
- **SC-002**: 100% of matches — in-progress and finished — eventually appear somewhere in the
  history; none are permanently hidden.
- **SC-003**: 100% of matches whose score was updated or that were finished after creation are
  re-ranked by that most-recent activity, not by their original creation order.
- **SC-004**: 100% of out-of-range page requests return an empty result rather than an error.

## Assumptions

- **Page size is fixed at 10, not configurable** — per explicit instruction (CLAUDE.md's
  Confirmed Decisions). Pages are 1-based (page 1 is the most-recent page).
- **"Most recent activity" is tracked via a monotonic sequence counter, not wall-clock
  `DateTime`** — consistent with `001-start-match`'s existing rationale for `Match.Id` (avoids
  timestamp-resolution ambiguity, keeps tests deterministic) and reused by `004-live-summary`'s
  own tie-break. This is a *new* tracked value, separate from `Id` (which reflects only
  creation order, not later activity).
- **`GetHistory` returns only the requested page's matches — no total-count/total-pages
  metadata wrapper.** Callers can infer "more pages likely exist" from receiving a full page of
  10. A richer pagination-metadata type is deferred as speculative design ahead of an actual
  need, consistent with this project's established pattern (e.g. `001-start-match`'s
  `IScoreboard` growth strategy).
- **No filtering by status/team/date** — this feature returns the complete, unfiltered history,
  page by page. A searchable/filterable history is a distinct, larger capability and out of
  scope unless requested separately.
- **Unlike `003-finish-match`/`004-live-summary`, this feature is expected to touch
  `StartMatch`/`UpdateScore`/`FinishMatch`'s existing implementations** — none of them currently
  track any "last activity" marker, so (unlike those two prior features, which needed zero or
  near-zero changes to existing methods) this one genuinely needs to add the tracking write at
  each of those three call sites. Noted here so `/speckit-plan` doesn't underestimate scope by
  assuming the same "already-defensive code" pattern applies again.
- **This is the project's chosen extra feature** (per the brief's "exactly one additional
  operation" requirement) — the pagination capability requested alongside `004-live-summary`
  was explicitly folded in here per the user's own decision (see that spec's Assumptions for why
  it wasn't bundled there instead). Per the brief and CLAUDE.md's Working Conventions, this
  feature's commit(s) must be distinct from any other feature's, and its rationale must be
  documented in README.md.
