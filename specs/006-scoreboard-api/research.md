# Research: Scoreboard API

Phase 0 output for `specs/006-scoreboard-api`. No open `NEEDS CLARIFICATION` markers remained
after `/speckit-clarify` (zero questions asked). The items below are design/pattern decisions
needed to move from spec to data model and contract.

## 1. Minimal API endpoints, not Controllers

- **Decision**: use ASP.NET Core Minimal API (`app.MapPost`/`MapGet`/etc. in `Program.cs`), not
  MVC Controllers.
- **Rationale**: the user's own description said "minimal .NET web API project"; Minimal API is
  the framework's own name for exactly this lightweight style, and six simple endpoints with no
  shared cross-cutting controller logic don't need Controllers' extra ceremony (attribute
  routing, base class, model binding conventions built for larger surfaces).
- **Alternatives considered**: MVC Controllers with `[ApiController]`: rejected as more
  structure than six thin pass-through endpoints need — would add files and indirection without
  a corresponding benefit at this scale.

## 2. Swashbuckle.AspNetCore, not the built-in `Microsoft.AspNetCore.OpenApi`

- **Decision**: add `Swashbuckle.AspNetCore` for both OpenAPI document generation and the
  interactive Swagger UI.
- **Rationale**: spec.md's FR-009 requires *interactive, browsable* documentation a caller can
  invoke endpoints from directly. .NET 9's built-in `Microsoft.AspNetCore.OpenApi` package only
  generates the OpenAPI JSON document — it ships no bundled UI. Swashbuckle.AspNetCore has
  provided both in one well-established package for years and is the most common choice for
  exactly this "browse and try it" requirement.
- **Alternatives considered**: built-in `Microsoft.AspNetCore.OpenApi` + a separately-hosted
  Swagger UI (e.g., via CDN or a second package): rejected — more moving parts for the same
  outcome Swashbuckle already provides as one package.

## 3. `Scoreboard` as a DI singleton

- **Decision**: register `IScoreboard`/`Scoreboard` as a singleton in the API's DI container.
- **Rationale**: `Scoreboard`'s constructor seeds `_nextId` and `_nextActivitySequence` by
  scanning `repository.GetAll()` once (`001-start-match`/`005-match-history`). A singleton means
  this scan happens once per process, matching how `demo/ScoreboardCli` already uses one
  `Scoreboard` for its entire session. `Scoreboard`'s own internal coarse lock already makes it
  safe under ASP.NET Core's concurrent request dispatch — no new synchronization needed.
- **Alternatives considered**: a scoped (per-request) `Scoreboard`: rejected — would re-scan
  `repository.GetAll()` on every single request just to reseed the same counters, which is both
  wasteful and pointless (the counters must be shared across requests to stay monotonic, so a
  per-request instance would actually break correctness, not just waste time).

## 4. Separate SQLite file for the API, not sharing the CLI demo's

- **Decision**: the API uses its own connection string, defaulting to `Data Source=scoreboard-api.db`,
  configurable via `appsettings.json`/environment variable — not `demo/ScoreboardCli`'s
  `scoreboard.db`.
- **Rationale**: the API and the CLI demo are two independently-run, independently-started
  processes. SQLite file-level locking can contend if two separate processes open the same file
  concurrently; there's also no requirement anywhere in spec.md or CLAUDE.md that they must show
  identical data. Keeping them separate avoids an entire class of "why did my CLI session's data
  change while the API was also running" confusion.
- **Alternatives considered**: sharing `scoreboard.db`: rejected for the contention/confusion
  reasons above; a purely in-memory SQLite database for the API's own runtime (not just tests):
  rejected — the API should behave like a real persistent service, matching the CLI demo's own
  persistence expectations, not reset its data every restart.

## 5. Request DTOs for `StartMatch`/`UpdateScore`, not raw `Match` binding

- **Decision**: two small request record types — `StartMatchRequest(string HomeTeam, string
  AwayTeam, DateTime ScheduledAt, string Location)` and `UpdateScoreRequest(int HomeScore, int
  AwayScore)` — bound from the JSON request body. `GetMatch`/`FinishMatch`/`GetHistory` take
  their inputs from the URL (route parameter / query string) directly, needing no body DTO.
  Responses return `Match` directly (no response DTO) — consistent with `004`/`005`'s own
  "don't invent a projection type until something actually needs one" precedent.
- **Rationale**: `Match`'s constructor requires an `Id` and constructed `Team` objects that a
  caller starting a match doesn't have yet — binding the request directly to `Match` isn't
  possible without exposing internal-only shape. A minimal, purpose-built request record is the
  simplest fix, not overengineering.
