# Implementation Plan: Scoreboard API

**Branch**: `006-scoreboard-api` | **Date**: 2026-08-04 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/006-scoreboard-api/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add a new ASP.NET Core Minimal API project (`src/WorldCupScoreboard.Api/`) that wraps
`IScoreboard`'s existing six operations behind six HTTP endpoints, with Swagger/OpenAPI for
interactive exploration, a `Dockerfile` for containerized startup, and integration tests
against real HTTP endpoints (via `WebApplicationFactory`, swapping in the existing
`InMemoryMatchRepository` test fake — not a real database). All business logic stays in the
library per constitution Principle IV; this project is a thin request/response and
status-code-mapping layer only. Per explicit user instruction, every rejection response
returns a structured `error_code`/`error_message` body (not `ProblemDetails`), produced by
representing each endpoint's outcome as an `OneOf<...>` discriminated union over its success
case and every named failure case, rather than nullable returns/`try`/`catch` (research.md
§6-§8).

## Technical Context

**Language/Version**: C# / .NET 9 (net9.0) — unchanged.

**Primary Dependencies**: `Swashbuckle.AspNetCore` (Swagger document generation *and* the
interactive Swagger UI in one package — chosen over the newer built-in
`Microsoft.AspNetCore.OpenApi`, which generates the OpenAPI JSON but ships no bundled UI without
extra work, and the spec explicitly requires an interactive, browsable UI, not just a document).
`Microsoft.AspNetCore.Mvc.Testing` for `WebApplicationFactory`-based integration tests. **`OneOf`**
(NuGet package) — per explicit user instruction, each endpoint handler that can fail returns a
discriminated union (`OneOf<Match, ...>`) over its success case and every named failure case,
instead of nullable returns/`try`/`catch` (research.md §8), mapped to the `error_code`/
`error_message` `ErrorResponse` body FR-008 now requires (research.md §6, superseding this
plan's earlier `ProblemDetails` choice). Reuses `001-start-match`'s `IMatchRepository`/EF
Core/SQLite stack and `002`/`003`/`005`'s exception types via a project reference to
`WorldCupScoreboard`.

**Storage**: SQLite via Entity Framework Core — the **same** `ScoreboardDbContext`/migrations
already used by `demo/ScoreboardCli`, but pointed at its **own** database file
(`scoreboard-api.db`, configurable via `ConnectionStrings:Scoreboard` in `appsettings.json` or
an environment variable), not the CLI demo's `scoreboard.db`. Rationale: the API and the CLI
are two independently-run processes; sharing one SQLite file risks file-lock contention between
them, and there's no requirement that they show the same data. No new migration is needed —
this feature adds no new persisted field.

**Testing**: xUnit + `Microsoft.AspNetCore.Mvc.Testing`'s `WebApplicationFactory<Program>`,
per constitution Principle IV ("API via integration tests against real HTTP endpoints"). The
test project references `WorldCupScoreboard.Tests` directly to reuse its existing
`InMemoryMatchRepository` fake (swapped in via `WithWebHostBuilder`'s service-replacement hook)
rather than duplicating it or hitting a real database.

**Target Platform**: Linux container (via the new `Dockerfile`) and cross-platform .NET 9 for
local development, unchanged runtime otherwise.

**Project Type**: Web service — Phase 2, per constitution Principle IV ("thin transport
adapter with no business logic of its own").

**Performance Goals**: None specified, unchanged rationale from prior features.

**Constraints**: `Scoreboard` is registered in DI as a **singleton** — its constructor already
seeds monotonic counters (`_nextId`, `_nextActivitySequence`) from existing repository data
once; a singleton means that seeding happens once per process (matching how
`demo/ScoreboardCli` already uses one long-lived `Scoreboard` instance for its whole session),
and `Scoreboard`'s own internal coarse lock (from `001-start-match`) already makes it safe for
ASP.NET Core's concurrent request handling without any new synchronization in this feature.

**Scale/Scope**: Unchanged from prior features — six endpoints, no new business rules.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Result |
|---|---|---|
| I. Test-First (NON-NEGOTIABLE) | Every endpoint must have a preceding failing integration test before implementation | **PASS** — enforced at `/speckit-tasks`/`/speckit-implement` time |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | Process gate for handling test failures/bugs during implementation | **PASS** — not a design-time blocker |
| III. Single-Concern Features | One feature, one concern (expose the library over HTTP) | **PASS** — no business logic added; spec.md's Assumptions already justify covering all 6 endpoints as one feature per CLAUDE.md's Roadmap |
| IV. Layered Architecture / Library-First | The API MUST be a thin transport adapter with no business logic of its own; library via unit tests, API via integration tests against real HTTP endpoints | **PASS** — every endpoint handler does request-shape validation + a single `IScoreboard` call + status-code mapping only; all business rules remain in `WorldCupScoreboard` |
| V. Runnable Local Verification | Every feature must be manually exercisable, not only via automated tests | **PASS (by spirit, not the literal CLI)** — Principle V's text names `demo/ScoreboardCli` specifically (a Phase 1 concept); for this Phase 2 API, Swagger UI (FR-009) is the manual-verification surface instead — same spirit (exercisable without writing a client), different mechanism appropriate to a web service |

No violations. Complexity Tracking table below is not applicable.

**Post-Phase-1 re-check**: `data-model.md`, `contracts/api.md`, and `quickstart.md` introduce no
new business logic, no new persisted field, and no new *exception* type — the four new
`IApiError` implementations are pure data/marker records consumed only by this project's own
`OneOf<...>`-to-HTTP-response mapping, not new business rules (those still live entirely in
`WorldCupScoreboard`). All five gates above still **PASS** unchanged.

## Project Structure

### Documentation (this feature)

```text
specs/006-scoreboard-api/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
│   └── api.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
Dockerfile                                    # NEW — multi-stage build (SDK build+publish,
                                               # ASP.NET runtime image), repo root so it can
                                               # COPY the whole solution context
.dockerignore                                 # NEW — bin/, obj/, tests/, specs/, etc.

src/WorldCupScoreboard.Api/                   # NEW project
├── WorldCupScoreboard.Api.csproj             # References WorldCupScoreboard (the library)
├── Program.cs                                # Minimal API endpoint mappings, DI registration
│                                              # (Scoreboard as singleton, DbContext, Swagger),
│                                              # Database.Migrate() on startup — ends with
│                                              # `public partial class Program { }` so
│                                              # WebApplicationFactory<Program> can see it
├── appsettings.json                          # ConnectionStrings:Scoreboard, defaults to
│                                              # "Data Source=scoreboard-api.db"
└── Contracts/                                # Request/error DTOs + error types (pure data)
    ├── StartMatchRequest.cs                  # HomeTeam, AwayTeam, ScheduledAt, Location
    ├── UpdateScoreRequest.cs                 # HomeScore, AwayScore
    ├── ErrorResponse.cs                      # ErrorCode, ErrorMessage ([JsonPropertyName]
    │                                          # "error_code"/"error_message" — research.md §6)
    ├── IApiError.cs                          # Shared interface: ErrorCode, ErrorMessage
    ├── StartRejectedError.cs                 # IApiError — StartMatch's generic rejection
    ├── MatchNotFoundError.cs                 # IApiError — reused by 3 endpoints
    ├── InvalidScoreError.cs                  # IApiError
    ├── InvalidPageError.cs                   # IApiError
    └── ApiErrorExtensions.cs                 # Shared ToHttpResult(this IApiError, int) helper
                                                # — the error→response mapping, written once
                                                # (research.md §8)

tests/WorldCupScoreboard.Api.Tests/           # NEW project
├── WorldCupScoreboard.Api.Tests.csproj       # References WorldCupScoreboard.Api AND
│                                              # WorldCupScoreboard.Tests (to reuse
│                                              # InMemoryMatchRepository)
├── ScoreboardApiFactory.cs                   # WebApplicationFactory<Program> subclass that
│                                              # swaps IMatchRepository for
│                                              # InMemoryMatchRepository
├── StartMatchEndpointTests.cs                # FR-001, FR-002; US1 Acceptance Scenarios 1-2
├── GetMatchEndpointTests.cs                  # FR-003; US1 Acceptance Scenarios 3-4
├── UpdateScoreEndpointTests.cs               # FR-004; US2 Acceptance Scenarios 1-3
├── FinishMatchEndpointTests.cs                # FR-005; US3 Acceptance Scenarios 1-2
├── GetSummaryEndpointTests.cs                 # FR-006; US4 Acceptance Scenarios 1-2
└── GetHistoryEndpointTests.cs                 # FR-007; US5 Acceptance Scenarios 1-3
```

**Structure Decision**: New sibling project `src/WorldCupScoreboard.Api/` next to
`src/WorldCupScoreboard/`, matching this repo's existing `<Project>.<Project>.Tests` naming
convention (`WorldCupScoreboard.Api.Tests` mirrors `WorldCupScoreboard.Tests`). Both new
projects are added to `WorldCupScoreboard.sln`. `Dockerfile`/`.dockerignore` live at the repo
root (standard practice, needed to `COPY` the whole solution during a multi-stage build) — not
inside `src/WorldCupScoreboard.Api/`. CLAUDE.md's target repo layout table (Phase 1 only) is
extended by this feature's own tasks, not redefined.

## Complexity Tracking

> No Constitution Check violations — this section is not applicable.
