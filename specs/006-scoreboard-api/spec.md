# Feature Specification: Scoreboard API

**Feature Branch**: `006-scoreboard-api`

**Created**: 2026-08-04

**Status**: Draft

**Input**: User description: "006-scoreboard-api. Create a minimal .net web api project that
uses that library and provide apis for all methods match creation, score update etc. If any
method is missing then ask. Add unit tests for api as well and test while completing it. Add a
docker file and swagger as well. It should be possible to use swagger to test the api. Return
appropriate status code and response where applicable."

## User Scenarios & Testing *(mandatory)*

<!--
  This feature exposes 001-start-match through 005-match-history's already-implemented library
  over HTTP — a thin transport adapter per constitution Principle IV, with no business logic of
  its own. Each user story below mirrors one of those five library features' priority order,
  so the API's own MVP shape matches the library's.
-->

### User Story 1 - Start and retrieve a match over HTTP (Priority: P1)

As a caller of the API (e.g., a future frontend, or a developer exploring via Swagger), I want
to start a new match and retrieve a match's details over HTTP, so that the library's foundational
capability is usable without writing a .NET client.

**Why this priority**: Mirrors `001-start-match`'s own priority — every other endpoint operates
on a match that must first exist and be retrievable.

**Independent Test**: Using Swagger (or any HTTP client), call the start-match endpoint with
valid inputs and confirm a `201 Created`-style response containing the new match's details;
call the get-match endpoint with that match's ID and confirm the same details come back; call
it with an unknown ID and confirm a not-found response.

**Acceptance Scenarios**:

1. **Given** valid team names, a location, and a scheduled date/time, **When** the start-match
   endpoint is called, **Then** the response indicates success and its body contains the new
   match's ID, teams, score (0-0), scheduled date/time, and location.
2. **Given** inputs that the library's `StartMatch` would reject (missing/duplicate team names,
   missing location, a team already in-progress elsewhere, or a location/time already booked),
   **When** the start-match endpoint is called, **Then** the response indicates a client error,
   with a body describing that the request was rejected.
3. **Given** a match that was already started, **When** the get-match endpoint is called with
   its ID, **Then** the response indicates success and its body matches the match's current
   recorded data.
4. **Given** a match ID that does not correspond to any match, **When** the get-match endpoint
   is called, **Then** the response indicates the resource was not found.

---

### User Story 2 - Update a match's score over HTTP (Priority: P2)

As a caller of the API, I want to update a match's score over HTTP, so that live score changes
made by the library are reachable without a .NET client.

**Why this priority**: Mirrors `002-update-score`'s own priority — the second core operation.

**Independent Test**: Start a match via the API, call the update-score endpoint with a higher
score for each team, and confirm the response reflects the new score; attempt a decrease or a
negative value and confirm a client-error response with the score left unchanged.

**Acceptance Scenarios**:

1. **Given** an in-progress match, **When** the update-score endpoint is called with new scores
   each greater than or equal to the current ones, **Then** the response indicates success and
   its body reflects the new scores.
2. **Given** an in-progress match, **When** the update-score endpoint is called with a score
   lower than the team's current score, or a negative score, **Then** the response indicates a
   client error, with a body describing why, and the match's score is unchanged.
3. **Given** a match ID that does not correspond to an in-progress match (nonexistent or already
   finished), **When** the update-score endpoint is called, **Then** the response indicates the
   resource was not found.

---

### User Story 3 - Finish a match over HTTP (Priority: P3)

As a caller of the API, I want to mark a match as finished over HTTP, so that the library's
one-way finish transition is reachable without a .NET client.

**Why this priority**: Mirrors `003-finish-match`'s own priority — the third core operation.

**Independent Test**: Start a match via the API, call the finish endpoint, and confirm the
response shows it as finished; call finish again on the same match and confirm a not-found
response; call the get-match endpoint afterward and confirm the match's data is still present.

**Acceptance Scenarios**:

1. **Given** an in-progress match, **When** the finish endpoint is called with its ID, **Then**
   the response indicates success and its body shows the match's status as finished.
2. **Given** a match that is already finished, or a match ID that does not exist, **When** the
   finish endpoint is called, **Then** the response indicates the resource was not found.

---

### User Story 4 - View the live summary over HTTP (Priority: P4)

As a caller of the API, I want to retrieve the live summary of in-progress matches over HTTP,
so that the ordering the library already guarantees (`004-live-summary`) is reachable without a
.NET client.

