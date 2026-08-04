# Implementation Plan: Scoreboard Frontend

**Branch**: `007-scoreboard-frontend` | **Date**: 2026-08-04 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/007-scoreboard-frontend/spec.md`

**Note**: this file was originally authored in a parallel session with a shorter, non-standard
structure (no Technical Context / Constitution Check / Project Structure per this repo's
`plan-template.md`). The first `/speckit-plan` re-run on 2026-08-04 added those missing
sections and generated the Phase 0/1 artifacts (`research.md`, `data-model.md`, `contracts/`,
`quickstart.md`) while preserving the original Architecture Overview / Key Technical Decisions
/ Verification Plan content as-is. This second re-run (same day) folds in **FR-011** (the
success-confirmation modal added via a follow-up `/speckit-clarify` round) into that same
structure — see research.md §5a for the new decision record.

## Summary

A separate Angular 18+ single-page application, styled with Angular Material in a white/blue
palette, that consumes `006-scoreboard-api`'s existing REST contract for all 6 `IScoreboard`
operations. Three side-nav sections (Summary, History, Matches) render matches as side-by-side
country cards with a "VS" separator; the Matches tab additionally supports starting matches
(country dropdowns), updating scores, and finishing matches. Backend rejections surface as a
Material dialog showing `error_code`/`error_message`. Both services run via one root
`docker-compose.yml`. A Playwright E2E suite validates every backend business rule through the
UI (FR-009, reconfirmed in scope via `/speckit-clarify`).

## Technical Context

**Language/Version**: TypeScript 5.x on Angular 18+ (frontend); consumes the existing .NET 9
`WorldCupScoreboard.Api` (006) unchanged, aside from the CORS policy already added there.

**Primary Dependencies**: `@angular/material`, `@angular/cdk`, RxJS, Angular `HttpClient` +
`HttpInterceptorFn`, Playwright (E2E), Karma + Jasmine (`ng test` default, component/unit
tests) — see research.md §1-3, §5a, §8.

**Storage**: N/A — the frontend is stateless; all persistence remains in `006`'s SQLite-backed
library, accessed only over HTTP (Constitution Principle IV).

**Testing**: Karma/Jasmine for component tests (e.g. `CountryCardComponent`,
`MatchRowComponent`, `ErrorDialogComponent`, `SuccessDialogComponent` render correctly given
inputs); Playwright for end-to-end tests against a running frontend + API pair, covering every
user story's acceptance scenarios, including the success-modal appearing on `201`/`200`
responses (research.md §3, §5a, §8).

**Target Platform**: Web browser (modern evergreen browsers), served as a static SPA via Nginx
in both the dev and containerized (`docker compose`) setups.

**Project Type**: Web frontend — pairs with the existing `006-scoreboard-api` backend (template
Option 2: Web application, backend + frontend).

**Performance Goals**: Tab switch (Summary/History/Matches) reflects the latest backend state
within one HTTP round-trip, with the fetched data rendered within 100ms of the response
arriving (client-side rendering overhead only, excluding network/backend latency); initial
dashboard load renders the shell before data arrives (loading state, not a blank screen).

**Constraints**: Frontend MUST NOT access the database or `WorldCupScoreboard`/
`WorldCupScoreboard.Api` project directly — REST calls only (Constitution Principle IV). No new
backend endpoints — `007` consumes `006`'s contract exactly as published in
`specs/006-scoreboard-api/contracts/api.md`.

**Scale/Scope**: 3 navigation sections, ~11 components (side nav, 2 card types, 3 feature
pages, error dialog, success dialog, plus supporting services/interceptor), 5 user stories per
`spec.md`.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design below.*

| Principle | Assessment |
|---|---|
| I. Test-First (NON-NEGOTIABLE) | **PASS.** Every component (including the new `SuccessDialogComponent`, FR-011) and the error interceptor get a Karma/Jasmine test written before implementation; every user story's acceptance scenarios get a corresponding Playwright test, including the success-modal assertions added to US2/US3. No production code without a preceding failing test, same as `001-006`. |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | **PASS.** Procedural — applies during `/speckit-implement` (reproduce → state fix → minimal fix → full-suite re-run) the same way it did for `001-006`. Not a design-time artifact gate. |
| III. Single-Concern Features | **PASS**, by the same precedent `006-scoreboard-api` already set: one Spec-Kit feature (`007`) can span multiple independently-testable-and-shippable user stories (Summary+History view, Match creation, Score/Finish management, Error handling, Docker+E2E) as long as each is independently testable — which `spec.md`'s 5 user stories already are. |
| IV. Layered Architecture / Library-First | **PASS.** The frontend only calls `006`'s REST endpoints (research.md §4) — no direct `IMatchRepository`/EF Core/database access, no duplicated business logic (validation, ordering, pagination all stay server-side; the frontend renders what the API returns). |
| V. Runnable Local Verification (CLI Demo) | **N/A for this phase**, per the constitution's own text: the CLI demo requirement is explicitly "distinct from the Phase 2 API and Phase 3 frontend in the roadmap." `007`'s own equivalent runnable-verification story is `docker compose up` + the Playwright suite (User Story 5), not a CLI demo. |

No violations requiring justification — Complexity Tracking table below is empty.

## Project Structure

### Documentation (this feature)

```text
specs/007-scoreboard-frontend/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
├── checklists/
│   └── requirements.md
└── tasks.md              # Phase 2 output (/speckit-tasks command)
```

### Source Code (repository root)

```text
src/WorldCupScoreboard.Frontend/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── models/ (match.model.ts, country.model.ts, api-error.model.ts)
│   │   │   ├── services/ (scoreboard.service.ts, flag.service.ts, error-dialog.service.ts,
│   │   │   │   success-dialog.service.ts)
│   │   │   └── interceptors/ (error.interceptor.ts)
│   │   ├── shared/
│   │   │   └── components/
│   │   │       ├── country-card/ (country-card.component.ts|html|scss|spec.ts)
│   │   │       ├── match-row/ (match-row.component.ts|html|scss|spec.ts)
│   │   │       ├── error-dialog/ (error-dialog.component.ts|html|scss|spec.ts)
│   │   │       └── success-dialog/ (success-dialog.component.ts|html|scss|spec.ts)
│   │   ├── features/
│   │   │   ├── summary/ (summary.component.ts|html|scss|spec.ts)
│   │   │   ├── history/ (history.component.ts|html|scss|spec.ts)
│   │   │   └── matches/ (matches.component.ts|html|scss|spec.ts)
│   │   ├── layout/
│   │   │   └── sidenav/ (sidenav.component.ts|html|scss|spec.ts)
│   │   ├── app.component.ts|html|scss
│   │   └── app.config.ts / app.routes.ts
│   ├── assets/
│   ├── styles.scss (Material theme + white/blue palette)
│   └── index.html
├── e2e/ (Playwright E2E tests: scoreboard.spec.ts, playwright.config.ts)
├── Dockerfile
├── package.json
└── angular.json

