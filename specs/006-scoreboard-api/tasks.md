---

description: "Task list for 006-scoreboard-api"
---

# Tasks: Scoreboard API

**Input**: Design documents from `/specs/006-scoreboard-api/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api.md, quickstart.md

**Tests**: Included — constitution Principle I (Test-First, NON-NEGOTIABLE) mandates a failing
test before every production-code change; here that means an integration test against the real
HTTP endpoint before each endpoint is mapped, per constitution Principle IV.

**Organization**: Tasks are grouped by user story (spec.md: US1-US5, mirroring `001`-`005`'s own
priority order) to enable independent implementation and testing of each endpoint group.

**Revision note**: this file was regenerated after the error-response design changed per
explicit user instruction — every rejection now returns a structured `error_code`/
`error_message` body (not `ProblemDetails`), produced via `OneOf<...>` discriminated unions
(the `OneOf` NuGet package) instead of nullable returns/`try`/`catch` (research.md §6-§8). This
also resolves `/speckit-analyze`'s finding I1 (an earlier task's ambiguous
`TypedResults.NotFound()`/`Problem(...)` phrasing, which risked an empty-body 404) — the new
design has no such ambiguity, since every rejection path now goes through one shared mapping
helper.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on an incomplete task)
- **[Story]**: Which user story this task belongs to (US1-US5)
- File paths are exact, per plan.md's Project Structure

## Phase 1: Setup

**Purpose**: Create the two new projects, their package references, and the Docker scaffolding
— nothing under `src/WorldCupScoreboard.Api/` or `tests/WorldCupScoreboard.Api.Tests/` exists
yet.

- [X] T001 Create `src/WorldCupScoreboard.Api/WorldCupScoreboard.Api.csproj` (net9.0 ASP.NET
  Core web project) and add it to `WorldCupScoreboard.sln`; add a `ProjectReference` to
  `src/WorldCupScoreboard/WorldCupScoreboard.csproj`.
- [X] T002 [P] Add `Swashbuckle.AspNetCore` (pinned version compatible with net9.0) to
  `WorldCupScoreboard.Api.csproj` (research.md §2).
- [X] T003 [P] Add `OneOf` (pinned version compatible with net9.0) to
  `WorldCupScoreboard.Api.csproj` (research.md §8, per explicit user instruction).
- [X] T004 Create `tests/WorldCupScoreboard.Api.Tests/WorldCupScoreboard.Api.Tests.csproj`
  (net9.0, xUnit — mirror `WorldCupScoreboard.Tests.csproj`'s package list) and add it to
  `WorldCupScoreboard.sln`; add `ProjectReference`s to `WorldCupScoreboard.Api` AND
  `tests/WorldCupScoreboard.Tests/WorldCupScoreboard.Tests.csproj` (to reuse
  `InMemoryMatchRepository`, plan.md — Testing).
- [X] T005 [P] Add `Microsoft.AspNetCore.Mvc.Testing` (pinned version compatible with net9.0) to
  `WorldCupScoreboard.Api.Tests.csproj`.
- [X] T006 [P] Create `Dockerfile` at the repo root — multi-stage build: an SDK stage that
  restores/publishes `src/WorldCupScoreboard.Api`, and an ASP.NET runtime stage that copies the
  published output and sets the entrypoint (plan.md's Project Structure).
- [X] T007 [P] Create `.dockerignore` at the repo root (`bin/`, `obj/`, `tests/`, `specs/`,
  `.git/`, `*.db`, etc.).

**Checkpoint**: `dotnet build` succeeds on the whole solution (including the two new, still
near-empty projects) before any endpoint code is added.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The request/error DTOs, the shared error-to-HTTP-response mapping, the
DI-wired-but-endpoint-less `Program.cs`, and the test factory every user story's endpoint work
depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T008 [P] Create `StartMatchRequest` (`HomeTeam`, `AwayTeam`, `ScheduledAt`, `Location`) in
  `src/WorldCupScoreboard.Api/Contracts/StartMatchRequest.cs` (per data-model.md).
- [X] T009 [P] Create `UpdateScoreRequest` (`HomeScore`, `AwayScore`) in
  `src/WorldCupScoreboard.Api/Contracts/UpdateScoreRequest.cs` (per data-model.md).
- [X] T010 [P] Create `ErrorResponse` (`ErrorCode`, `ErrorMessage`, each with
  `[JsonPropertyName("error_code")]`/`[JsonPropertyName("error_message")]`) in
  `src/WorldCupScoreboard.Api/Contracts/ErrorResponse.cs` (data-model.md — the snake_case JSON
  names are explicit per user instruction, independent of the API's default camelCase
  convention elsewhere).
- [X] T011 [P] Create the shared `IApiError` interface (`string ErrorCode { get; }`,
  `string ErrorMessage { get; }`) in `src/WorldCupScoreboard.Api/Contracts/IApiError.cs`
  (data-model.md).
- [X] T012 [P] Create `StartRejectedError` (implements `IApiError`, `ErrorCode =
  "start_rejected"`, fixed generic message) in
  `src/WorldCupScoreboard.Api/Contracts/StartRejectedError.cs`. Depends on T011.
- [X] T013 [P] Create `MatchNotFoundError` (implements `IApiError`, `ErrorCode =
  "match_not_found"`, carries the requested match ID to build `ErrorMessage`) in
  `src/WorldCupScoreboard.Api/Contracts/MatchNotFoundError.cs` — reused by 3 endpoints
  (data-model.md). Depends on T011.
- [X] T014 [P] Create `InvalidScoreError` (implements `IApiError`, `ErrorCode =
  "invalid_score"`, carries team name/attempted/current score to build `ErrorMessage`,
  mirroring `InvalidScoreException`'s own fields) in
  `src/WorldCupScoreboard.Api/Contracts/InvalidScoreError.cs`. Depends on T011.
- [X] T015 [P] Create `InvalidPageError` (implements `IApiError`, `ErrorCode = "invalid_page"`,
  carries the requested page number to build `ErrorMessage`) in
  `src/WorldCupScoreboard.Api/Contracts/InvalidPageError.cs`. Depends on T011.
- [X] T016 Create the shared error-to-HTTP-response mapping — a `ToHttpResult(this IApiError
  error, int statusCode)` extension method (or equivalent static helper) in
  `src/WorldCupScoreboard.Api/Contracts/ApiErrorExtensions.cs` that builds an `ErrorResponse`
  from any `IApiError` and returns `TypedResults.Json(errorResponse, statusCode: statusCode)` —
  written once so no endpoint duplicates this mapping (research.md §8; resolves
  `/speckit-analyze` finding I1). Depends on T010, T011.
- [X] T017 Write `src/WorldCupScoreboard.Api/Program.cs` with no endpoints yet: register
  `ScoreboardDbContext` (SQLite, connection string from configuration, default
  `Data Source=scoreboard-api.db` — research.md, Storage), register `IMatchRepository` →
  `SqliteMatchRepository`, register `IScoreboard`/`Scoreboard` as a **singleton** (research.md,
  Constraints), add `AddEndpointsApiExplorer()`/`AddSwaggerGen()`, map
  `UseSwagger()`/`UseSwaggerUI()`, call `dbContext.Database.Migrate()` on startup (mirroring
  `demo/ScoreboardCli`'s existing pattern), and end the file with `public partial class Program
  { }` (research.md §9). Depends on T001-T003.
- [X] T018 Create `tests/WorldCupScoreboard.Api.Tests/ScoreboardApiFactory.cs`: a
  `WebApplicationFactory<Program>` subclass that overrides service registration (via
  `WithWebHostBuilder`) to remove the real `IMatchRepository`/`ScoreboardDbContext`
  registrations and register `InMemoryMatchRepository` (from `WorldCupScoreboard.Tests.Fakes`)
  instead — no real SQLite database in any test run. Depends on T004-T005, T017.
- [X] T019 `appsettings.json` in `src/WorldCupScoreboard.Api/`: add a `ConnectionStrings`
  section with the default `scoreboard-api.db` connection string (data-model.md; research.md,
  Storage). Depends on T017.

**Checkpoint**: The API project builds and starts (with zero endpoints — a 404 for everything,
which is fine), Swagger UI is reachable, the shared error-mapping helper exists, and the test
factory can spin up an in-memory-backed instance of it. User story work can now begin.

---

## Phase 3: User Story 1 - Start and retrieve a match over HTTP (Priority: P1) 🎯 MVP

**Goal**: `POST /matches` and `GET /matches/{id}` work end-to-end, mapping `StartMatch`/
`GetMatch`'s existing behavior to HTTP per contracts/api.md, using `OneOf<...>` for the
discriminated success/failure result.

**Independent Test**: Via the test factory's `HttpClient`, `POST /matches` with valid input and
confirm `201 Created` with the match in the body; `POST` again with conflicting input and
confirm `400 Bad Request` with `error_code: "start_rejected"`; `GET /matches/{id}` for the
created match and confirm `200 OK` matching data; `GET /matches/9999` and confirm `404 Not
Found` with `error_code: "match_not_found"`.

### Tests for User Story 1

> **Write these tests FIRST — confirm they FAIL (404 with no body, since no endpoint exists
> yet) before writing implementation (T022-T023)**

- [X] T020 [P] [US1] Write failing tests for `POST /matches` (FR-001, FR-002, FR-008;
  Acceptance Scenarios 1-2 — success returns `201 Created` with the match body, rejection
  returns `400 Bad Request` with `{ "error_code": "start_rejected", "error_message": "..." }`)
  in `tests/WorldCupScoreboard.Api.Tests/StartMatchEndpointTests.cs`, using
  `ScoreboardApiFactory`.
- [X] T021 [P] [US1] Write failing tests for `GET /matches/{id}` (FR-003, FR-008; Acceptance
  Scenarios 3-4 — found returns `200 OK` with matching data, unknown ID returns `404 Not
  Found` with `error_code: "match_not_found"`) in
  `tests/WorldCupScoreboard.Api.Tests/GetMatchEndpointTests.cs`.

### Implementation for User Story 1

- [X] T022 [US1] Map `POST /matches` in `src/WorldCupScoreboard.Api/Program.cs`: bind
  `StartMatchRequest`, call `scoreboard.StartMatch(...)`, wrap the nullable result as
  `OneOf<Match, StartRejectedError>`, then `.Match(match => TypedResults.Created(...), error =>
  error.ToHttpResult(400))` (contracts/api.md, research.md §8). Depends on T008, T012, T016,
  T017.
- [X] T023 [US1] Map `GET /matches/{id}` in `Program.cs`: call `scoreboard.GetMatch(id)`, wrap
  as `OneOf<Match, MatchNotFoundError>`, then `.Match(match => TypedResults.Ok(match), error =>
  error.ToHttpResult(404))` (contracts/api.md). Same file as T022 — sequential, not parallel.
  Depends on T013, T016, T017.
- [X] T024 [US1] Run `dotnet test --filter FullyQualifiedName~StartMatchEndpoint|FullyQualifiedName~GetMatchEndpoint`;
  confirm T020-T021 all pass. Then run the full suite (`dotnet test`) to confirm no regression
  in the library's own test suite. On any failure, apply constitution Principle II (reproduce →
  state the fix in one sentence → minimal fix → re-run the FULL suite) before proceeding.

**Checkpoint**: User Story 1 is complete, independently functional, and fully tested — this is
the MVP.

---

## Phase 4: User Story 2 - Update a match's score over HTTP (Priority: P2)

**Goal**: `PUT /matches/{id}/score` works end-to-end, mapping `UpdateScore`'s existing behavior
(including its two distinct rejection types) to a three-case `OneOf<...>`.

**Independent Test**: Start a match via the API, `PUT` a valid higher score and confirm `200
OK`; `PUT` a decrease or a negative score and confirm `400 Bad Request` with `error_code:
"invalid_score"` and the score unchanged; `PUT` against a nonexistent/finished match ID and
confirm `404 Not Found` with `error_code: "match_not_found"`.

### Tests for User Story 2

> **Write these tests FIRST — confirm they FAIL before writing implementation (T026)**

- [X] T025 [P] [US2] Write failing tests for `PUT /matches/{id}/score` (FR-004, FR-008;
  Acceptance Scenarios 1-3 — success, `invalid_score` rejection, `match_not_found` rejection)
  in `tests/WorldCupScoreboard.Api.Tests/UpdateScoreEndpointTests.cs`.

### Implementation for User Story 2

- [X] T026 [US2] Map `PUT /matches/{id}/score` in `Program.cs`: bind `UpdateScoreRequest`, call
  `scoreboard.UpdateScore(...)`, catch `MatchNotFoundException`/`InvalidScoreException` and
  convert each to the matching `IApiError`, modeling the result as `OneOf<Match,
  MatchNotFoundError, InvalidScoreError>`, then `.Match(match => TypedResults.Ok(match),
  notFound => notFound.ToHttpResult(404), invalidScore => invalidScore.ToHttpResult(400))`
  (contracts/api.md, research.md §8). Same file as T022-T023 — sequential. Depends on T009,
  T013, T014, T016, T017.
- [X] T027 [US2] Run `dotnet test --filter FullyQualifiedName~UpdateScoreEndpoint`; confirm
  T025 passes. Then run the full suite to confirm no regression in User Story 1.

**Checkpoint**: User Stories 1-2 are both independently functional.

---

## Phase 5: User Story 3 - Finish a match over HTTP (Priority: P3)

**Goal**: `POST /matches/{id}/finish` works end-to-end, mapping `FinishMatch`'s existing
one-way-transition behavior to a two-case `OneOf<...>`.

**Independent Test**: Start a match via the API, `POST .../finish` and confirm `200 OK` with
status finished; `POST .../finish` again and confirm `404 Not Found` with `error_code:
"match_not_found"`; `GET` the match afterward and confirm its data is still present.

### Tests for User Story 3

> **Write these tests FIRST — confirm they FAIL before writing implementation (T029)**

- [X] T028 [P] [US3] Write failing tests for `POST /matches/{id}/finish` (FR-005, FR-008;
  Acceptance Scenarios 1-2 — success, and `match_not_found` rejection for
  already-finished/nonexistent) in
  `tests/WorldCupScoreboard.Api.Tests/FinishMatchEndpointTests.cs`.

### Implementation for User Story 3

- [X] T029 [US3] Map `POST /matches/{id}/finish` in `Program.cs`: call
  `scoreboard.FinishMatch(id)`, catch `MatchNotFoundException` → `MatchNotFoundError`, model as
  `OneOf<Match, MatchNotFoundError>`, then `.Match(match => TypedResults.Ok(match), error =>
  error.ToHttpResult(404))` (contracts/api.md). Same file as T022-T023/T026 — sequential.
  Depends on T013, T016, T017.
- [X] T030 [US3] Run `dotnet test --filter FullyQualifiedName~FinishMatchEndpoint`; confirm
  T028 passes. Then run the full suite to confirm no regression in User Stories 1-2.

**Checkpoint**: User Stories 1-3 are all independently functional.

---

## Phase 6: User Story 4 - View the live summary over HTTP (Priority: P4)

**Goal**: `GET /matches/summary` works end-to-end, mapping `GetSummary`'s existing ordering
guarantee to HTTP. No `OneOf` needed here — this endpoint has no failure case.

**Independent Test**: Start and update several matches via the API, `GET /matches/summary`,
and confirm the returned order matches total-score-descending with
most-recently-started-first on ties; confirm an empty list (not an error) when nothing is
in-progress.

### Tests for User Story 4

> **Write these tests FIRST — confirm they FAIL before writing implementation (T032)**

- [X] T031 [P] [US4] Write failing tests for `GET /matches/summary` (FR-006; Acceptance
  Scenarios 1-2 — correct order, empty-but-successful when nothing is in-progress) in
  `tests/WorldCupScoreboard.Api.Tests/GetSummaryEndpointTests.cs`.

### Implementation for User Story 4

- [X] T032 [US4] Map `GET /matches/summary` in `Program.cs`: call `scoreboard.GetSummary()`,
  return `TypedResults.Ok(matches)` directly — no `OneOf` needed, this operation never rejects
  (contracts/api.md). Same file as T022-T023/T026/T029 — sequential. Depends on T017.
- [X] T033 [US4] Run `dotnet test --filter FullyQualifiedName~GetSummaryEndpoint`; confirm T031
  passes. Then run the full suite to confirm no regression in User Stories 1-3.

**Checkpoint**: User Stories 1-4 are all independently functional.

---

## Phase 7: User Story 5 - Browse match history over HTTP (Priority: P5)

**Goal**: `GET /matches/history?page={page}` works end-to-end, mapping `GetHistory`'s existing
pagination/ordering guarantee to a two-case `OneOf<...>`.

**Independent Test**: Start more matches via the API than fit on one page, `GET
.../history?page=1` and `page=2`, and confirm the results match the library's pagination;
`GET .../history?page=0` and confirm `400 Bad Request` with `error_code: "invalid_page"`.

### Tests for User Story 5

> **Write these tests FIRST — confirm they FAIL before writing implementation (T035)**

- [X] T034 [P] [US5] Write failing tests for `GET /matches/history` (FR-007, FR-008; Acceptance
  Scenarios 1-3 — correct page contents, empty-but-successful for an out-of-range page, `400
  Bad Request` with `error_code: "invalid_page"` for an invalid page number) in
  `tests/WorldCupScoreboard.Api.Tests/GetHistoryEndpointTests.cs`.

### Implementation for User Story 5

- [X] T035 [US5] Map `GET /matches/history` in `Program.cs`: bind `page` from the query string,
  call `scoreboard.GetHistory(page)`, catch `InvalidPageException` → `InvalidPageError`, model
  as `OneOf<Match[], InvalidPageError>`, then `.Match(matches => TypedResults.Ok(matches), error
  => error.ToHttpResult(400))` (contracts/api.md). Same file as
  T022-T023/T026/T029/T032 — sequential. Depends on T015, T016, T017.
- [X] T036 [US5] Run `dotnet test --filter FullyQualifiedName~GetHistoryEndpoint`; confirm T034
  passes. Then run the full suite (`dotnet test`) to confirm no regression anywhere — this is
  the last endpoint, so this run should be the first fully-green run across every test project.

**Checkpoint**: All five user stories are complete — the whole `IScoreboard` contract is now
reachable over HTTP, with a consistent `error_code`/`error_message` shape for every rejection.

---

## Phase 8: Polish & Cross-Cutting Concerns

- [X] T037 Manually verify Swagger UI (`/swagger`) lists all six endpoints, that each can be
  invoked directly from the browser (spec.md FR-009), and that error responses visibly show
  `error_code`/`error_message` fields in the Swagger UI's example/response panel — run `dotnet
  run --project src/WorldCupScoreboard.Api` and check.
- [X] T038 Build and run the Docker image (`docker build -t scoreboard-api .` /
  `docker run -p 8080:8080 scoreboard-api`); confirm the container starts, migrates its own
  SQLite database on first run, and Swagger UI is reachable at the containerized port
  (spec.md FR-010).
- [X] T039 [P] Run `dotnet format` (or verify existing formatting) across
  `src/WorldCupScoreboard.Api/` and `tests/WorldCupScoreboard.Api.Tests/`.
- [X] T040 Walk through `specs/006-scoreboard-api/quickstart.md`'s full manual validation (the
  `curl`/Swagger steps for all 5 user stories) against both the local (`dotnet run`) and
  Dockerized instances, confirming every acceptance scenario — including the exact
  `error_code`/`error_message` values — holds end-to-end.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: T001 before T002-T003 (same project); T004 before T005 (same project,
  and T004 itself depends on T001 existing for its `ProjectReference` to resolve); T006/T007
  independent of the rest.
- **Foundational (Phase 2)**: Depends on Setup. T008-T015 (DTOs/error types) are all
  independent of each other except T012-T015 each depending on T011 (the shared interface);
  T016 depends on T010 and T011; T017 depends on T001-T003; T018 depends on T004-T005 and
  T017; T019 depends on T017. BLOCKS every user story.
- **User Stories (Phases 3-7)**: Each depends on Foundational only, but all five stories'
  implementation tasks (T022-T023, T026, T029, T032, T035) edit the **same** `Program.cs`
  file, so — as in the original plan — the five stories are not fully parallel-implementable
  even though they're independently testable; the natural execution order is
  P1→P2→P3→P4→P5, one story fully done before the next story's implementation task lands
  (tests can still be written earlier, in parallel, per story).
- **Polish (Phase 8)**: Depends on all five user stories being complete.

### Within Each User Story

- That story's test task(s) MUST be written and FAIL (as a 404 with no body, since the route
  doesn't exist yet) before its implementation task.
- Implementation tasks across stories are strictly sequential (same file, `Program.cs`).

### Parallel Opportunities

- T002 and T003 (Setup, same project but different package references) can run in parallel;
  T005 depends on T004 only; T006/T007 (Docker files) are independent of everything else.
- T008-T015 (Foundational DTOs/error types, 8 different files) can mostly run in parallel —
  T012-T015 each need T011 first, but are independent of each other and of T008-T010.
- **All five user stories' test-writing tasks (T020, T021, T025, T028, T031, T034) can be
  written in parallel with each other** — different files, and each only depends on
  Foundational (specifically T018's factory), not on any other story's implementation landing
  first. Only the *implementation* tasks are forced sequential (same file).
- T039 (Polish) can run in parallel with T037/T038/T040.

---

## Parallel Example: Writing Every Story's Tests Up Front

```bash
# All five stories' tests can be written in parallel once Foundational (T008-T019) is done —
# even though their implementations (T022+) must land one story at a time:
Task: "Write failing tests for POST /matches in tests/WorldCupScoreboard.Api.Tests/StartMatchEndpointTests.cs"
Task: "Write failing tests for GET /matches/{id} in tests/WorldCupScoreboard.Api.Tests/GetMatchEndpointTests.cs"
Task: "Write failing tests for PUT /matches/{id}/score in tests/WorldCupScoreboard.Api.Tests/UpdateScoreEndpointTests.cs"
Task: "Write failing tests for POST /matches/{id}/finish in tests/WorldCupScoreboard.Api.Tests/FinishMatchEndpointTests.cs"
Task: "Write failing tests for GET /matches/summary in tests/WorldCupScoreboard.Api.Tests/GetSummaryEndpointTests.cs"
Task: "Write failing tests for GET /matches/history in tests/WorldCupScoreboard.Api.Tests/GetHistoryEndpointTests.cs"
```

---

## Implementation Strategy

### MVP First — User Story 1

1. Complete Phase 1: Setup (T001-T007)
2. Complete Phase 2: Foundational (T008-T019)
3. Complete Phase 3: User Story 1 (T020-T024)
4. **STOP and VALIDATE**: `dotnet test` green, Swagger UI shows `POST /matches`/`GET
   /matches/{id}` and both work when tried from the browser, error responses show
   `error_code`/`error_message`
5. This is a shippable increment — starting and retrieving a match over HTTP

### Incremental Delivery

1. Setup + Foundational → the API project exists, builds, starts, has Swagger, has the shared
   error-mapping helper, has no routes yet
2. Add User Story 1 → test independently → MVP
3. Add User Story 2 → test independently, confirm no US1 regression
4. Add User Story 3 → test independently, confirm no US1-2 regression
5. Add User Story 4 → test independently, confirm no US1-3 regression
6. Add User Story 5 → test independently, confirm no US1-4 regression — the whole
   `IScoreboard` contract is now reachable over HTTP
7. Polish (Docker verification, formatting, full manual walkthrough)

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions, this feature maps to one or a couple of small,
  reviewable commits — but do not commit without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing as a 404, then
  passing) before moving to the next task — no production code without a preceding failing
  test.