**Why this priority**: Mirrors `004-live-summary`'s own priority.

**Independent Test**: Start and update several matches via the API, call the summary endpoint,
and confirm the returned order matches total-score-descending with most-recently-started-first
on ties — the same guarantee `004-live-summary` already provides.

**Acceptance Scenarios**:

1. **Given** several in-progress matches with different scores, **When** the summary endpoint
   is called, **Then** the response body lists them in the same order the library's `GetSummary`
   would produce.
2. **Given** no matches are currently in progress, **When** the summary endpoint is called,
   **Then** the response indicates success with an empty list — not an error.

---

### User Story 5 - Browse match history over HTTP (Priority: P5)

As a caller of the API, I want to browse paginated match history over HTTP, so that the
library's chosen extra feature (`005-match-history`) is reachable without a .NET client.

**Why this priority**: Mirrors `005-match-history`'s own priority — the last of the library's
operations to be exposed.

**Independent Test**: Start more matches via the API than fit on one page, call the history
endpoint for page 1 and page 2, and confirm the results match the library's `GetHistory`
pagination and ordering; call it with an invalid page number and confirm a client-error
response.

**Acceptance Scenarios**:

1. **Given** more matches exist than fit on one page, **When** the history endpoint is called
   with a page number, **Then** the response body contains that page's matches, ordered by most
   recent activity, matching the library's `GetHistory`.
2. **Given** a page number beyond the available data, **When** the history endpoint is called,
   **Then** the response indicates success with an empty list — not an error.
3. **Given** a page number less than 1, **When** the history endpoint is called, **Then** the
   response indicates a client error.

---

### Edge Cases

- What happens when the underlying library rejects a request for a reason the API can't
  distinguish from other rejection reasons (e.g., `StartMatch`'s single non-throwing `null` for
  every kind of conflict)? → The API returns one generic client-error response for all of that
  operation's rejection reasons — it does not guess a more specific reason than the library
  itself exposes.
- What happens if a request's inputs can't even be parsed (e.g., a non-numeric match ID or
  score in the URL/body)? → The API rejects it as a client error before ever calling the
  library, the same way `002-update-score`'s CLI demo already rejects non-numeric input before
  it reaches the library.
- What happens when the API is queried for documentation/exploration rather than a business
  operation? → The interactive documentation (FR-009) itself must be reachable and must not
  require the caller to already know each endpoint's shape in advance.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST expose an HTTP endpoint to start a new match, accepting the same
  inputs as the library's `StartMatch` (home team, away team, scheduled date/time, location),
  returning the created match's details on success.
- **FR-002**: The start-match endpoint MUST respond with a single, generic client-error status
  and a descriptive body when the underlying operation is rejected for any reason (invalid
  input, team conflict, or location/time conflict) — the API MUST NOT invent a more specific
  status per rejection reason than the library itself distinguishes.
- **FR-003**: System MUST expose an HTTP endpoint to retrieve a single match by its ID,
  responding with a not-found status when no such match exists.
- **FR-004**: System MUST expose an HTTP endpoint to update a match's score, returning the
  updated match on success, a not-found status when the match doesn't exist or isn't
  in-progress, and a client-error status with a descriptive reason when the requested score is
  invalid (negative or a decrease).
- **FR-005**: System MUST expose an HTTP endpoint to finish a match, returning the finished
  match on success and a not-found status when the match doesn't exist or is already finished.
- **FR-006**: System MUST expose an HTTP endpoint to retrieve the live summary of in-progress
  matches, in exactly the order the library's `GetSummary` already produces.
- **FR-007**: System MUST expose an HTTP endpoint to retrieve a page of match history, accepting
  a page number, returning that page's matches, an empty list for an out-of-range page, and a
  client-error status for a page number less than 1.
