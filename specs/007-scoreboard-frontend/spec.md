# Feature Specification: Scoreboard Frontend

**Feature Branch**: `007-scoreboard-frontend`

**Created**: 2026-08-04

**Status**: Draft

**Input**: User description: "007-scoreboard-frontend. Create a separate angular application that will call the api for all the features implemented. Both services could be run through docker compose file. For front end, use material design theme, white and blue color pallet. The Dashboard should look professional and sleek. It should have a left side nav for History and Summary. For both, each country should shown as a separate card against each other in single row with "VS" in between. Each card would have the flag of country and their name then after a space the score. When we need to start a new match, then through side nav, there should be a separate tab for matches and inside that we would select country with flag and name through a drop down for left country and for right and a button to start the match. Then we can update the score there and finish the match. If we switch to History or summary then latest match we just started, or score updated or finished should be loaded. If we receive an error from the backend for cases like assigning same country to different matches then a modal or pop up should appear showing error and errors should be handled and shown in professional manner. Make sure that there are playwright test to test all the test cases that api handles from the front-end side. For anything missing, ask me."

## Clarifications

### Session 2026-08-04

- Q: Should the Playwright E2E test suite (User Story 5 / FR-009), carried over from an earlier
  session's fuller prompt, remain in scope even though the latest `/speckit-specify` re-run
  didn't repeat it? → A: Yes — keep it in scope; implement the Playwright work.
- Q: User Story 3's acceptance scenarios (update score, finish match) aren't backed by a
  dedicated top-level Functional Requirement — is there a decision to make here, or is this
  just a documentation gap? → A: Documentation gap, no decision needed — add an FR mirroring
  the existing acceptance scenarios (see FR-010 below).
- Q: For the success confirmations on starting a match, updating a score, and finishing a
  match, should this use a blocking modal dialog (like the existing error dialog, requires a
  dismiss click) or a lighter auto-dismissing toast/snackbar? → A: Modal dialog — same
  `MatDialog` family as the error path (see FR-011 below).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Live Summary and Paginated Match History (Priority: P1)

As a user visiting the World Cup Scoreboard dashboard, I want to see a sleek, professional interface with a left side navigation bar to switch between Live Summary, Match History, and Match Management, so that I can view current live matches and past historical matches presented in side-by-side country cards.

**Why this priority**: Core presentation layout required for all user interactions.

**Independent Test**: Navigate to the dashboard; switch between "Summary" and "History" tabs via the left side navigation; observe the custom match cards displaying home/away country flags, country names, scores, and a central "VS" separator.

**Acceptance Scenarios**:
1. **Given** the user lands on the application, **When** the dashboard opens, **Then** a left side navigation bar is present with options for "Summary", "History", and "Matches", styled in a modern Angular Material white-and-blue design theme.
2. **Given** in-progress matches exist in the system, **When** the user views the "Summary" tab, **Then** each match is displayed as a single row containing a Home Country Card and an Away Country Card with a "VS" badge in between. Each card displays the flag icon/image, country name, and current score.
3. **Given** matches exist in history, **When** the user selects the "History" tab, **Then** historical and in-progress matches are displayed in the side-by-side card format with status indicators (In Progress / Finished) and pagination controls (10 matches per page).

---

### User Story 2 - Start a New Match from the Matches Tab (Priority: P2)

As a user in the Matches management view, I want to select home and away countries with flag dropdowns, specify location/time, and click "Start Match" to create a new live match.

**Why this priority**: Enables match creation from the frontend UI.

**Independent Test**: Go to the "Matches" tab, select "Mexico" (home) and "Canada" (away) with flag icons, provide a location, click "Start Match", and verify the match is created and visible in live summary.

**Acceptance Scenarios**:
1. **Given** the user is on the "Matches" tab, **When** selecting home and away countries from flag-enabled dropdowns and clicking "Start Match", **Then** the Angular app sends a `POST /matches` request to the backend API, and on receiving `201 Created` displays a success confirmation modal and shows the newly created match.
2. **Given** the user starts a match, **When** navigating immediately to the "Summary" or "History" tab, **Then** the freshly created match is immediately fetched and reflected in the view.

---

### User Story 3 - Update Scores and Finish Matches (Priority: P3)

As a user managing active matches in the Matches tab, I want to update home/away scores and mark matches as finished.

**Why this priority**: Core match progression management.

**Independent Test**: On an active match card in the Matches tab, update home score to 1 and away score to 2, click update, then click "Finish Match" to complete the match.

**Acceptance Scenarios**:
1. **Given** an in-progress match, **When** the user enters updated scores and submits, **Then** `PUT /matches/{id}/score` is called, and on receiving `200 OK` a success confirmation modal appears and the score updates dynamically on screen.
2. **Given** an in-progress match, **When** the user clicks "Finish Match", **Then** `POST /matches/{id}/finish` is called, and on receiving `200 OK` a success confirmation modal appears and the match status updates to Finished. Switching to Summary removes the finished match from live summary, while History shows it marked as Finished.