- **Alternatives considered**: binding directly to positional route/body-tuple parameters with
  no named type (Minimal API supports this for simple cases): rejected for `StartMatch`
  specifically — four positional parameters of similar types (three strings, one `DateTime`) in
  a request body is exactly the kind of thing a named request type prevents mixing up; the
  single- and two-parameter endpoints (`UpdateScore`'s two scores) still benefit from a named
  type for the same reason, if to a lesser degree.

## 6. Status code mapping (FR-002 through FR-007)

| Endpoint | Success | Rejection |
|---|---|---|
| `StartMatch` | `201 Created` (with the new match's location) | `400 Bad Request` — single generic reason (spec.md FR-002; library returns non-throwing `null`, no distinguishable cause) |
| `GetMatch` | `200 OK` | `404 Not Found` — no such match |
| `UpdateScore` | `200 OK` | `404 Not Found` for `MatchNotFoundException`; `400 Bad Request` for `InvalidScoreException` |
| `FinishMatch` | `200 OK` | `404 Not Found` for `MatchNotFoundException` |
| `GetSummary` | `200 OK` (possibly an empty list) | never rejects |
| `GetHistory` | `200 OK` (possibly an empty list) | `400 Bad Request` for `InvalidPageException` |

- **Decision (superseded — see below)**: ~~error responses use ASP.NET Core's built-in
  `ProblemDetails` (RFC 7807) shape~~. **Superseded by explicit user instruction**: every 4xx
  response body is a custom `ErrorResponse` record — `ErrorCode` (string, snake_case, e.g.
  `"match_not_found"`) and `ErrorMessage` (string, human-readable) — serialized with
  `[JsonPropertyName("error_code")]`/`[JsonPropertyName("error_message")]` so the JSON body uses
  those exact snake_case field names regardless of the rest of the API's default camelCase
  System.Text.Json convention (applying the snake_case names only to this DTO, not globally, so
  `Match`'s own success-response serialization is untouched).
- **Rationale**: the user explicitly asked for `error_code`/`error_message` properties by name,
  with concrete example values (`"match_not_found"`), superseding the earlier `ProblemDetails`
  choice. `ProblemDetails` doesn't have a stable, short, machine-readable "reason code" field by
  convention (it has `title`/`detail`, meant as human-readable, not a code enum) — a bespoke
  `ErrorResponse` is what's actually being asked for.
- **Concrete error codes** (spec.md FR-008's table): `start_rejected`, `match_not_found`,
  `invalid_score`, `invalid_page` — one per distinct rejection reason the library itself already
  distinguishes (research.md §6 above), reused across endpoints where the underlying exception
  type is the same (e.g. `match_not_found` for `GetMatch`/`UpdateScore`/`FinishMatch` alike,
  since all three treat a missing/non-in-progress match identically).
- **Alternatives considered**: keeping `ProblemDetails` and adding a custom `extensions` field
  for the code (`ProblemDetails` supports an open-ended `Extensions` dictionary): rejected —
  adds indirection (the code would be nested under `extensions.error_code` by ASP.NET Core's
  default `ProblemDetails` serialization, not top-level `error_code` as explicitly requested).

## 8. `OneOf` for discriminated-union endpoint results, per explicit user instruction

- **Decision**: each endpoint handler that can fail returns `OneOf<Match, TError1, ...>` (the
  `OneOf` NuGet package) instead of catching exceptions or checking for `null` inline. A shared
  `IApiError` interface (`ErrorCode`, `ErrorMessage` properties) is implemented by small,
  per-case error records — `MatchNotFoundError`, `InvalidScoreError`, `InvalidPageError`,
  `StartRejectedError` — so a single shared helper can convert *any* `IApiError` into the
  `ErrorResponse` JSON body + matching status code, without duplicating that mapping logic once
  per endpoint.
  - `StartMatch` handler: `OneOf<Match, StartRejectedError>` → 201 / 400.
  - `GetMatch` handler: `OneOf<Match, MatchNotFoundError>` → 200 / 404.
  - `UpdateScore` handler: `OneOf<Match, MatchNotFoundError, InvalidScoreError>` → 200 / 404 / 400.
  - `FinishMatch` handler: `OneOf<Match, MatchNotFoundError>` → 200 / 404.
  - `GetSummary` handler: plain `Match[]` — no failure case exists for this operation.
  - `GetHistory` handler: `OneOf<Match[], InvalidPageError>` → 200 / 400.
- **Rationale**: the user explicitly asked for "One of package and discriminated union to
  handle those cases" — `OneOf<T0, T1, ...>` is exactly a discriminated union over a closed set
  of named cases, and its `.Match(...)`/`.Switch(...)` methods force every case to be handled
  (compiler-checked exhaustiveness), which is a meaningfully different, stronger guarantee than
  `try`/`catch` blocks scattered per endpoint (nothing forces every `catch` to be present; a
  missed exception type would silently 500 instead of returning the intended 4xx). The
  business logic that *produces* each outcome still lives entirely in the library
  (`WorldCupScoreboard`) — this only changes how the API's own thin mapping layer represents
  "call the library, then produce the right HTTP response" (constitution Principle IV
  unaffected: no business logic moves into the API).
- **Alternatives considered**: keep the `try`/`catch` approach from the original plan (still
  functionally correct, and what most of `002`-`005`'s own exception-based library design would
  suggest mirroring 1:1): superseded by the explicit instruction favoring a discriminated-union
  style specifically; a hand-rolled discriminated union (an abstract base class + sealed
  subclasses, no external package) instead of the `OneOf` package: rejected — `OneOf` is exactly
  what was asked for by name, is a small, well-established, single-purpose NuGet package (no
  heavier dependency than Swashbuckle already is), and avoids reinventing pattern-matching
  boilerplate the package already provides.

## 9. `WebApplicationFactory<Program>` requires `Program` to be accessible

- **Decision**: end `Program.cs` with `public partial class Program { }`.
- **Rationale**: top-level statement `Program.cs` files compile to an `internal partial class
  Program` by default; `WebApplicationFactory<TEntryPoint>` (used by the new test project) needs
  `TEntryPoint` to be visible from the test assembly. Adding one explicit `public partial class
  Program { }` line is the standard, minimal fix — well-documented ASP.NET Core testing pattern,
  not a workaround specific to this project.
- **Alternatives considered**: `[assembly: InternalsVisibleTo("WorldCupScoreboard.Api.Tests")]`:
  rejected — works, but is one more thing to keep in sync if the test project is ever renamed;
  the `public partial class Program` line has no such coupling.
