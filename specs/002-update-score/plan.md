# Implementation Plan: Update Score

**Branch**: `002-update-score` | **Date**: 2026-08-03 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-update-score/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add an `UpdateScore` operation to the scoreboard library that sets new absolute home/away scores
on an existing in-progress match, enforcing monotonic non-decrease per team (FR-002/FR-003):
either team's new score may stay the same or increase, never decrease, and any malformed value
(negative, non-integer at the contract boundary) is rejected. Any rejection — a malformed/
decreasing score, or a match ID that doesn't resolve to an in-progress match (FR-005) — raises an
error and leaves the match's previously recorded score completely unchanged (FR-004); this is the
project's first feature to raise an error rather than return a non-throwing result, so it also
introduces the `Exceptions/` folder anticipated in CLAUDE.md's target repo layout.

## Technical Context

**Language/Version**: C# / .NET 9 (net9.0) — unchanged from `001-start-match`.

**Primary Dependencies**: None new. Reuses `001-start-match`'s existing
`Microsoft.EntityFrameworkCore.Sqlite`/`.Design` 9.0.10 and `IMatchRepository` abstraction; xUnit
for `tests/WorldCupScoreboard.Tests`.

**Storage**: SQLite via Entity Framework Core, unchanged from `001-start-match`. No schema change
and no new EF Core migration are needed — `Team.Score` (mapped as `HomeTeamScore`/`AwayTeamScore`)
is already a column in the `InitialCreate` migration, already `internal set`-mutable, and
`IMatchRepository.Update` already exists (added ahead of need during `001-start-match`'s
persistence retrofit, specifically anticipating this feature — see that feature's
`/speckit-analyze` finding E1, later confirmed correct rather than premature).

**Testing**: xUnit, strict TDD/Red-Green-Refactor per constitution Principle I (Test-First,
NON-NEGOTIABLE) — a failing test precedes every production-code change for FR-001 through FR-007.

**Target Platform**: Cross-platform .NET 9 class library, unchanged.

**Project Type**: Library (single project) — Phase 1, per constitution Principle IV. No API or
frontend concerns in this plan.

**Performance Goals**: None specified, unchanged rationale from `001-start-match`.

**Constraints**: Reuses `Scoreboard`'s existing coarse-grained lock (`001-start-match`
research.md §4) — `UpdateScore` is one more mutating operation guarded by the same single lock,
no new concurrency primitive introduced.

**Scale/Scope**: Unchanged from `001-start-match` — small, in-memory-typical collection of
matches; SQLite-backed per the project's persistence decision.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Result |
|---|---|---|
| I. Test-First (NON-NEGOTIABLE) | Every FR (001-007) must have a preceding failing test before implementation | **PASS** — enforced at `/speckit-tasks`/`/speckit-implement` time; no design change needed here |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | Process gate for handling test failures/bugs during implementation | **PASS** — not a design-time blocker; will be followed during implementation |
| III. Single-Concern Features | This spec must do exactly one thing (update an existing match's score) | **PASS** — no start/finish/summary/history logic included; validation and rejection rules are all in service of the single `UpdateScore` operation |
| IV. Layered Architecture / Library-First (persistence via `IMatchRepository`) | Business logic/validation must live only in the library; `Scoreboard` must depend only on `IMatchRepository`, never EF Core/SQLite directly | **PASS** — `UpdateScore` is implemented entirely inside `Scoreboard`, reading/writing only through the already-established `IMatchRepository`; no new persistence-facing type is needed |
| V. Runnable Local Verification (CLI Demo) | Every feature must be exercisable via `demo/ScoreboardCli`, updated in the same commit | **PASS** — plan requires an `update` command added to the CLI demo (Project Structure below) |

No violations. Complexity Tracking table below is not applicable.

**Post-Phase-1 re-check**: `data-model.md`, `contracts/IScoreboard.md`, and `quickstart.md`
introduce one new concept not present in `001-start-match` — two custom exception types
(`Exceptions/MatchNotFoundException.cs`, `Exceptions/InvalidScoreException.cs`) — but these are
plain C# exception classes with no framework dependency, so they don't disturb Principle IV's
layering. All five gates above still **PASS** unchanged.

## Project Structure

### Documentation (this feature)

```text
specs/002-update-score/
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
├── IScoreboard.cs         # Adds UpdateScore(int matchId, int homeScore, int awayScore) : Match
├── Scoreboard.cs          # Adds UpdateScore: resolve+validate under the existing lock, mutate
│                          # both Team.Score values, persist via repository.Update
├── Match.cs, Team.cs      # Unchanged — Team.Score is already internal-set-mutable
└── Exceptions/            # NEW — first feature to raise errors rather than return null
    ├── MatchNotFoundException.cs   # Thrown for FR-005 (no in-progress match with that ID)
    └── InvalidScoreException.cs    # Thrown for FR-002/FR-003 (malformed or decreasing score)

tests/WorldCupScoreboard.Tests/
└── UpdateScoreTests.cs    # FR-001..FR-007, Acceptance Scenarios 1-6

demo/ScoreboardCli/
└── Program.cs             # Adds an `update <matchId> <homeScore> <awayScore>` command
```

**Structure Decision**: Same single-project library layout as `001-start-match`
(`src/WorldCupScoreboard/`, `tests/WorldCupScoreboard.Tests/`, `demo/ScoreboardCli/`). No new
project is created. `Exceptions/` is the only new folder, matching CLAUDE.md's target repo layout
(`src/WorldCupScoreboard/Exceptions/`) — this is the first feature whose rejection behavior
(spec.md Assumptions: "raised error, not a non-throwing result") actually needs it.

## Complexity Tracking

> No Constitution Check violations — this section is not applicable.
