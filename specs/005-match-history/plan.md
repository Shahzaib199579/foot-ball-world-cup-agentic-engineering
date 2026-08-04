# Implementation Plan: Match History

**Branch**: `005-match-history` | **Date**: 2026-08-04 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/005-match-history/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add a `GetHistory(int page)` operation returning matches (in-progress and finished) 10 at a
time, ordered by most recent activity — creation, a score update, or being finished — most
recent first. Unlike `003-finish-match`/`004-live-summary`, this feature genuinely touches
`StartMatch`, `UpdateScore`, and `FinishMatch`: none of them currently track any "last activity"
marker, so each needs one new line bumping a new `Match.ActivitySequence` field (a persisted,
monotonic counter — separate from `Id`, which only reflects creation order). A new
`InvalidPageException` is added for page numbers less than 1; out-of-range pages (beyond
available data) simply return an empty result, no exception.

## Technical Context

**Language/Version**: C# / .NET 9 (net9.0) — unchanged.

**Primary Dependencies**: None new. Reuses `001-start-match`'s
`Microsoft.EntityFrameworkCore.Sqlite`/`.Design` 9.0.10 and `IMatchRepository`; xUnit for
`tests/WorldCupScoreboard.Tests`.

**Storage**: SQLite via Entity Framework Core. **This feature does need a new EF Core
migration** — unlike `004-live-summary`'s computed `TotalScore`, `ActivitySequence` has no
other source to derive from (it's not a function of any other field), so it must be a real,
persisted `INTEGER` column, mapped with `ValueGeneratedNever()` (same reasoning as `Match.Id` —
the application assigns it, not the database).

**Testing**: xUnit, strict TDD/Red-Green-Refactor per constitution Principle I — a failing test
precedes every production-code change for FR-001 through FR-007.

**Target Platform**: Cross-platform .NET 9 class library, unchanged.

**Project Type**: Library (single project) — Phase 1, per constitution Principle IV. This is
the brief's chosen "additional operation" — still Phase 1, not Phase 2/3.

**Performance Goals**: None specified, unchanged rationale from prior features.

**Constraints**: `GetHistory` reuses `Scoreboard`'s existing coarse-grained lock. Pagination is
implemented as in-memory `.Skip()`/`.Take()` over `IMatchRepository.GetAll()`'s already-fully-
materialized result — consistent with how `GetSummary` (`004`) already sorts in-memory rather
than composing a database-level query in `Scoreboard`, per Principle IV's persistence
abstraction. At this project's stated scale (unchanged, no perf targets), this is not a
bottleneck; a future page-at-the-repository-level optimization would be `IMatchRepository`'s
own concern, not `Scoreboard`'s, if ever needed.

**Scale/Scope**: Unchanged from prior features.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Result |
|---|---|---|
| I. Test-First (NON-NEGOTIABLE) | Every FR (001-007) must have a preceding failing test before implementation | **PASS** — enforced at `/speckit-tasks`/`/speckit-implement` time |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | Process gate for handling test failures/bugs during implementation | **PASS** — not a design-time blocker |
| III. Single-Concern Features | This spec must do exactly one thing (paginated history browsing) | **PASS** — no filtering/search added; this is the brief's one chosen extra operation, land in its own commit per CLAUDE.md |
| IV. Layered Architecture / Library-First (persistence via `IMatchRepository`) | Business logic/validation must live only in the library; `Scoreboard` must depend only on `IMatchRepository` | **PASS** — `GetHistory` reads via `repository.GetAll()` and paginates in-memory in `Scoreboard`; the new `ActivitySequence` column is configured in `ScoreboardDbContext`, not referenced by `Scoreboard` directly |
| V. Runnable Local Verification (CLI Demo) | Every feature must be exercisable via `demo/ScoreboardCli`, updated in the same commit | **PASS** — plan requires a `history <page>` command added to the CLI demo |

No violations. Complexity Tracking table below is not applicable.

**Post-Phase-1 re-check**: `data-model.md`, `contracts/IScoreboard.md`, and `quickstart.md`
introduce one new persisted field (`Match.ActivitySequence`) and one new exception
(`InvalidPageException`) — no new persistence-facing *type* beyond a schema change to the
existing `ScoreboardDbContext`/migration set. All five gates above still **PASS** unchanged.

## Project Structure

### Documentation (this feature)

```text
specs/005-match-history/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
│   └── IScoreboard.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/WorldCupScoreboard/
├── Match.cs                       # Adds `public int ActivitySequence { get; internal set; }`
├── IScoreboard.cs                  # Adds IEnumerable<Match> GetHistory(int page)
├── Scoreboard.cs                   # Adds GetHistory; adds one activity-bump line each to
│                                   # StartMatch, UpdateScore, FinishMatch
├── Exceptions/
│   └── InvalidPageException.cs     # NEW — thrown for page < 1
└── Persistence/
    ├── ScoreboardDbContext.cs      # Maps ActivitySequence, ValueGeneratedNever()
    └── Migrations/                 # NEW migration: AddActivitySequence

tests/WorldCupScoreboard.Tests/
├── GetHistoryPaginationTests.cs    # FR-001, FR-003, FR-004, FR-005; Acceptance Scenarios 1-2, 4, 6
├── GetHistoryOrderingTests.cs      # FR-002; Acceptance Scenario 3, Edge Cases 1, 3
└── GetHistoryScopeTests.cs         # FR-006, FR-007; Acceptance Scenario 5, Edge Case 4

demo/ScoreboardCli/
└── Program.cs                      # Adds a `history <page>` command
```

**Structure Decision**: Same single-project library layout as prior features. New this time:
`Exceptions/InvalidPageException.cs` (a genuinely new validation concern — no existing
exception fits) and a new EF Core migration (`ActivitySequence` needs real persistence, unlike
`004`'s computed `TotalScore`). `StartMatch`/`UpdateScore`/`FinishMatch` are edited, not just
extended-by-omission — per spec.md's Assumptions, this feature doesn't get the "already
defensive/correct code" free lunch `003`/`004` did.

## Complexity Tracking

> No Constitution Check violations — this section is not applicable.
