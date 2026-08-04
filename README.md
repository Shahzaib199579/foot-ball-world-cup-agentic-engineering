# Live Football World Cup Scoreboard

A take-home coding exercise (Sportradar's "Live Football World Cup Scoreboard" kata, from
"Coding Exercise version 2.01.pdf" — Sportradar's "Data & Odds Platform" exercise). The full
brief is summarized verbatim in its own section below.

**The library (`src/WorldCupScoreboard/`) is the actual deliverable the exercise asked for**,
implemented in .NET 9 (C#) with xUnit. The HTTP API and the Angular frontend in this repo are
both explicitly **additional, optional work built on top of the library** — not something the
brief requested. They're kept as their own separate project phases specifically so that
distinction stays visible rather than getting blurred into "the exercise."

## Process: Spec-Driven Development with GitHub Spec-Kit

This repo was built using [GitHub Spec-Kit](https://github.com/github/spec-kit)'s full
SDLC pipeline, not by writing code first and documenting it afterward. Each unit of work — the
4 required operations, the chosen extra feature, and each later phase — went through:

```
/speckit-constitution → /speckit-specify → /speckit-clarify → /speckit-plan →
/speckit-tasks → /speckit-analyze → /speckit-converge → /speckit-implement
```

- **`.specify/memory/constitution.md`** — 5 ratified project-wide principles (Test-First;
  Verify-Plan-Implement-Verify for bug fixes; Single-Concern Features; Layered
  Architecture/Library-First; Runnable Local Verification) that every feature below had to
  satisfy.
- **`specs/`** — one folder per feature (`001-start-match` through `007-scoreboard-frontend`),
  each with its own `spec.md` (requirements), `plan.md` (technical design), and `tasks.md`
  (the actual checklist implementation followed) — the full paper trail behind every decision
  in this README, not just this document's summary of it.
- **`/speckit-analyze`** and **`/speckit-converge`** were run repeatedly, not once, to catch
  drift between what the specs said and what the code actually did (including two real bugs
  `/speckit-converge` caught before they'd have shipped — see the Testing section below).
- **`AI.md`** documents how these tools were used stage-by-stage, with embedded prompt history.

## The coding exercise brief

Summarized from the brief document itself (`Coding Exercise version 2.01.pdf`):

> Implement a **simple library** that manages a Live Football World Cup Scoreboard, supporting
> multiple simultaneous matches, as a **Java package in a Maven project** (this repo uses .NET
> 9/C# instead). Required operations:
> 1. Start a new match (initial score 0-0)
> 2. Update the score
> 3. Finish a match
> 4. Get a summary of matches **in progress**, ordered by total score (descending), tied
>    matches broken by most-recently-started first
> 5. **Add exactly one additional operation of your choice** — documented in this README,
>    explaining the feature and *why* it was chosen, landing in its own distinct git commit
>
> The brief includes a worked example (five matches started and scored in a specific order)
> with an exact expected summary ordering, and requires a `README.md` (assumptions, reasoning,
> trade-offs) and an `AI.md` (AI usage summary, prompt history, guiding artifacts) alongside the
> library implementation.

Everything above is implemented and tested — see "What it does" and the worked-example test
below. What follows is the chosen extra feature and the reasoning behind it, since the brief
asks for that explanation specifically.

### The chosen extra feature: `GetHistory` (paginated match history)

**`GetHistory(int page)`** returns every match ever started — in-progress *and* finished —
tagged with its status, ordered by most recently created-or-updated first, paginated at 10
entries per page.

**Why this one:** the scoreboard's natural home is a "Data & Odds Platform" (the exercise's own
subtitle) — historical match results have standalone value beyond the live board (settling
bets, building statistics, auditing), and pagination is what makes browsing that history
practical once there are more than a handful of matches. It's also the most natural complement
to `GetSummary`: one shows "what's live right now," the other shows "everything that's ever
happened," and both share the same underlying data with a different filter/ordering — a good
test of whether the model generalizes cleanly rather than being special-cased for the summary
view alone.

`GetHistory` is implemented and tested as its own feature ([`specs/005-match-history/`](specs/005-match-history/))
and lands in its own commit, per the brief's explicit requirement.

## Phase 1: The library

### What it does

`WorldCupScoreboard.Scoreboard` (implementing `IScoreboard`) supports:

- **`StartMatch`** — start a new match between two teams, initial score 0-0
- **`UpdateScore`** — update a match's home/away score (absolute values, not deltas)
- **`FinishMatch`** — remove a match from play
- **`GetSummary`** — get all matches currently in progress, ordered by total score descending,
  most-recently-started first on ties
- **`GetHistory`** — the chosen extra feature (see above): every match ever started,
  in-progress and finished, paginated

### Basic usage

```csharp
using Microsoft.EntityFrameworkCore;
using WorldCupScoreboard;
using WorldCupScoreboard.Persistence;

// Business logic depends only on IMatchRepository, never on a concrete storage technology
// (see "Persistence" below) — this example uses the real SQLite-backed implementation.
var dbContext = new ScoreboardDbContext(
    new DbContextOptionsBuilder<ScoreboardDbContext>()
        .UseSqlite("Data Source=scoreboard.db")
        .Options);
dbContext.Database.Migrate();

IScoreboard scoreboard = new Scoreboard(new SqliteMatchRepository(dbContext));

// Start a few matches (the brief's own worked example)
scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow, "Estadio Azteca");
scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow, "Camp Nou");
scoreboard.StartMatch("Germany", "France", DateTime.UtcNow, "Allianz Arena");
scoreboard.StartMatch("Uruguay", "Italy", DateTime.UtcNow, "Estadio Centenario");
scoreboard.StartMatch("Argentina", "Australia", DateTime.UtcNow, "La Bombonera");

// Update scores (absolute values — see Assumptions)
scoreboard.UpdateScore(1, homeScore: 0, awayScore: 5);
scoreboard.UpdateScore(2, homeScore: 10, awayScore: 2);
scoreboard.UpdateScore(3, homeScore: 2, awayScore: 2);
scoreboard.UpdateScore(4, homeScore: 6, awayScore: 6);
scoreboard.UpdateScore(5, homeScore: 3, awayScore: 1);

// GetSummary(): Uruguay 6-Italy 6, Spain 10-Brazil 2, Mexico 0-Canada 5,
//               Argentina 3-Australia 1, Germany 2-France 2
foreach (var match in scoreboard.GetSummary())
{
    Console.WriteLine($"{match.HomeTeam.Name} {match.HomeTeam.Score} - " +
                       $"{match.AwayTeam.Name} {match.AwayTeam.Score}");
}
```

This exact scenario is the brief's own worked example and is encoded as a literal acceptance
test: [`GetSummaryWorkedExampleTests.cs`](tests/WorldCupScoreboard.Tests/GetSummaryWorkedExampleTests.cs).

### Assumptions

The brief leaves several details unspecified. These are the choices made, and why:

- **Team names must be non-null/non-empty, and a match can't have the same team on both
  sides.** `StartMatch` treats either as a rejection (returns `null`, doesn't throw — see
  "Throwing vs. non-throwing" below).
- **A team can't be in more than one in-progress match at a time.** Also enforced as a
  rejection in `StartMatch`, not an exception — starting a match is a routine, expected-to-fail
  sometimes operation (like a form submission), not an exceptional one.
- **Scores are non-negative integers, supplied as absolute values, not deltas** — matching the
  brief's own example ("Mexico 0 - Canada 5" is a value to set, not an increment).
- **Scores can only go up, never down.** `UpdateScore` rejects (throws) if either team's new
  score is lower than its currently recorded value; an unchanged score is accepted (only an
  actual decrease is rejected).
- **Operating on a non-existent or already-finished match throws** (`MatchNotFoundException`)
  for `UpdateScore`, `FinishMatch`, and `GetHistory`'s page-argument equivalent
  (`InvalidPageException`) — see "Throwing vs. non-throwing" below for why this differs from
  `StartMatch`/`GetMatch`.
- **Thread-safety via coarse-grained locking.** Every `Scoreboard` method takes a single
  internal lock for its full duration. This is simple and provably correct, not optimized for
  throughput — a fine-grained (e.g. per-match) locking scheme would allow more concurrency but
  adds real complexity and deadlock risk for a kata-scale library with no stated performance
  requirement. Documented here as a deliberate simplicity-over-throughput trade-off, not an
  oversight.

### Ordering: monotonic counters, not wall-clock

`GetSummary`'s tie-break ("most recently started first") and `GetHistory`'s ordering ("most
recently created or updated first") are both implemented using **monotonic in-memory sequence
counters** (`Match.Id` for start order, `Match.ActivitySequence` for most-recent-activity
order) — not `DateTime.UtcNow` timestamps.

**Why:** wall-clock timestamps have real resolution limits, and two matches started
programmatically in a tight loop (exactly the shape of the brief's own worked example and its
test) can legitimately share the same millisecond, making a `DateTime`-based tie-break
non-deterministic. A counter incremented once per operation, inside the same lock that performs
the operation, is unambiguous and keeps tests deterministic without any `Thread.Sleep`-style
workarounds.

### Throwing vs. non-throwing: the API is deliberately split

- **`StartMatch`/`GetMatch` return `null`** on rejection/not-found — starting a match that
  conflicts with an existing one, or looking up a match that doesn't exist, are both routine,
  expected outcomes a caller should handle inline, not exceptional conditions.
- **`UpdateScore`/`FinishMatch`/`GetHistory` throw** (`MatchNotFoundException`,
  `InvalidScoreException`, `InvalidPageException`) — these represent a caller trying to act on
  a match (or page) that should already be known-valid by the time these are called (e.g. a
  match ID obtained from `GetSummary`), so a violation is closer to a programming error than a
  normal branch of control flow, and .NET convention favours exceptions for that case.

This split is documented explicitly rather than left as an inconsistency: it's a considered
choice about which failures are "normal" and which aren't, not different code paths that
happened to diverge.

### Trade-offs

- **Simplicity over strict validation**: `StartMatch` returns a single generic rejection
  (`null`) rather than distinguishing *which* validation rule failed. The library itself always
  knows the specific reason internally, but exposing a taxonomy of rejection reasons wasn't
  asked for and would add API surface for a kata-scale library; the HTTP API layer (Phase 2)
  does the same — see its own section below.
- **Coarse locking over throughput**: covered above — one lock, whole-method scope, chosen for
  provable correctness over concurrency at kata scale.
- **SQLite + EF Core persistence, not in-memory-only**: the brief calls for a "simple library,"
  which could reasonably mean in-memory-only. This project persists to SQLite via EF Core
  instead, introduced from the very first spec so every later feature builds on it without
  rework. This is a deliberate choice to go beyond the brief's minimum, not a misreading of
  it — matches survive process restarts, which is closer to how the extra feature (`GetHistory`)
  would actually be used in practice. Business logic never depends on EF Core directly; it
  depends only on the `IMatchRepository` abstraction, so unit tests run against a fast
  in-memory fake, not a real database (see Testing below).

## Testing

### Test folders and what's in them

| Folder | What it tests | How |
|---|---|---|
| `tests/WorldCupScoreboard.Tests/` | The Phase 1 library (17 test files, 61 tests) | xUnit, against `Fakes/InMemoryMatchRepository` — no real database |
| `tests/WorldCupScoreboard.Api.Tests/` | The Phase 2 API (7 test files, 15 tests) | xUnit + `WebApplicationFactory`, real HTTP requests against an in-memory-backed instance |
| `src/WorldCupScoreboard.Frontend/src/**/*.spec.ts` | The Phase 3 frontend components/services (14 spec files, 44 tests) | Karma/Jasmine |
| `src/WorldCupScoreboard.Frontend/e2e/scoreboard.spec.ts` | The full stack, end-to-end (4 scenarios) | Playwright, against a real running frontend + API pair |

### Methodology: Test-First (TDD), not tests-after

Every operation across all three phases was built **red-green-refactor**: a failing test was
written first, confirmed to fail for the right reason (the code/route/component didn't exist
yet), then the minimum implementation was written to make it pass, per this project's
Constitution Principle I (Test-First, NON-NEGOTIABLE — see `.specify/memory/constitution.md`).
No production code was written without a preceding failing test.

When a bug surfaced (rather than a new feature), the project's Principle II applied instead:
reproduce and identify the root cause first — never guess-fix — state the intended fix in one
sentence, implement the minimal fix, then re-run the **full** test suite (not just the failing
test) to confirm the fix and rule out regressions. Two real examples where this mattered: a
DI-lifetime bug in the API (caught before it shipped, not after) and two CRITICAL bugs in the
frontend's data model/status handling that `/speckit-converge` caught by comparing the
already-written code against the specs — both fixed this way, both verified live against the
real running services afterward, not just re-tested in isolation.

### Coverage

Measured directly (`dotnet test --collect:"XPlat Code Coverage"`, `ng test --code-coverage`),
not estimated:

- **Library business logic** (`Scoreboard`, `Match`, `Team`, `Exceptions/*` — excluding EF Core's
  auto-generated `Migrations/` and the `ScoreboardDbContext`/`SqliteMatchRepository` persistence
  classes): **85.5% line coverage** from the 61 unit tests.
- **API layer** (`Program.cs` + `Contracts/*`): **99.3% line coverage** from the 15 integration
  tests.
- **Frontend**: **94.6% line coverage** (93.5% statements, 90.7% functions) from the 44
  Karma/Jasmine tests.

The persistence classes (`ScoreboardDbContext`, `SqliteMatchRepository`, EF Core's generated
`Migrations/`) are **deliberately not unit-tested** — per Constitution Principle IV, unit tests
exercise business logic against a fake `IMatchRepository`, not a real database, so persistence
code itself isn't in scope for that suite. It's exercised instead by the CLI demo, the API's own
startup path, and the Docker/Playwright verification against a real SQLite database — not
skipped, just verified at a different layer.

## Run each part separately

**Library** — build and test it on its own (no server, it's a library):

```bash
dotnet build
dotnet test
```

`dotnet test` runs all 76 backend tests (61 library + 15 API) — zero failures expected,
including [`GetSummaryWorkedExampleTests.cs`](tests/WorldCupScoreboard.Tests/GetSummaryWorkedExampleTests.cs),
which encodes the brief's exact worked example as a literal acceptance test.

**CLI demo** — a thin interactive console wrapping `IScoreboard`, with no business logic of its
own:

```bash
dotnet run --project demo/ScoreboardCli
```

Supported commands: `start`, `get`, `update`, `finish`, `summary`, `history`, `ids`, `help`,
`exit` — every library operation can be exercised manually this way.

**API** (Phase 2, standalone):

```bash
dotnet run --project src/WorldCupScoreboard.Api --urls http://localhost:5000
# then open http://localhost:5000/swagger
```

**Frontend** (Phase 3, standalone — requires the API running separately first, per above):

```bash
cd src/WorldCupScoreboard.Frontend
npm install
npm start   # ng serve, http://localhost:4200
```

Frontend tests, on their own:

```bash
cd src/WorldCupScoreboard.Frontend
npm test                          # ng test — Karma/Jasmine, 44 tests
npx playwright test               # 4 end-to-end scenarios (needs both API and frontend already running)
```

## Run everything together (Docker Compose)

The root `docker-compose.yml` defines two services — `scoreboard-api` (built from the root
`Dockerfile`, port `5000`) and `scoreboard-frontend` (built from
`src/WorldCupScoreboard.Frontend/Dockerfile`, port `4200`, `depends_on` the API) — so both come
up together from one command, already wired to talk to each other. No manual configuration
(ports, URLs, CORS) is needed.

**Prerequisites**: Docker (with Compose) installed and running.

**Build and start both containers:**

```bash
docker compose up --build
```

This builds both images (a multi-stage .NET SDK build for the API, a multi-stage Node build →
Nginx runtime for the frontend) and starts both containers in the foreground, streaming both
services' logs interleaved. Wait for `Now listening on: http://+:5000` from the API and for the
frontend's Nginx to report it's ready, then open:

- **`http://localhost:4200`** — the dashboard itself
- **`http://localhost:5000/swagger`** — the API's interactive Swagger UI

**Run it in the background instead** (so your terminal is free), and check status/logs:

```bash
docker compose up --build -d
docker compose ps          # confirm both containers show "Up"
docker compose logs -f     # follow both services' logs
```

**Stop everything:**

```bash
docker compose down
```

**Note**: the API's SQLite database lives inside the container's own filesystem (no volume is
mounted in `docker-compose.yml`), so match data persists across a plain `docker compose stop`/
`start` but is reset whenever the container is recreated (`docker compose down` followed by a
fresh `up`, or `--build` after a code change) — by design, since the exercise doesn't call for
durable storage across container rebuilds, and it keeps every fresh `docker compose up --build`
a clean, repeatable starting point for the Playwright suite (`npx playwright test`, from
[`e2e/scoreboard.spec.ts`](src/WorldCupScoreboard.Frontend/e2e/scoreboard.spec.ts)) to run
against.

---

## Phase 2 (beyond the brief): HTTP API

`src/WorldCupScoreboard.Api/` is a minimal ASP.NET Core Web API (Minimal API, not MVC
controllers) wrapping every `IScoreboard` operation behind HTTP. **This is not part of the
brief** — it's an explicit scope expansion, kept as its own separate phase/spec so it's clear
this goes beyond "a simple library."

- **Endpoints**: `POST /matches`, `GET /matches/{id}`, `PUT /matches/{id}/score`,
  `POST /matches/{id}/finish`, `GET /matches/summary`, `GET /matches/history?page={page}` — one
  per `IScoreboard` method, each a thin transport adapter with no business logic of its own (all
  validation/ordering logic stays in the Phase 1 library).
- **Error shape**: every 4xx response returns `{ "error_code": "...", "error_message": "..." }`
  (e.g. `start_rejected`, `match_not_found`, `invalid_score`, `invalid_page`) — implemented via
  the `OneOf` package as a discriminated union per endpoint (`OneOf<Match, ErrorType>`), so the
  compiler enforces that every failure case is handled, and the error-to-HTTP-response mapping
  is written once in a shared helper rather than duplicated per endpoint. Full contract:
  [`specs/006-scoreboard-api/contracts/api.md`](specs/006-scoreboard-api/contracts/api.md).
- **Interactive docs**: Swagger UI (via Swashbuckle.AspNetCore) documents and lets you invoke
  every endpoint directly from a browser.
- **Containerized**: a `Dockerfile` at the repo root builds and runs the API standalone.

## Phase 3 (beyond the brief): Angular frontend

`src/WorldCupScoreboard.Frontend/` is an Angular 18 single-page application (Angular Material,
white/blue theme) consuming the Phase 2 API — **also not part of the brief**, a further
deliberate scope expansion. It provides a left-nav dashboard (Summary / History / Matches),
country-vs-country match cards, a form to start/update/finish matches, and Material dialogs for
both success confirmations and backend rejections.

---

## Repository layout

```
/
├── README.md / AI.md / chat-history.md
├── WorldCupScoreboard.sln
├── Dockerfile / docker-compose.yml
├── src/
│   ├── WorldCupScoreboard/            Phase 1: the library (Match, Scoreboard, Persistence/)
│   ├── WorldCupScoreboard.Api/        Phase 2: the HTTP API
│   └── WorldCupScoreboard.Frontend/   Phase 3: the Angular frontend
├── tests/
│   ├── WorldCupScoreboard.Tests/          61 unit tests (library)
│   └── WorldCupScoreboard.Api.Tests/      15 integration tests (API, real HTTP endpoints)
├── demo/ScoreboardCli/                 Phase 1's runnable CLI demo
├── .specify/memory/constitution.md     The 5 ratified project principles (see Process above)
└── specs/                              Spec-Kit artifacts — one folder per feature
    ├── 001-start-match / 002-update-score / 003-finish-match / 004-live-summary
    ├── 005-match-history                    (the chosen extra feature)
    ├── 006-scoreboard-api                    (Phase 2)
    └── 007-scoreboard-frontend                (Phase 3)
```
