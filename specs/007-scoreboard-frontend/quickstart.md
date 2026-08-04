# Quickstart: Scoreboard Frontend

Validation guide for `specs/007-scoreboard-frontend`. Wraps `006-scoreboard-api` (already
implemented) with an Angular UI.

## Prerequisites

- Node.js 20+ and Angular CLI, for local (non-Docker) frontend runs.
- .NET 9 SDK, for running `006-scoreboard-api` locally.
- Docker, for the containerized run.
- `006-scoreboard-api` already implemented (it is).

## Build & test

```bash
# Frontend component/unit tests
cd src/WorldCupScoreboard.Frontend
npm test        # ng test, Karma/Jasmine, headless CI mode

# Frontend production build
npm run build

# Backend (unchanged from 006 — no regression check skipped)
cd ../..
dotnet build
dotnet test
```

## Run locally (no Docker)

```bash
# Terminal 1 — backend
dotnet run --project src/WorldCupScoreboard.Api --urls http://localhost:5000

# Terminal 2 — frontend
cd src/WorldCupScoreboard.Frontend
npm start        # ng serve, http://localhost:4200
```

Open `http://localhost:4200`.

## Run in Docker Compose

```bash
docker compose up --build
```

Then open `http://localhost:4200` (frontend, Nginx-served) — it talks to the API container at
`http://localhost:5000`.

## Manual validation walkthrough

Exercises every acceptance scenario in `spec.md` directly through the UI.

1. **Dashboard shell** (US1, Acceptance Scenario 1): dashboard loads with a left side nav
   showing "Summary", "History", "Matches", styled in the white/blue Material theme.

2. **Start a match** (US2/FR-011): go to "Matches", select "Mexico" (home) and "Canada" (away)
   from the flag dropdowns, enter a location, click "Start Match". Expect a success
   confirmation modal (`201 Created`) followed by the new match appearing in the active-match
   list on the same tab.

3. **Duplicate-team rejection modal** (US4): attempt to start a second match with "Mexico"
   again while it's still in-progress. Expect a Material dialog showing
   `error_code: "start_rejected"`'s message, not a silent failure or console-only error.

4. **Live refresh on tab switch** (US1 Acceptance Scenario 2, US2 Acceptance Scenario 2):
   switch to "Summary" — the match just started is immediately visible as a country-vs-country
   card row with a "VS" separator, flags, names, and `0-0` score. Repeatedly clicking
   History↔Summary in rapid succession (spec.md Edge Case 3) should never throw a console error
   or leave the page stuck on stale data — each click cancels the previous in-flight request
   (research.md §9).

5. **Update score** (US3/FR-010/FR-011): back on "Matches", update the active match's score
   (e.g. `2`-`1`) and submit. Expect a success confirmation modal (`200 OK`) followed by the
   on-screen score updating; switch to "Summary" and confirm the new score is reflected there
   too.

6. **Invalid score rejection modal** (US4): attempt to submit a lower score than currently
   recorded. Expect a Material dialog showing `error_code: "invalid_score"`'s message.

7. **Finish the match** (US3/FR-010/FR-011): click "Finish Match". Expect a success
   confirmation modal (`200 OK`) followed by the match disappearing from "Summary" (no longer
   in-progress) and appearing in "History" marked "Finished".

8. **History pagination** (US1 Acceptance Scenario 3): start more than 10 matches total across
   the session (or reuse fixture data), open "History", and confirm pagination controls show
   10 per page, newest activity first.

9. **Playwright E2E suite** (US5):
   ```bash
   cd src/WorldCupScoreboard.Frontend
   npx playwright test
   ```
   Expect every scenario above (steps 2-8, automated) to pass green against a running
   frontend + API pair.

## Expected outcome

`ng test` (component/unit) and `npx playwright test` (e2e) both report all tests passing,
`dotnet test` still reports the full `006`/library suite green (no regression), and the manual
steps above behave as described whether run locally or via `docker compose up`.
