# Contract: Scoreboard HTTP API

Public HTTP contract for `006-scoreboard-api`. Each endpoint is a thin wrapper over exactly one
`IScoreboard` method (`001`-`005`) — see those features' own contracts for the underlying
business rules; this file only documents the HTTP-level mapping (research.md §6-§8).

Every rejection (4xx) response below returns an `ErrorResponse` body:
`{ "error_code": "...", "error_message": "..." }` (data-model.md) — not `ProblemDetails` (an
earlier decision, superseded per explicit user instruction).

## `POST /matches`

Request body: `StartMatchRequest` (`homeTeam`, `awayTeam`, `scheduledAt`, `location`).

- **201 Created** — `StartMatch` succeeded. Body: the new `Match`. `Location` header points to
  `GET /matches/{id}` for the new match.
- **400 Bad Request** — `StartMatch` returned `null` (any rejection reason — missing/duplicate
  team names, missing location, team conflict, or location/time conflict; the library doesn't
  distinguish which, so neither does this endpoint). Body: `ErrorResponse` with
  `error_code: "start_rejected"`.

## `GET /matches/{id}`

- **200 OK** — `GetMatch` found the match. Body: the `Match`.
- **404 Not Found** — `GetMatch` returned `null`. Body: `ErrorResponse` with
  `error_code: "match_not_found"`.

## `PUT /matches/{id}/score`

Request body: `UpdateScoreRequest` (`homeScore`, `awayScore`).

- **200 OK** — `UpdateScore` succeeded. Body: the updated `Match`.
- **404 Not Found** — `UpdateScore` threw `MatchNotFoundException` (no such in-progress match).
  Body: `ErrorResponse` with `error_code: "match_not_found"`.
- **400 Bad Request** — `UpdateScore` threw `InvalidScoreException` (negative or decreasing
  score). Body: `ErrorResponse` with `error_code: "invalid_score"`, `error_message` built from
  the exception's own team name/attempted/current-score fields.

## `POST /matches/{id}/finish`

- **200 OK** — `FinishMatch` succeeded. Body: the finished `Match`.
- **404 Not Found** — `FinishMatch` threw `MatchNotFoundException` (nonexistent or
  already-finished). Body: `ErrorResponse` with `error_code: "match_not_found"`.

## `GET /matches/summary`

- **200 OK** — always. Body: `Match[]`, ordered exactly as `GetSummary` orders them (possibly
  empty). No rejection case exists for this endpoint.

## `GET /matches/history?page={page}`

- **200 OK** — `page >= 1`. Body: `Match[]`, that page's results (possibly empty if `page` is
  beyond the available data).
- **400 Bad Request** — `GetHistory` threw `InvalidPageException` (`page < 1`). Body:
  `ErrorResponse` with `error_code: "invalid_page"`.

## Implementation shape (research.md §8)

Each handler above that has a rejection case returns `OneOf<Match, ...>` (or `OneOf<Match[],
InvalidPageError>` for history) rather than a nullable return or a `try`/`catch` block — a
discriminated union over the success case and every named failure case for that endpoint,
per explicit user instruction. A single shared step converts whichever `IApiError` case was
returned into the `ErrorResponse` body and status code above; this mapping is written once,
not duplicated per endpoint.

## Notes

- Every response body is JSON.
- `Swagger UI` (served by Swashbuckle.AspNetCore, per research.md §2) documents this exact
  contract interactively and lets a caller invoke every endpoint above directly from a browser
  (spec.md FR-009).
- This is the whole contract for this feature — no further endpoints are planned here.
  `007-scoreboard-frontend` is a separate phase with its own contract (a UI, not an HTTP API).
