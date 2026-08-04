# Tasks: Scoreboard Frontend

**Input**: Design documents from `specs/007-scoreboard-frontend/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/frontend-api-usage.md, quickstart.md

**Revision note**: this replaces the earlier 10-broad-task version (written in a parallel
session before `plan.md` was brought into the standard template structure). Regenerated after
two `/speckit-plan` passes — the original restructure (Technical Context/Constitution Check/
Project Structure, Karma+Jasmine component tests, FR-010) and the follow-up fold-in of FR-011
(the success-confirmation modal). Organized by user story, test-first per Constitution
Principle I, mirroring `006-scoreboard-api`'s task structure and conventions.

**Tests**: Karma/Jasmine component tests + Playwright E2E tests are included per story — TDD is
mandatory here per Constitution Principle I (Test-First, NON-NEGOTIABLE) and Principle IV
("frontend via component/e2e tests"), not merely optional per the template's default.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on an incomplete task)
- **[Story]**: US1-US5, mapping to spec.md's 5 user stories (priority order P1→P5)
- All paths are relative to `src/WorldCupScoreboard.Frontend/` unless noted otherwise

---

## Phase 1: Setup

**Purpose**: Angular workspace, Material, and Playwright tooling exist and build cleanly

- [X] T001 Create the Angular 18+ standalone workspace at `src/WorldCupScoreboard.Frontend/`
  (`ng new WorldCupScoreboard.Frontend --standalone --style=scss --routing`), sibling to the
  existing `src/WorldCupScoreboard/` and `src/WorldCupScoreboard.Api/` projects (plan.md
  Project Structure).
- [X] T002 [P] Add Angular Material (`ng add @angular/material`) and define the custom
  white/blue theme (`#003366`, `#0055a5`, `#e8f0fe`, `#ffffff`) in `src/styles.scss` (FR-001,
  research.md §2). Depends on T001.
- [X] T003 [P] Install Playwright as a dev dependency and scaffold
  `e2e/playwright.config.ts` (research.md §8) — config only, test files are added per story
  below. Depends on T001.
- [X] T004 [P] Configure Angular CLI's default linting (`ng lint`) for the new project.
  Depends on T001.

**Checkpoint**: `ng build` succeeds on an empty Material-themed shell; Playwright can run
(against zero tests) and lint passes.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared models, the `ScoreboardService` API client, and the side-nav app shell —
none of this is story-specific; every user story below depends on it

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T005 [P] Create the `Match`/`Team` interfaces in `core/models/match.model.ts`
  (data-model.md).
- [X] T006 [P] Create `CountryOption` model + the static bundled country/flag list in
  `core/models/country.model.ts` (research.md §6).