- **FR-008**: Every endpoint MUST return a structured (e.g. JSON) response body for both success
  and rejection cases — never a bare status code with no body. Every rejection (4xx) response
  body MUST include an `error_code` property (a short, stable, machine-readable identifier for
  the specific rejection reason) and an `error_message` property (a human-readable description)
  — not just a generic status code with no distinguishing detail. The error codes this feature
  produces are:

  | `error_code` | Produced by |
  |---|---|
  | `start_rejected` | Start-match endpoint, any rejection reason (FR-002 — the library itself doesn't distinguish which) |
  | `match_not_found` | Get-match (FR-003), update-score (FR-004), and finish (FR-005) endpoints, whenever the referenced match doesn't exist or isn't in-progress |
  | `invalid_score` | Update-score endpoint, when the requested score is negative or a decrease (FR-004) |
  | `invalid_page` | History endpoint, when the requested page number is less than 1 (FR-007) |
- **FR-009**: The API MUST expose interactive, browsable API documentation that allows a caller
  to invoke every endpoint directly, without writing a separate HTTP client.
- **FR-010**: The API MUST be packaged so it can be built and run as a container image, startable
  with a single container-run command, with no manual dependency installation on the host beyond
  a container runtime.
- **FR-011**: Every endpoint MUST have automated test coverage exercising at least one success
  case and, where the underlying library operation can reject, at least one rejection case.

### Key Entities

- **Match** (established by `001-start-match`): the API returns the same data the library's
  `Match` already carries (ID, teams and their scores, scheduled date/time, location, status)
  — this feature introduces no new entity, only an HTTP-reachable view of the existing one.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A caller can exercise all six of the library's operations (start, get, update
  score, finish, live summary, history) via HTTP using only the interactive documentation — no
  custom client code required.
- **SC-002**: 100% of endpoints return a status code and body — including the correct
  `error_code`/`error_message` pair for rejections (FR-008's table) — that accurately reflects
  success or the specific rejection reason available from the underlying library operation.
- **SC-003**: The API can be built and started via a single container command, with no manual
  setup step on the host beyond having a container runtime installed.
- **SC-004**: 100% of endpoints have automated tests covering at least one success case and, for
  every endpoint whose underlying operation can reject, at least one rejection case.

## Assumptions

- **No library methods are missing.** `IScoreboard`'s current contract has exactly six methods
  (`StartMatch`, `GetMatch`, `UpdateScore`, `FinishMatch`, `GetSummary`, `GetHistory`), and every
  one maps cleanly to exactly one endpoint (FR-001, FR-003 through FR-007). The user's own
  instruction said to ask if anything were missing — nothing is; this is stated explicitly
  rather than silently assumed.
- **"Minimal .NET Web API project"** is read as a request for a small, single-purpose API
  project with minimal footprint (constitution Principle IV: "a thin transport adapter with no
  business logic of its own") — not necessarily a mandate for one specific ASP.NET Core project
  template over another (e.g., Minimal API endpoints vs. Controllers). The exact ASP.NET Core
  style is a `/speckit-plan`-level technical decision, not fixed here.
- **Status code and `error_code` granularity is bounded by what the library already
  distinguishes.** Where the library exposes a specific reason via a typed exception
  (`MatchNotFoundException`, `InvalidScoreException`, `InvalidPageException`), the API maps it
  to a specific status and `error_code` (FR-008's table). Where the library only returns a
  generic non-throwing `null` (`StartMatch`), the API returns one generic `error_code`
  (`start_rejected`) — introducing finer-grained rejection reasons would require changing the
  library itself, which is out of scope for an API-only feature.
- **How the endpoint-to-response mapping is implemented in code (e.g. a discriminated-union
  library such as `OneOf`, representing each endpoint's outcome as "success or one of N named
  failure cases" instead of nullable returns/exception catching) is a `/speckit-plan`-level
  technical decision** — spec.md fixes the *observable* contract (FR-008's `error_code`/
  `error_message` shape and table), not the C# pattern used to produce it.
- **No authentication/authorization is introduced.** Consistent with this project's kata-scale
  scope and the library's own no-auth design; this endpoint set has no concept of a caller
  identity to check.
- **Automated tests exercise real HTTP endpoints, not the library directly** — per constitution
  Principle IV ("API via integration tests against real HTTP endpoints"). This feature's own
  test coverage is about the HTTP layer's request/response/status-code mapping; the library's
  own business-logic tests (from `001`-`005`) are not re-derived here.
- **This is one Spec-Kit feature covering the whole API layer**, not five separate specs
  mirroring `001`-`005`'s own per-operation split. CLAUDE.md's Roadmap allocates a single entry
  (`006-scoreboard-api`) to this whole phase; internal decomposition happens at the User Story
  level within this one feature instead. A finer split is possible if requested, but isn't
  assumed here.
- **No health-check/readiness endpoint is introduced** — not requested, and out of scope unless
  asked for separately.
- **Docker/Swagger are per this feature's explicit request, not the library's Phase 1 scope** —
  consistent with CLAUDE.md's framing of Phase 2 (API) as an intentional expansion beyond the
  brief's original ask, already documented as such.