docker-compose.yml        # repo root — orchestrates `api` (006) + `frontend` (007)
```

**Structure Decision**: Web application structure (Option 2), mirroring `006`'s
`src/WorldCupScoreboard.Api/` sibling placement — `src/WorldCupScoreboard.Frontend/` sits
alongside the existing `src/WorldCupScoreboard/` (library) and `src/WorldCupScoreboard.Api/`
(Phase 2 API) projects, keeping all three phases under one `src/` root per the repo's existing
convention rather than introducing a separate top-level `frontend/` directory.

## Architecture Overview

The frontend architecture consists of an Angular 18 Single Page Application (SPA) built with
Angular Material components, styled with a customized White and Blue theme. The frontend
communicates with the .NET 9 Web API (`WorldCupScoreboard.Api`) over HTTP.

### Key Technical Decisions

1. **Angular & Material**:
   - Modern standalone components (`standalone: true`).
   - Angular Material Toolbar, Sidenav, Cards, Select, Inputs, Buttons, Dialogs, and Paginator.
   - White (`#ffffff`) and Deep Blue (`#0a369d` / `#1e5f74` / `#003366`) palette with sleek
     glassmorphism and subtle shadows.

2. **Backend Integration & CORS**:
   - CORS is already enabled in the `.NET API` `Program.cs` (`AddCors`/`UseCors`,
     `AllowAnyOrigin`/`AllowAnyHeader`/`AllowAnyMethod`) — no further backend change needed.
   - HTTP Client calls to `/matches`, `/matches/{id}/score`, `/matches/{id}/finish`,
     `/matches/summary`, `/matches/history` — see `contracts/frontend-api-usage.md`.

3. **Error Interceptor & Dialog**:
   - A functional `HttpInterceptorFn` catches non-2xx API responses (e.g. 400 Bad Request with
     `error_message`).
   - Calls `ErrorDialogService.openError(...)`, which opens `MatDialog` with
     `ErrorDialogComponent` — the interceptor never calls `MatDialog` directly, mirroring how
     the success path (point 3a) also opens its dialog through a dedicated service rather than
     inline.

3a. **Success Dialog (FR-011)**:
   - `MatchesComponent`'s calls to `ScoreboardService.startMatch`/`updateScore`/`finishMatch`
     each open a shared `SuccessDialogComponent` via `MatDialog` on `201`/`200`, with an
     action-specific message ("Match started", "Score updated", "Match finished") — the
     dialog-opening happens at the calling component, not inside `ScoreboardService` itself
     (see point 3's parallel note on the error path) — a separate component from
     `ErrorDialogComponent`, not a shared variant (research.md §5a).
   - Deliberately triggered at the service call site, not the `HttpInterceptorFn` — the
     interceptor only sees non-2xx responses and has no natural per-action message context.

4. **Docker Compose**:
   - Multi-stage Dockerfile for the Angular frontend using Nginx to serve built static assets.
   - `docker-compose.yml` orchestrating `api` (port 5000) and `frontend` (port 4200).

5. **Playwright E2E Testing**:
   - Node-based Playwright test suite in `e2e/scoreboard.spec.ts`.
   - Validates Start Match, Duplicate Team Error Modal, Update Score, Finish Match, Live
     Summary Ordering, and History Pagination.

6. **Request Cancellation (spec.md Edge Case 3)**:
   - `SummaryComponent`/`HistoryComponent` fetch via a `Subject` → `switchMap` →
     `takeUntilDestroyed(this.destroyRef)` pipeline rather than a plain `.subscribe()` per
     call, so a rapid re-trigger (or navigating away) cancels the previous in-flight request
     instead of letting a stale response overwrite newer state (research.md §9).

## Verification Plan

### Automated Tests

- `ng test` (Karma/Jasmine) for every component/service/interceptor.
- `npm run build` in the Angular project.
- Playwright E2E suite (`npx playwright test`).

### Manual Verification

- `docker compose up` to run both API and Frontend containers simultaneously.
- Accessing `http://localhost:4200` in browser, testing tab navigation, match creation, score
  updates, finishing matches, error modals, summary ordering, and history — see
  `quickstart.md` for the full walkthrough.

## Complexity Tracking

*No Constitution Check violations — this table is intentionally empty.*
