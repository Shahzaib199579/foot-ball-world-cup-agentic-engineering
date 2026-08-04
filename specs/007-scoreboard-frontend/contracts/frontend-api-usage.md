# Frontend → API Usage Contract

`007-scoreboard-frontend` introduces no new backend endpoints — this documents which of
`006-scoreboard-api`'s already-published endpoints (full contract:
`specs/006-scoreboard-api/contracts/api.md`) each Angular feature/component calls, and which
user story that mapping serves. Request/response shapes are not repeated here — see the linked
contract for the authoritative shapes; this file only maps UI → endpoint.

| Angular feature | Calls | Maps to | User Story |
|---|---|---|---|
| `MatchesComponent` — "Start Match" button | `POST /matches` | `006` FR-001/FR-002 | US2 (success modal: FR-011) |
| `MatchesComponent` — score update inputs | `PUT /matches/{id}/score` | `006` FR-004 | US3 (FR-010, success modal: FR-011) |
| `MatchesComponent` — "Finish Match" button | `POST /matches/{id}/finish` | `006` FR-005 | US3 (FR-010, success modal: FR-011) |
| `SummaryComponent` | `GET /matches/summary` | `006` FR-006 | US1 |
| `HistoryComponent` | `GET /matches/history?page={p}` | `006` FR-007 | US1 |
| `MatchesComponent` — active match list | `GET /matches/{id}` (per-match refresh, if needed) | `006` FR-003 | US2, US3 |
| `ErrorInterceptor` (all of the above) | any non-2xx response | `006` FR-008 (`error_code`/`error_message` body) | US4 |

## Client-side service surface

`ScoreboardService` (Angular `@Injectable`) is the single point of contact with the API —
no component calls `HttpClient` directly, mirroring the same "one shared layer, not duplicated
per call site" approach `006` used for its own error mapping (research.md §5):

```typescript
interface ScoreboardService {
  startMatch(homeTeam: string, awayTeam: string, scheduledAt: string, location: string): Observable<Match>;
  getMatch(id: number): Observable<Match>;
  updateScore(id: number, homeScore: number, awayScore: number): Observable<Match>;
  finishMatch(id: number): Observable<Match>;
  getSummary(): Observable<Match[]>;
  getHistory(page: number): Observable<Match[]>;
}
```

Every method's error path resolves through the shared `ErrorInterceptor` (research.md §5) —
callers only need to handle the success path; the interceptor handles showing the error
dialog for any rejection. For `startMatch`/`updateScore`/`finishMatch` specifically, the
success path additionally opens the shared `SuccessDialogComponent` (FR-011, research.md
§5a) with an action-specific message before resolving — `getMatch`/`getSummary`/`getHistory`
(read-only fetches) do NOT trigger a success modal, since FR-011 only covers the three
mutating actions the user named (start/update/finish), not every successful API call.

## Refresh-on-navigation contract (FR-006)

Switching to Summary or History MUST re-fetch from the corresponding endpoint above rather than
relying on cached component state, so a match just started/updated/finished on the Matches tab
is reflected immediately (spec.md US1 Acceptance Scenario 2, US2 Acceptance Scenario 2). This is
implemented as a fetch-on-`ngOnInit`/route-activation per feature component — no shared
cross-tab state store is required for this scope (no requirement calls for it).
