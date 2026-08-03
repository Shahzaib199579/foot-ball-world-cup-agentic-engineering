# Feature Specification: Start New Match

**Feature Branch**: `001-start-match`

**Created**: 2026-08-03

**Status**: Draft

**Input**: User description: "start new match between two teams which represents their countries in foot-ball with score 0 - 0. Match should be class and the score will be tracked in it. It should also have members that hold class team with members name, score to track the team and their scores. It will track score for both teams. It should also have proprties to save the date and time a match is scheduled, Location of where the match is happening and methods to manipulate or return this data as needed. This should be designed keeping in mind that this library could be used by an api that could set a match and start it between 2 teams and returns if successful in scheduling a match. A match can't be started in the same location and same time. Ask me anything else you need clarification on."

## Clarifications

### Session 2026-08-03

- Q: Should the "no two matches at the same location and time" rule only block against other currently in-progress matches, or against every match ever created (including finished ones)? → A: In-progress matches only — once a match finishes, its location/time slot is free to reuse.
- Q: How should a specific match be identified when it's retrieved later, or when future features (update score, finish) need to target it? → A: System-generated unique match ID, assigned when the match is started.
- Q: When starting a match is rejected (e.g., a conflict), how should the caller find out it failed — an exception, or a non-throwing result/return value? → A: Returns a non-throwing success/failure result value; no exception is thrown for a rejected start attempt.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Start a match between two teams (Priority: P1)

As a caller of the scoreboard library (e.g., an API built on top of it), I want to start a new
match between two teams for a given date/time and location, so that the match becomes trackable
with an initial score of 0-0.

**Why this priority**: This is the foundational operation every other capability in the roadmap
depends on — updating scores, finishing a match, live summaries, and history all require a
started match to exist first. It establishes the Match/Team data model used throughout.

**Independent Test**: Can be fully tested by starting a match between two distinct team names for
a given date/time and location, and verifying a new match exists with score 0-0 for both teams.

**Acceptance Scenarios**:

1. **Given** no existing matches, **When** a match is started between Team A and Team B for a
   given date/time and location, **Then** the match exists with score 0-0 for both teams.
2. **Given** Team A is already part of another in-progress match, **When** starting a new match
   that also includes Team A, **Then** the operation does not succeed and no new match is created.
3. **Given** an in-progress match already exists at Location X for date/time T, **When** starting
   another match also at Location X for date/time T, **Then** the operation does not succeed and
   no new match is created.
4. **Given** a match at Location X for date/time T has already finished, **When** starting a new
   match also at Location X for date/time T, **Then** the operation succeeds.

---

### User Story 2 - Retrieve a started match's details (Priority: P2)

As a caller of the library, I want to retrieve a started match's recorded details — its teams,
current score, scheduled date/time, and location — so this information can be displayed or acted
on elsewhere.

**Why this priority**: Secondary to starting a match, but the data captured in User Story 1 is
only useful if it can be read back. Independently testable and deliverable on its own once a
match exists.

**Independent Test**: Start a match, then retrieve it by its assigned match ID and verify every
recorded attribute (teams, score, date/time, location) matches what was provided at start.

**Acceptance Scenarios**:

1. **Given** a match has been started, **When** its details are requested by its match ID,
   **Then** the returned data includes both team names, the current score (0-0 initially), the
   scheduled date/time, and the location.
2. **Given** no match exists with the requested match ID, **When** its details are requested,
   **Then** the operation does not succeed.

---

### Edge Cases

- What happens when the same team name is supplied for both sides of a match? → The match is
  rejected; a team cannot play itself.
- What happens when the team name, date/time, or location is missing/empty? → The match is
  rejected; all are required.
- What happens when two match requests share the same location but different date/times, or the
  same date/time but different locations? → Both are allowed; only an exact match on **both**
  location and date/time is a conflict.
- What happens when the provided scheduled date/time is in the past, present, or future? →
  Accepted in all cases as descriptive metadata; the match is created and activated
  (in-progress) immediately by this operation regardless — there is no separate, later
  "begin" step in this feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow starting a new match between two distinct, named teams,
  initializing the score at 0-0 for both teams, and MUST assign it a unique match ID at that
  time.
- **FR-002**: System MUST record a location for every match; location is required and non-empty.
- **FR-003**: System MUST record a scheduled date/time for every match, set when the match is
  created. This value MAY be a past, present, or future date/time and is purely descriptive
  metadata — this operation always creates the match in an active (in-progress) state
  immediately; there is no separate, later "begin a previously-scheduled match" action in this
  feature (a true two-step schedule-then-begin capability is out of scope here and would be its
  own future spec if ever needed).
- **FR-004**: System MUST reject starting a match when either team name is missing/empty, or when
  both team names are identical.
- **FR-005**: System MUST reject starting a match if either team is already participating in
  another in-progress match (existing project rule, reused here).
- **FR-006**: System MUST reject starting a match if another **in-progress** match already
  exists at the exact same location and the exact same scheduled date/time instant. No
  overlapping-window or duration concept is introduced — equality is by exact instant only.
  Once a match finishes, its location/time combination becomes available again for a new
  match.
- **FR-007**: System MUST make a started match's recorded data (teams, score, scheduled date/time,
  location) retrievable by its match ID after it has been started.
- **FR-008**: When a start attempt is rejected (FR-004, FR-005, or FR-006), the system MUST
  communicate the failure to the caller via a non-throwing success/failure result rather than
  raising an exception, and MUST NOT create a match in that case.

### Key Entities

- **Match**: A single football match between two teams. Attributes: a unique match ID (assigned
  at start), the two participating teams, the running score for each, scheduled date/time, and
  location. Created with score 0-0.
- **Team**: One side in a match. Attributes: name (the country/team identifier) and its current
  score within that match.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A caller can start a new match given two team names, a date/time, and a location,
  and immediately retrieve it by its match ID with score 0-0.
- **SC-002**: 100% of conflicting start attempts (a team already in an in-progress match, or a
  location/time already booked) are rejected without creating a second match.
- **SC-003**: Every successfully started match's recorded details (teams, score, date/time,
  location) can be retrieved exactly as provided at start, with no data loss.

## Assumptions

- Team names follow the project's established validation rule: non-null, non-empty strings.
- The two teams in a match must differ from one another — a team cannot play itself.
- Location is a simple required text identifier (e.g., venue or city name) — no structured venue
  registry or geocoding is assumed at this stage.
- The existing project rule that a team cannot be in more than one in-progress match at a time
  still applies, in addition to the new location/date-time conflict rule introduced here.
- This feature covers only *starting* a match (Roadmap spec 001). Updating scores, finishing a
  match, live summaries, and history are separate specs (002-005) and out of scope here.
- Resolved scope decision: this feature stays single-purpose per the constitution's
  Single-Concern Features principle. "Scheduling" and "starting" are not split into two
  operations here — starting a match always both records its (possibly future-dated)
  scheduled date/time and activates it in one call. A genuine two-step schedule-then-begin
  workflow was considered and deliberately deferred to a possible future spec, not added to
  the current Roadmap.
- Noted divergence: CLAUDE.md's project-wide Confirmed Decisions state that operating on a
  non-existent or already-finished match "throws." This spec's rejected-start behavior
  (FR-008) is a non-throwing result instead, specific to the *start* operation per this
  session's clarification. Later specs (002-update-score, 003-finish-match) should confirm
  whether they follow the same non-throwing convention or the original throwing one — not
  assumed to be settled by this spec alone.
