# Data Model: Scoreboard API

Phase 1 output for `specs/006-scoreboard-api`. This feature introduces no new business entity
and no persisted field — `Match` (established by `001-start-match`) is returned as-is in every
success response. New this feature: two request-only DTOs, one error-response DTO, one shared
error interface, and four small per-case error types (all pure data/marker types, no business
logic — research.md §6-§8).

## Request DTOs (new, this feature only)

### `StartMatchRequest`

| Field | Type | Notes |
|---|---|---|
| `HomeTeam` | `string` | Passed through unchanged to `IScoreboard.StartMatch`. |
| `AwayTeam` | `string` | Passed through unchanged. |
| `ScheduledAt` | `DateTime` | Passed through unchanged. |
| `Location` | `string` | Passed through unchanged. |

No validation logic lives here — `StartMatch` itself already validates (constitution Principle
IV); this DTO only shapes the incoming JSON body.

### `UpdateScoreRequest`

| Field | Type | Notes |
|---|---|---|
| `HomeScore` | `int` | Passed through unchanged to `IScoreboard.UpdateScore`. |
| `AwayScore` | `int` | Passed through unchanged. |

## Error DTO and per-case error types (new, this feature only)

### `ErrorResponse`

| Field | Type | JSON property name | Notes |
|---|---|---|---|
| `ErrorCode` | `string` | `error_code` | One of `start_rejected`, `match_not_found`, `invalid_score`, `invalid_page` (spec.md FR-008's table). |
| `ErrorMessage` | `string` | `error_message` | Human-readable description. |

Explicit `[JsonPropertyName]` attributes fix the snake_case JSON names regardless of the API's
default camelCase convention elsewhere (research.md §6).

### `IApiError` (shared interface) and its implementations

| Type | Implements | `ErrorCode` | Used by |
|---|---|---|---|
| `StartRejectedError` | `IApiError` | `start_rejected` | Start-match endpoint |
| `MatchNotFoundError` | `IApiError` | `match_not_found` | Get-match, update-score, finish endpoints |
| `InvalidScoreError` | `IApiError` | `invalid_score` | Update-score endpoint |
| `InvalidPageError` | `IApiError` | `invalid_page` | History endpoint |

Each carries whatever detail its case needs to build `ErrorMessage` (e.g. `MatchNotFoundError`
carries the requested match ID; `InvalidScoreError` carries the team name, attempted score, and
current score — mirroring `InvalidScoreException`'s own fields from `002-update-score`). A
single shared mapping step converts any `IApiError` into an `ErrorResponse` plus its matching
HTTP status code (research.md §8) — this mapping is written once, not once per endpoint.

## Response shape

Every success response returns the library's `Match` entity directly (JSON-serialized) or a
JSON array of `Match` for `GetSummary`/`GetHistory` — no response DTO is introduced for success
(research.md §5). Every rejection (4xx) response returns an `ErrorResponse` body (research.md
§6), produced from whichever `IApiError` case the endpoint's `OneOf<...>` result matched
(research.md §8) — not a `ProblemDetails` body (that earlier decision is superseded).

## Endpoint-to-operation mapping (from Functional Requirements)

| Endpoint | Library operation | Request shape | Response shape (success) |
|---|---|---|---|
| Start a match | `StartMatch` | `StartMatchRequest` (body) | `Match` |
| Get a match | `GetMatch` | match ID (route) | `Match` |
| Update a match's score | `UpdateScore` | match ID (route) + `UpdateScoreRequest` (body) | `Match` |
| Finish a match | `FinishMatch` | match ID (route) | `Match` |
| Live summary | `GetSummary` | none | `Match[]` |
| History page | `GetHistory` | page number (query string) | `Match[]` |

No validation rules or state transitions are introduced by this feature — all of that already
lives in the library (`001`-`005`) and is exercised unchanged through these endpoints.