---

### User Story 4 - Professional Error Dialog Handling (Priority: P4)

As a user performing actions that violate business constraints (such as assigning the same country to multiple active matches or entering a lower/negative score), I want a professional Material dialog/modal to appear displaying the exact error message returned by the API.

**Why this priority**: Essential for robust UX and error recovery.

**Independent Test**: Attempt to start a match with "Mexico" while Mexico is already in an active match. Confirm an Angular Material error modal pops up displaying the backend error details clearly with a close/dismiss action.

**Acceptance Scenarios**:
1. **Given** an invalid request (e.g. duplicate team in active match, negative score, invalid page), **When** the backend API returns a 400 Bad Request error response, **Then** an Angular Material modal dialog appears showing the error message in a sleek, readable format.

---

### User Story 5 - Full Docker Compose & Playwright End-to-End Test Suite (Priority: P5)

As a developer or evaluator, I want to run the backend API and frontend Angular app together using Docker Compose, and run Playwright E2E tests validating all frontend-to-API behaviors.

**Why this priority**: Guarantees containerized portability and automated test coverage.

**Independent Test**: Run `docker compose up` and run Playwright test suite (`npx playwright test`), confirming all scenarios (start match, invalid match start error modal, score update, finish match, live summary order, history pagination) pass green against the frontend.

**Acceptance Scenarios**:
1. **Given** `docker-compose.yml`, **When** `docker compose up` is executed, **Then** both `scoreboard-api` (port 5000) and `scoreboard-frontend` (port 4200) run cleanly and communicate.
2. **Given** the Playwright test suite, **When** executed against the frontend, **Then** all API validation edge cases and happy path flows pass.

---

### Edge Cases

- What happens if backend is unreachable? Frontend displays an error notification banner/modal indicating network connection issue.
- What happens when custom country names (outside standard list) are typed? A fallback generic country flag icon is rendered gracefully.
- What happens when rapid switching between History and Summary occurs? RxJS state management / HTTP cancellation handles stale requests cleanly without race conditions.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST be an Angular (v18+) SPA with Angular Material components, styled using a custom white and blue palette (`#003366`, `#0055a5`, `#e8f0fe`, `#ffffff`).
- **FR-002**: System MUST feature a left side navigation bar with 3 main sections: Summary, History, and Matches.
- **FR-003**: System MUST render match items in Summary and History as a single row containing two separate country cards (Home VS Away) with a centered "VS" element.
- **FR-004**: Each country card MUST display a country flag image/icon, country name, and score separated by spacing.
- **FR-005**: System MUST provide a dropdown in the Matches tab with country names and flag icons for selecting Home and Away teams.
- **FR-006**: System MUST automatically refresh / load latest match state when switching between navigation tabs (Summary, History, Matches).
- **FR-007**: System MUST intercept API errors (e.g., 400 Bad Request, 404 Not Found) and display an Angular Material Dialog error modal containing `error_message`.
- **FR-008**: System MUST include a root `docker-compose.yml` that builds and launches both .NET API and Angular frontend.
- **FR-009**: System MUST include Playwright E2E test scripts covering all backend business rules via UI interactions.
- **FR-010**: System MUST allow updating a match's home/away scores and marking a match as finished directly from the Matches tab's active-match view.
- **FR-011**: System MUST display an Angular Material modal dialog confirming success immediately after a match is started (`201 Created`), a score is updated (`200 OK`), or a match is finished (`200 OK`), based on the status code returned by the API — using the same `MatDialog` mechanism as the error path (FR-007), not a toast/snackbar.

### Key Entities

- **Match**: Frontend model (TypeScript interface `Match`) wrapping ID, HomeTeam and AwayTeam (each a nested `Team` object with a team Name and Score — mirrors `006-scoreboard-api`'s actual response shape, not flat Home/Away score fields), Status, ScheduledAt, Location, and sequence tracking.
- **CountryOption**: Country model with name and flag URL / ISO code.
- **ApiError**: Error contract representing `error_code` and `error_message`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of API features (Start, Update Score, Finish, Summary, History) accessible and functional through the Angular UI.
- **SC-002**: All Playwright E2E tests execute and pass automated execution.
- **SC-003**: `docker compose up` brings up both frontend and backend services with zero manual configuration.

## Assumptions

- Angular application hosted on port 4200 in development / container.
- Backend API hosted on port 5000 with CORS enabled for `http://localhost:4200`.
- Country flags sourced via standard SVG icons or `flagcdn.com` URLs with fallbacks for unknown countries.
