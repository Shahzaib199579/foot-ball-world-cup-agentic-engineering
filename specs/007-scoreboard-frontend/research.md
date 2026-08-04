# Research: Scoreboard Frontend

Phase 0 output for `007-scoreboard-frontend`. Resolves every open technical decision implied
by `spec.md` and the Technical Context in `plan.md`.

## 1. Angular version and component style

- **Decision**: Angular 18+, standalone components (`standalone: true`) — no `NgModule`s.
- **Rationale**: standalone components are the current Angular default/recommended style since
  v17, avoid boilerplate module wiring, and keep each feature (Summary/History/Matches)
  self-contained per Constitution Principle III (Single-Concern Features).
- **Alternatives considered**: classic `NgModule`-based architecture — rejected, no benefit for
  a project this size and goes against current Angular guidance.

## 2. Angular Material + theming

- **Decision**: `@angular/material` with a custom theme built from Angular Material's theming
  API, using the white/blue palette already fixed in `spec.md` FR-001
  (`#003366`, `#0055a5`, `#e8f0fe`, `#ffffff`).
- **Rationale**: explicitly requested by the user ("material design theme, white and blue color
  pallet"); Angular Material's `mat.define-theme`/palette APIs are the standard way to apply a
  custom palette across every component consistently instead of overriding component styles
  one-by-one.
- **Alternatives considered**: a non-Material component kit (e.g. PrimeNG, Tailwind UI) —
  rejected, contradicts the explicit "material design theme" requirement.

## 3. Component/unit test runner

- **Decision**: Karma + Jasmine (Angular CLI's default `ng test` toolchain) for component/unit
  tests, alongside the already-planned Playwright suite for e2e.
- **Rationale**: Constitution Principle IV requires every layer be independently testable —
  "frontend via component/e2e tests." Playwright (already committed to in `spec.md` FR-009)
  covers the e2e side; Karma/Jasmine is the zero-extra-config default `ng new` ships with for
  the component side, so component-level tests (e.g. `CountryCardComponent` renders flag/name/
  score correctly, `MatchRowComponent` renders home/VS/away in the right order) don't require
  introducing a second toolchain.
- **Alternatives considered**: Jest — a reasonable alternative some Angular teams now prefer,
  but requires extra build configuration (`jest-preset-angular` or the newer experimental Angular
  Jest builder) with no functional benefit for this project's scope; rejected to keep the setup
  minimal, consistent with the backend's own "simplicity over throughput" trade-off philosophy
  (CLAUDE.md).

## 4. Backend integration & CORS

- **Decision**: the frontend calls `006-scoreboard-api`'s existing REST contract directly over
  `HttpClient`, no BFF/proxy layer. CORS is already enabled on the API side
  (`src/WorldCupScoreboard.Api/Program.cs` — `AddCors`/`UseCors` with `AllowAnyOrigin`/
  `AllowAnyHeader`/`AllowAnyMethod`, added between the `006` implementation session and this
  one).
- **Rationale**: Constitution Principle IV — the frontend is a "thin presentation layer that
  only calls the API — no direct library access, no duplicated business logic." A BFF/proxy
  would add a layer with no requirement driving it. CORS being already permissive means no
  further backend change is needed for this feature; `Assumptions` in `spec.md` (port
  4200/5000) already anticipated this.
- **Alternatives considered**: a stricter CORS policy scoped to `http://localhost:4200` only —
  reasonable for production hardening, but out of scope here (the existing policy was already
  set by a prior session/linter pass and changing it isn't a `007` frontend concern; noted as a
  possible future hardening item, not a blocker).

## 5. Error handling: `HttpInterceptorFn` + `ErrorDialogService`

- **Decision**: a single functional `HttpInterceptorFn` (Angular's modern interceptor style)
  catches any non-2xx response, extracts the `ApiError` (`error_code`/`error_message`) body per
  `006-scoreboard-api`'s contract, and calls `ErrorDialogService.openError(...)`, which opens
  the shared `ErrorDialogComponent` via `MatDialog` — the interceptor never calls `MatDialog`
  directly, keeping that one line of dialog-opening logic in one place rather than duplicated
  anywhere else `ErrorDialogComponent` might need to be shown.
- **Rationale**: FR-007 requires intercepting API errors and showing a Material dialog with
  `error_message`; centralizing this in one interceptor (rather than per-call `catchError`
  blocks) means the mapping is written once, mirroring the same "shared mapping, not duplicated
  per call site" principle already used for `006`'s `ApiErrorExtensions` on the backend.
- **Alternatives considered**: per-component `catchError` + inline `MatSnackBar` — rejected,
  duplicates the same handling logic in every component that calls the API and contradicts
  FR-007's "handled and shown in a professional manner" consistently everywhere.

## 5a. Success confirmation: `MatDialog` on the calling component, not the interceptor

- **Decision**: FR-011's success confirmation (start/update/finish → `201`/`200`) opens a
  shared `SuccessDialogComponent` via `MatDialog`, per explicit user choice (`/speckit-clarify`,
  2026-08-04 round 2) of a modal over a snackbar — same dialog *family* as the error path
  (`ErrorDialogComponent`), but a **separate component with its own template** (a checkmark/
  affirmative styling, not reusing the error template with different text) so the two states
  stay visually distinct. Triggered from `MatchesComponent`'s three call sites into
  `ScoreboardService` (`startMatch`/`updateScore`/`finishMatch`) — i.e. in the component's own
  `.subscribe` success callback, not inside `ScoreboardService`'s methods themselves, and
  **not** from the `HttpInterceptorFn` either.
- **Rationale**: the interceptor only sees the HTTP layer and only fires on non-2xx responses
  (research.md §5) — it has no natural hook for the 2xx/success path, and success handling is
  inherently specific to which of the three actions just succeeded (different message per
  action: "Match started", "Score updated", "Match finished"), unlike the error path where one
  generic `error_message` string covers every case. `ScoreboardService` stays a thin HTTP
  client with no dialog-opening logic of its own; keeping success confirmation at the calling
  component (`MatchesComponent`'s own `.subscribe`) is the natural place for action-specific
  logic, while still funneling through one shared `SuccessDialogComponent` so the dialog itself
  isn't duplicated three times.
- **Alternatives considered**: extending the same `HttpInterceptorFn` to also handle 2xx
  responses — rejected, forces the interceptor to know action-specific message text, breaking
  the "one generic error mapping" simplicity that made the interceptor worth having in the
  first place (research.md §5's own rationale); a single dialog component reused for both
  success and error with a `variant` flag — rejected, the user's own choice was for visual
  consistency in *mechanism* (`MatDialog`) not necessarily identical *styling*, and a dedicated
  success template is simpler to test in isolation (Karma/Jasmine, research.md §3) than a
  conditional-styling shared component.

## 6. Country/flag data source

- **Decision**: a static `CountryOption[]` list bundled with the frontend (name + ISO-3166
  alpha-2 code), rendering flags via `https://flagcdn.com/{code}.svg`, with a generic fallback
  flag icon for any code that fails to load.
- **Rationale**: matches `spec.md`'s own Assumptions section exactly; a static list avoids
  needing a new backend endpoint (out of scope — country data isn't part of `IScoreboard`) and
  keeps the dropdown deterministic/offline-friendly for the Playwright suite.
- **Alternatives considered**: a live third-party countries API (e.g. restcountries.com) —
  rejected, adds a runtime external dependency and failure mode for data that doesn't change.

## 7. Docker Compose & multi-stage frontend build

- **Decision**: `Dockerfile` at `src/WorldCupScoreboard.Frontend/` — a Node build stage
  (`ng build`) producing static assets, served by an `nginx:alpine` runtime stage. A root
  `docker-compose.yml` orchestrates `api` (built from the existing root `Dockerfile`, port 5000)
  and `frontend` (port 4200).
- **Rationale**: FR-008 requires a `docker-compose.yml` that "builds and launches both .NET API
  and Angular frontend"; Nginx serving pre-built static assets is the standard, minimal-runtime
  pattern for Angular production containers (no Node runtime needed at serve time).
- **Alternatives considered**: serving via `ng serve` inside the container — rejected, that's a
  dev server not meant for production-style container runs and doesn't produce optimized
  static output.

## 8. Playwright E2E suite

- **Decision**: a Node-based Playwright project at `src/WorldCupScoreboard.Frontend/e2e/`,
  running against the built frontend + a real (or in-memory-backed test) instance of the API,
  covering: start match (success + duplicate-team rejection modal), score update (success +
  invalid-score rejection modal), finish match, live summary ordering (the brief's worked
  example, replayed through the UI), and history pagination.
- **Rationale**: explicitly requested (FR-009, confirmed in scope again via the
  `2026-08-04` `/speckit-clarify` session) — "Make sure that there are playwright test to test
  all the test cases that api handles from the front-end side."
- **Alternatives considered**: Cypress — a comparable tool, but Playwright was named explicitly
  by the user; no reason to substitute.

## 9. Request cancellation for rapid tab switching (spec.md Edge Case 3)

- **Decision**: `SummaryComponent` and `HistoryComponent` route their data fetches through a
  `Subject` trigger piped through RxJS `switchMap` (cancels the previous in-flight request the
  moment a new one is triggered) and `takeUntilDestroyed(this.destroyRef)` (cancels anything
  still in flight when the component is destroyed — e.g. the user navigates to a different
  route before a response arrives), instead of a plain `service.method().subscribe(...)` call
  per fetch.
- **Rationale**: spec.md's Edge Case 3 explicitly calls for "RxJS state management/HTTP
  cancellation" so a stale, out-of-order response can never overwrite newer state — this was
  identified as unimplemented (not just untested) via `/speckit-analyze` (finding E1) after the
  feature's initial implementation, then fixed directly. `HistoryComponent`'s `currentPage` is
  updated only inside the inner pipe (after that specific page's response actually arrives),
  so a cancelled page's stale response can never incorrectly claim to be the current page.
- **Alternatives considered**: manual `Subscription` tracking + explicit `.unsubscribe()` calls
  — rejected, `switchMap`/`takeUntilDestroyed` are the idiomatic RxJS/Angular primitives for
  exactly this "cancel the previous, keep only the latest" pattern and require no manual
  bookkeeping; debouncing instead of cancelling — rejected, debouncing delays every request
  rather than just discarding stale ones, which doesn't match "handles stale requests cleanly"
  as directly.
