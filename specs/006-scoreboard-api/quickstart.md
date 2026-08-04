# Quickstart: Scoreboard API

Validation guide for `specs/006-scoreboard-api`. Wraps `001`-`005`'s already-implemented
library over HTTP.

## Prerequisites

- .NET 9 SDK installed (`dotnet --version` → `9.x`), for local (non-Docker) runs.
- Docker installed, for the containerized run.
- `001-start-match` through `005-match-history` implemented (they are).

## Build & test

```bash
dotnet build
dotnet test
```

All tests in `tests/WorldCupScoreboard.Api.Tests/` must pass, alongside the full existing
`WorldCupScoreboard.Tests` suite (no regression). These are integration tests against real HTTP
endpoints (via `WebApplicationFactory`), using the `InMemoryMatchRepository` fake — no real
SQLite database involved in the test run.

## Run locally (no Docker)

```bash
dotnet run --project src/WorldCupScoreboard.Api
```

Then open `http://localhost:<port>/swagger` (the port is printed on startup) — this is the
interactive Swagger UI (FR-009). Every endpoint in `contracts/api.md` can be invoked directly
from there, no separate HTTP client needed.

## Run in Docker

```bash
docker build -t scoreboard-api .
docker run -p 8080:8080 scoreboard-api
```

Then open `http://localhost:8080/swagger`.

## Manual validation walkthrough

Exercises the contract in `contracts/api.md` directly — either via Swagger UI or `curl`.

1. **Start a match** (US1, Acceptance Scenario 1):
   ```bash
   curl -i -X POST http://localhost:8080/matches \
     -H "Content-Type: application/json" \
     -d '{"homeTeam":"Mexico","awayTeam":"Canada","scheduledAt":"2026-08-04T15:00:00Z","location":"Estadio Azteca"}'
   ```
   Expect `201 Created` with the new match's JSON body.

2. **A rejected start** (US1, Acceptance Scenario 2) — repeat the same call:
   ```bash
   curl -i -X POST http://localhost:8080/matches \
     -H "Content-Type: application/json" \
     -d '{"homeTeam":"Mexico","awayTeam":"Spain","scheduledAt":"2026-08-04T15:00:00Z","location":"Different Venue"}'
   ```
   Expect `400 Bad Request` (Mexico already in-progress) with body
   `{"error_code":"start_rejected","error_message":"..."}`.

3. **Get a match** (US1, Acceptance Scenarios 3-4):
   ```bash
   curl -i http://localhost:8080/matches/1
   curl -i http://localhost:8080/matches/9999
   ```
   Expect `200 OK` then `404 Not Found` with body
   `{"error_code":"match_not_found","error_message":"..."}`.

4. **Update a score** (US2):
   ```bash
   curl -i -X PUT http://localhost:8080/matches/1/score \
     -H "Content-Type: application/json" -d '{"homeScore":2,"awayScore":1}'
   curl -i -X PUT http://localhost:8080/matches/1/score \
     -H "Content-Type: application/json" -d '{"homeScore":1,"awayScore":1}'
   ```
   Expect `200 OK` then `400 Bad Request` (decrease rejected) with body
   `{"error_code":"invalid_score","error_message":"..."}`.

5. **Finish a match** (US3):
   ```bash
   curl -i -X POST http://localhost:8080/matches/1/finish
   curl -i -X POST http://localhost:8080/matches/1/finish
   ```
   Expect `200 OK` then `404 Not Found` (already finished) with body
   `{"error_code":"match_not_found","error_message":"..."}`.

6. **Live summary** (US4):
   ```bash
   curl -i http://localhost:8080/matches/summary
   ```
   Expect `200 OK` with matches ordered by total score descending.

7. **History** (US5):
   ```bash
   curl -i "http://localhost:8080/matches/history?page=1"
   curl -i "http://localhost:8080/matches/history?page=0"
   ```
   Expect `200 OK` then `400 Bad Request` (invalid page) with body
   `{"error_code":"invalid_page","error_message":"..."}`.

## Expected outcome

`dotnet test` reports all `WorldCupScoreboard.Api.Tests` passing (plus the full library suite
still green), and the manual steps above behave as described whether run locally or via Docker.
