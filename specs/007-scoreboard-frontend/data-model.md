# Data Model: Scoreboard Frontend

Phase 1 output for `007-scoreboard-frontend`. These are frontend-only view models — no new
backend entities. They mirror `006-scoreboard-api`'s existing JSON contract
(`specs/006-scoreboard-api/contracts/api.md`, `data-model.md`), not a new source of truth.

## Match

The frontend TypeScript interface `Match` (in `core/models/match.model.ts`), maps 1:1 to the
`Match` JSON body returned by every `006` endpoint.

| Field | Type | Notes |
|---|---|---|
| `id` | `number` | Matches `Match.Id`. |
| `homeTeam` | `Team` | `{ name: string; score: number }`. |
| `awayTeam` | `Team` | `{ name: string; score: number }`. |
| `scheduledAt` | `string` (ISO 8601) | Displayed formatted, not edited. |
| `location` | `string` | Displayed as entered at match start. |
| `status` | `number` (`0` = InProgress, `1` = Finished) | Drives the "In Progress"/"Finished" badge (spec.md Acceptance Scenario US1.3). |
| `activitySequence` | `number` | Not rendered directly; used only for stable list-diffing if needed. |
| `totalScore` | `number` | Present on the API response; not recomputed client-side. |

**Derived state (frontend-only, not part of the API body)**:
- `isInProgress: boolean` — `status === 0`, used to show/hide score-update and finish controls
  in the Matches tab (FR-010).

## CountryOption

Static, frontend-bundled list — not sourced from the API (research.md §6).

| Field | Type | Notes |
|---|---|---|
| `name` | `string` | Matches the string sent as `homeTeam`/`awayTeam` on `POST /matches`. |
| `code` | `string` | ISO-3166 alpha-2, lowercase — used to build the flag URL (`flagcdn.com/{code}.svg`). |

**Validation rule**: dropdown selections MUST come from this fixed list — no free-text team
name entry from the Matches tab, so the API's own "non-empty, matching a real team" burden
never reaches the backend malformed. (The API still separately enforces non-empty/duplicate
rules per `006`'s own contract — this is a UX convenience, not a substitute for that
validation.)

## ApiError

Maps 1:1 to `006`'s `ErrorResponse` body, per `specs/006-scoreboard-api/data-model.md`.

| Field | Type | Notes |
|---|---|---|
| `error_code` | `string` | One of `start_rejected`, `match_not_found`, `invalid_score`, `invalid_page` (006 contracts/api.md). |
| `error_message` | `string` | Rendered verbatim in the `ErrorDialogComponent` (FR-007). |

## State transitions (frontend-observed, not owned)

The frontend never mutates match state directly — it only reflects state transitions the
backend already performs and returns:

```
(none) --POST /matches--> InProgress --PUT .../score--> InProgress (score updated)
                                     --POST .../finish--> Finished
```

Any rejected transition (duplicate team, negative/decreasing score, unknown/already-finished
match ID) leaves the frontend's local state unchanged and surfaces the returned `ApiError` via
the error dialog (FR-007) — no optimistic update is applied before the API confirms success.