- [X] T007 [P] Create `ApiError` in `core/models/api-error.model.ts` (data-model.md).
- [X] T008 Write a failing test for `FlagService` in `core/services/flag.service.spec.ts`
  (asserts a known country returns its bundled `flagcdn.com` URL and an unknown/custom country
  returns the fallback icon gracefully — spec.md Edge Case 2), confirm it fails (service
  doesn't exist yet), then implement
  `FlagService` (builds `flagcdn.com/{code}.svg` URLs, with a fallback generic flag icon) in
  `core/services/flag.service.ts` — per Constitution Principle I (Test-First, NON-NEGOTIABLE).
  Depends on T006.
- [X] T009 [P] Write a failing component test for `ScoreboardService` — using
  `HttpTestingController`, assert each of the 6 methods (`startMatch`, `getMatch`,
  `updateScore`, `finishMatch`, `getSummary`, `getHistory`) calls the correct endpoint/verb
  per contracts/frontend-api-usage.md — in `core/services/scoreboard.service.spec.ts`. Confirm
  it fails (service doesn't exist yet) before T010.
- [X] T010 Implement `ScoreboardService` in `core/services/scoreboard.service.ts` per
  contracts/frontend-api-usage.md. Depends on T005, T007, T009.
- [X] T011 Configure `app.config.ts`: `provideHttpClient()`, `provideAnimations()`, the
  Material theme from T002, and route stubs for `/summary`, `/history`, `/matches` (empty
  placeholder components for now). Depends on T002.
- [X] T012 [P] Write a failing component test for `SidenavComponent` — assert 3 nav links
  render (Summary/History/Matches, FR-002) — in `layout/sidenav/sidenav.component.spec.ts`.
  Confirm it fails before T013.
- [X] T013 Implement `SidenavComponent` + `AppComponent` shell, wiring the 3 routes from T011.
  Depends on T011, T012.

**Checkpoint**: The app builds and shows the side-nav shell with 3 empty routed pages;
`ScoreboardService` is fully implemented and unit-tested against a fake HTTP backend. No
user-story-specific UI exists yet — user story work can now begin.

---

## Phase 3: User Story 1 - View Live Summary and Paginated Match History (Priority: P1) 🎯 MVP

**Goal**: Summary and History tabs render matches as side-by-side country-card rows with a
"VS" separator; History adds pagination and status badges.

**Independent Test**: Navigate to the dashboard; switch between "Summary" and "History" tabs;
observe the match cards.

### Tests for User Story 1

> **Write these tests FIRST — confirm they FAIL (component/route doesn't exist yet) before
> writing implementation (T019-T023)**

- [X] T014 [P] [US1] Component test: `CountryCardComponent` renders a flag, country name, and
  score given a `Team` input, in
  `shared/components/country-card/country-card.component.spec.ts` (FR-004). Also covered by
  T008's `FlagService` test: an unknown/custom country name renders the generic fallback flag
  icon rather than a broken image — spec.md Edge Case 2.
- [X] T015 [P] [US1] Component test: `MatchRowComponent` renders a Home `CountryCardComponent`,
  a "VS" badge, then an Away `CountryCardComponent`, in that order, in
  `shared/components/match-row/match-row.component.spec.ts` (FR-003).
- [X] T016 [P] [US1] Component test: `SummaryComponent` calls `ScoreboardService.getSummary()`
  on init and renders one `MatchRowComponent` per returned match (plus an empty state when the
  list is empty); also assert that refreshing again before a prior request resolves cancels
  the stale request instead of letting it overwrite newer state (spec.md Edge Case 3,
  research.md §9), in `features/summary/summary.component.spec.ts`.
- [X] T017 [P] [US1] Component test: `HistoryComponent` calls `getHistory(page)`, renders a
  status badge (In Progress/Finished) per match, and shows pagination controls (10/page); also
  assert that requesting a new page before a prior page's request resolves cancels the stale
  request instead of letting it overwrite `currentPage`/`matches` (spec.md Edge Case 3,
  research.md §9), in `features/history/history.component.spec.ts`.
- [X] T018 [P] [US1] Playwright test: replay the brief's worked example through the UI (Mexico
  0-Canada 5, Spain 10-Brazil 2, Germany 2-France 2, Uruguay 6-Italy 6, Argentina 3-Australia 1)
  and assert the Summary tab's card order matches exactly; separately assert History shows 10
  results per page with newest activity first, in `e2e/scoreboard.spec.ts` — plan.md's Project
  Structure specifies a single combined Playwright spec file, not one per story.

### Implementation for User Story 1

- [X] T019 [P] [US1] Implement `CountryCardComponent` in `shared/components/country-card/`.
  Depends on T014.
- [X] T020 [P] [US1] Implement `MatchRowComponent` (composes two `CountryCardComponent`s + a
  "VS" badge) in `shared/components/match-row/`. Depends on T015, T019.
- [X] T021 [US1] Implement `SummaryComponent` (fetch via a `Subject` → `switchMap` →
  `takeUntilDestroyed(this.destroyRef)` pipeline, not a plain `.subscribe()` per call — cancels
  a stale in-flight request on rapid refresh or navigating away, per research.md §9 — render
  the `MatchRowComponent` list, empty state) in `features/summary/`. Depends on T016, T020,
  T010.
- [X] T022 [US1] Implement `HistoryComponent` (fetch-on-page-change via the same `Subject` →
  `switchMap` → `takeUntilDestroyed(this.destroyRef)` pipeline as T021, per research.md §9 —
  `currentPage` is only updated inside the inner pipe once that page's response actually
  arrives, so a cancelled page can never overwrite it — status badge, `MatPaginator`) in
  `features/history/`. Depends on T017, T020, T010.
- [X] T023 [US1] Wire the real `SummaryComponent`/`HistoryComponent` into the `/summary` and
  `/history` routes in `app.routes.ts` (replacing T011's placeholders). Depends on T021, T022.
- [X] T024 [US1] Run `ng test` filtered to US1 specs; confirm T014-T017 all pass. Then run the
  full component-test suite to confirm no regression against Phase 2's Foundational tests.

**Checkpoint**: Summary and History tabs are fully functional and independently testable — this
is the MVP scope.

---

## Phase 4: User Story 2 - Start a New Match from the Matches Tab (Priority: P2)

**Goal**: The Matches tab lets a user pick Home/Away countries from flag dropdowns and start a
match, with a success confirmation modal on `201 Created` (FR-011).

**Independent Test**: Go to "Matches", select Mexico/Canada, provide a location, click "Start
Match", confirm the success modal then the match appearing.

### Tests for User Story 2

> **Write these tests FIRST — confirm they FAIL before writing implementation (T028-T030)**

- [X] T025 [P] [US2] Component test: `SuccessDialogComponent` renders the given message, in
  `shared/components/success-dialog/success-dialog.component.spec.ts` (FR-011).
- [X] T026 [P] [US2] Component test: `MatchesComponent`'s start-match form — Home/Away dropdowns
  populated from `CountryOption` (T006), calls `ScoreboardService.startMatch(...)` on submit,
  and opens `SuccessDialogComponent` with a "Match started" message on `201`, in
  `features/matches/matches.component.spec.ts`.
- [X] T027 [P] [US2] Playwright test: start a match via the UI; assert the success modal
  appears, then the new match is visible in the active-match list; navigate to Summary/History
  and confirm it's immediately reflected (spec.md US2 Acceptance Scenario 2), added to the same
  `e2e/scoreboard.spec.ts` file as T018 (plan.md specifies one combined spec file).

### Implementation for User Story 2

- [X] T028 [P] [US2] Implement `SuccessDialogComponent` + a `SuccessDialogService` (opens it
  with an action-specific message, research.md §5a) in
  `shared/components/success-dialog/` and `core/services/success-dialog.service.ts`. Depends
  on T025.
- [X] T029 [US2] Implement `MatchesComponent`'s start-match form (Home/Away `CountryOption`
  dropdowns with flags via `FlagService`, location + scheduled-time inputs, "Start Match"
  button calling `ScoreboardService.startMatch`, opening the success dialog on `201`) in
  `features/matches/`. Depends on T026, T028, T010, T006, T008.
- [X] T030 [US2] Wire the real `MatchesComponent` into the `/matches` route in
  `app.routes.ts`. Depends on T029.
- [X] T031 [US2] Run `ng test` filtered to US2 specs; confirm T025-T026 pass. Then run the full
  suite to confirm no regression in US1.

**Checkpoint**: Matches can be started from the UI with a success confirmation; US1 and US2 are
both independently functional.

---

## Phase 5: User Story 3 - Update Scores and Finish Matches (Priority: P3)

**Goal**: An active match's score can be updated and the match finished from the Matches tab
(FR-010), each with a success confirmation modal (FR-011).

**Independent Test**: On an active match, update the score, click update, then click "Finish
Match".

### Tests for User Story 3

> **Write these tests FIRST — confirm they FAIL before writing implementation (T035)**

- [X] T032 [P] [US3] Component test: `MatchesComponent`'s active-match score-update controls
  call `ScoreboardService.updateScore(...)` and open the success dialog on `200`, added to
  `features/matches/matches.component.spec.ts` (extends T026's file).
- [X] T033 [P] [US3] Component test: `MatchesComponent`'s "Finish Match" button calls
  `finishMatch(...)` and opens the success dialog on `200`, added to the same spec file.
- [X] T034 [P] [US3] Playwright test: update an active match's score then finish it; assert
  both success modals appear, the score updates on screen, and the match moves from Summary to
  History marked Finished, added to the same `e2e/scoreboard.spec.ts` file as T018/T027.

### Implementation for User Story 3

- [X] T035 [US3] Implement score-update inputs + "Finish Match" button on `MatchesComponent`'s
  active-match list (FR-010), reusing the `SuccessDialogService` from T028. Depends on
  T032-T034, T029.
- [X] T036 [US3] Run `ng test` filtered to US3 specs; confirm T032-T033 pass. Then run the full
  suite to confirm no regression in US1-US2.

**Checkpoint**: Matches can be started, updated, and finished from the UI, each with a success
confirmation; US1-US3 are all independently functional.

---

## Phase 6: User Story 4 - Professional Error Dialog Handling (Priority: P4)

**Goal**: Any backend rejection (duplicate team, invalid score, etc.) surfaces as a Material
error dialog showing `error_code`/`error_message` (FR-007).

**Independent Test**: Start a match with a team already in-progress; confirm the error modal.

### Tests for User Story 4

> **Write these tests FIRST — confirm they FAIL before writing implementation (T040-T042)**

- [X] T037 [P] [US4] Component test: `ErrorDialogComponent` renders the given
  `error_code`/`error_message`, in
  `shared/components/error-dialog/error-dialog.component.spec.ts`. Also write a failing test
  for `ErrorDialogService.openError(...)` (named in plan.md's Project Structure) asserting it
  calls `MatDialog.open` with `ErrorDialogComponent` and the given message/title/code, in
  `core/services/error-dialog.service.spec.ts` — per Constitution Principle I, this service
  needs its own preceding test, not just the component it opens.
- [X] T038 [P] [US4] Component test: `ErrorInterceptor` catches a non-2xx response, extracts
  the `ApiError` body, and opens `ErrorDialogComponent`; also assert it shows a
  connection-failure message (not a raw/blank error) when the request fails with no HTTP
  response at all (`status: 0`) — spec.md Edge Case 1 ("backend unreachable") — in
  `core/interceptors/error.interceptor.spec.ts`.
- [X] T039 [P] [US4] Playwright test: attempt to start a match with a team already
  in-progress and assert the error dialog shows `error_code: "start_rejected"`'s message;
  repeat for an invalid (decreasing) score update, added to the same `e2e/scoreboard.spec.ts`
  file as T018/T027/T034.

### Implementation for User Story 4

- [X] T040 [P] [US4] Implement `ErrorDialogComponent` in `shared/components/error-dialog/`,
  plus `ErrorDialogService` (opens it via `MatDialog`) in
  `core/services/error-dialog.service.ts` — the interceptor (T041) calls the service, not
  `MatDialog` directly. Depends on T037.
- [X] T041 [US4] Implement `ErrorInterceptor` (functional `HttpInterceptorFn`) in
  `core/interceptors/error.interceptor.ts`. Depends on T038, T040, T007.
- [X] T042 [US4] Register `ErrorInterceptor` via
  `provideHttpClient(withInterceptors([errorInterceptor]))` in `app.config.ts`. Depends on
  T041, T011.
- [X] T043 [US4] Run `ng test` filtered to US4 specs; confirm T037-T038 pass. Then run the full
  suite to confirm no regression in US1-US3.

**Checkpoint**: Backend rejections surface as a professional error modal everywhere; US1-US4
are all independently functional.

---

## Phase 7: User Story 5 - Full Docker Compose & Playwright End-to-End Test Suite (Priority: P5)

**Goal**: `docker compose up` runs both services together; the full Playwright suite (already
written across US1-US4 above) passes against the containerized stack.

**Independent Test**: `docker compose up` then `npx playwright test`.

*No new test-writing tasks here — this story exercises the Playwright specs already written in
T018/T027/T034/T039 against the Dockerized stack; that run **is** the independent test.*

### Implementation for User Story 5

- [X] T044 [P] [US5] Create a multi-stage `Dockerfile` (Node build stage → `nginx:alpine`
  runtime stage serving the built static assets) at `src/WorldCupScoreboard.Frontend/Dockerfile`
  (research.md §7).
- [X] T045 [P] [US5] Create `.dockerignore` for the frontend build context (`node_modules/`,
  `e2e/`, `dist/`, etc.).
- [X] T046 [US5] Create the root `docker-compose.yml` orchestrating `api` (the existing root
  `Dockerfile` from `006`, port 5000) and `frontend` (T044, port 4200). Depends on T044.
- [X] T047 [US5] Run `docker compose up --build`; confirm both containers start, and both
  Swagger (`:5000/swagger`) and the frontend (`:4200`) are reachable (FR-008, spec.md US5
  Acceptance Scenario 1). Depends on T046.
- [X] T048 [US5] Run the full Playwright suite (T018, T027, T034, T039) against the Dockerized
  stack; confirm every scenario passes (spec.md US5 Acceptance Scenario 2, SC-002). Depends on
  T047.

**Checkpoint**: All five user stories are complete — `docker compose up` + `npx playwright test`
is the full, containerized, automated proof the entire feature works end-to-end.

---

## Phase 8: Polish & Cross-Cutting Concerns

- [X] T049 [P] Run `ng lint` (and formatting check) across `src/WorldCupScoreboard.Frontend/`.
- [X] T050 Walk through `quickstart.md`'s full manual validation (all 9 steps) against both
  `ng serve` (local) and the Dockerized instance.
- [X] T051 [P] Confirm `dotnet test` (the existing `006`/library suite) is still green — no
  backend regression from any CORS/contract assumption made in this feature.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: T001 before T002-T004 (same workspace). No other dependencies.
- **Foundational (Phase 2)**: Depends on Setup. T005-T007 (models) are independent of each
  other; T008 depends on T006; T009-T010 (service test then implementation) depend on
  T005/T007; T011 depends on T002; T012-T013 (nav test then implementation) depend on T011.
  BLOCKS every user story.
- **User Stories (Phases 3-7)**: Each depends on Foundational only, but — as in `006`'s own
  task structure — several implementation tasks across stories touch the same shared files
  (`app.routes.ts`, `app.config.ts`, `matches.component.ts`), so while all five stories' *tests*
  can be written in parallel once Foundational is done, US2's and US3's `MatchesComponent`
  implementation tasks (T029, T035) are sequential (same file), and US4's `app.config.ts` edit
  (T042) is sequential with US2/US3's `app.routes.ts` edits (T023, T030) only in the loose sense
  of "don't clobber each other's uncommitted changes" — they touch different files
  (`app.config.ts` vs `app.routes.ts`) so are not strictly ordered, but the natural execution
  order is still P1→P2→P3→P4→P5 to keep each checkpoint meaningful.
- **Polish (Phase 8)**: Depends on all five user stories being complete.

### Within Each User Story

- That story's test task(s) MUST be written and FAIL before its implementation task(s), per
  Constitution Principle I.
- All Playwright tests across every story live in the single `e2e/scoreboard.spec.ts` file
  (plan.md's Project Structure), not one file per story — later stories (US3, US4) append their
  scenarios to the same file rather than creating new ones, which also suits how US3 continues
  from a match US2 already started.

### Parallel Opportunities

- T002-T004 (Setup) can run in parallel once T001 completes.
- T005-T007 (Foundational models) can run in parallel; T009 and T012 (Foundational tests) can
  run in parallel with each other.
- **All five user stories' test-writing tasks (T014-T018, T025-T027, T032-T034, T037-T039) can
  be written in parallel with each other** — different files, and each only depends on
  Foundational (T010's `ScoreboardService`), not on any other story's implementation landing
  first. Only the *implementation* tasks that share a file (`matches.component.ts` across
  US2/US3) are forced sequential.
- T044-T045 (Setup for US5's Docker files) can run in parallel with each other.

---

## Parallel Example: Writing Every Story's Tests Up Front

```bash
# All five stories' tests can be written in parallel once Foundational (T005-T013) is done:
Task: "Component test for CountryCardComponent in shared/components/country-card/country-card.component.spec.ts"
Task: "Component test for SuccessDialogComponent in shared/components/success-dialog/success-dialog.component.spec.ts"
Task: "Component test for ErrorDialogComponent in shared/components/error-dialog/error-dialog.component.spec.ts"
Task: "Playwright test for summary ordering + history pagination in e2e/scoreboard.spec.ts"
Task: "Playwright test for the duplicate-team/invalid-score error modals, appended to the same e2e/scoreboard.spec.ts"
```

---

## Implementation Strategy

### MVP First — User Story 1

1. Complete Phase 1: Setup (T001-T004)
2. Complete Phase 2: Foundational (T005-T013)
3. Complete Phase 3: User Story 1 (T014-T024)
4. **STOP and VALIDATE**: `ng test` green, Summary/History tabs work when tried in the
   browser, the brief's worked example renders in the exact documented order.
5. This is a shippable increment — viewing live matches and history over the UI.

### Incremental Delivery

1. Setup + Foundational → the app shell exists, builds, shows 3 empty nav routes, has a fully
   tested `ScoreboardService`.
2. Add User Story 1 → test independently → MVP.
3. Add User Story 2 → test independently, confirm no US1 regression.
4. Add User Story 3 → test independently, confirm no US1-2 regression.
5. Add User Story 4 → test independently, confirm no US1-3 regression.
6. Add User Story 5 → test independently, confirm no US1-4 regression — the whole feature is
   now provable end-to-end in Docker.
7. Polish (lint, full quickstart walkthrough, backend regression check).

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions, this feature maps to one or a couple of small,
  reviewable commits — but do not commit without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing, then passing)
  before moving to the next task — no production code without a preceding failing test, same
  as `001-006`.

---

## Phase 9: Convergence

**Purpose**: Close the gap between `spec.md`/`plan.md`/`tasks.md`'s intent and the current
state of `src/WorldCupScoreboard.Frontend/`, which was implemented outside this session (T001
through most of T044/T046 already exist in some form) but was never verified against these
artifacts. Found via `/speckit-converge` on 2026-08-04.

- [X] T052 CRITICAL fix the `Match`/`Team` frontend models in `core/models/match.model.ts` to
  match `006-scoreboard-api`'s actual JSON contract — nested `homeTeam`/`awayTeam` objects
  (`{ name: string; score: number }`), not flat `homeTeam: string`/`homeScore: number` — and
  update every component that reads those fields (`MatchesComponent`, `MatchRowComponent`,
  `CountryCardComponent`'s callers, `SummaryComponent`, `HistoryComponent`) accordingly, per
  data-model.md (contradicts)
- [X] T053 CRITICAL fix `MatchRowComponent`'s status badge (`match-row.component.ts`) to use
  the numeric `status` enum (`0` = InProgress, `1` = Finished) instead of comparing/
  lowercasing it as a string — the current `(match.status || '').toLowerCase()` throws a
  runtime `TypeError` the moment a Finished match (`status: 1`, truthy) renders in History,
  per FR-003 / US1 Acceptance Scenario 3 (contradicts)
- [X] T054 [P] implement `SuccessDialogComponent` + `SuccessDialogService` and wire them into
  `MatchesComponent`'s start/update/finish success (`.subscribe.next`) handlers, per FR-011
  (missing)
- [X] T055 write the missing Karma/Jasmine component/service tests — `ScoreboardService`,
  `errorInterceptor`, `CountryCardComponent`, `MatchRowComponent`, `SidenavComponent`,
  `SummaryComponent`, `HistoryComponent`, `MatchesComponent`, `ErrorDialogComponent` — per
  Constitution Principle I (Test-First, NON-NEGOTIABLE), which currently has zero coverage
  beyond the CLI-scaffolded `app.component.spec.ts` (missing)
- [X] T056 extend `e2e/scoreboard.spec.ts` (or split per tasks.md's original per-story file
  plan) to assert the success modal appears (once T054 lands), replay the full 5-match brief
  worked example for Summary ordering (currently only 2 of 5 matches are exercised), and assert
  History's 10-per-page pagination behavior explicitly, per US1 AC3 / US2 AC1 / US3 AC1-2
  (partial)
- [X] T057 [P] either implement the planned `FlagService` (`core/services/flag.service.ts`) so
  `plan.md`'s Project Structure matches reality, or update `plan.md`/`research.md` §6 to
  document the simpler function-based approach (`getCountryFlagUrl` in `country.model.ts`) as
  the actual decision — currently undocumented drift between plan and code (unrequested)
- [X] T058 [P] add `.dockerignore` for the frontend Docker build context in
  `src/WorldCupScoreboard.Frontend/.dockerignore`, per T045 (missing)
- [X] T059 [P] configure Angular ESLint (`ng add @angular-eslint/schematics` or equivalent) so
  `ng lint` is runnable, per T004/T049 (missing)
