# Implementation Plan: Start New Match

**Branch**: `001-start-match` | **Date**: 2026-08-03 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-start-match/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add a `StartMatch` operation to the scoreboard library that creates a new in-progress match
between two distinct teams (score 0-0), records a scheduled date/time and location, assigns it a
unique match ID, and rejects the attempt (via a non-throwing result) if either team is already in
another in-progress match or another in-progress match already exists at the same location and
date/time. Also adds a `GetMatch` operation to read a started match back by its ID. This
establishes the `Match`, `Team`, `MatchStatus`, and `IScoreboard` skeleton that later specs
(002-005) build on.

## Technical Context

**Language/Version**: C# / .NET 9 (net9.0) — per CLAUDE.md's confirmed, documented deviation from
the brief's requested Java/Maven stack.

**Primary Dependencies**: Entity Framework Core + the SQLite provider for persistence
(`Microsoft.EntityFrameworkCore.Sqlite`), abstracted behind `IMatchRepository` so the library's
business logic never references EF Core/SQLite types directly; xUnit for
`tests/WorldCupScoreboard.Tests`, exercising `Scoreboard` against a fake/in-memory
`IMatchRepository` (no real database in unit tests).

**Storage**: SQLite via Entity Framework Core, per CLAUDE.md's Persistence decision — introduced
starting at this spec (001) so it applies to `Scoreboard` and every later spec without rework.
Accessed exclusively through `IMatchRepository` (constitution Principle IV, amended); `Scoreboard`
depends only on the interface, never on `DbContext`/EF Core/SQLite concretely. Amendment note:
this plan originally specified in-memory-only storage (no persistence in Phase 1); the Persistence
decision was made after `001-start-match` was first implemented against a plain
`Dictionary<int, Match>`, and this plan is now amended retroactively to bring the design in line
with the ratified constitution before the feature's tasks/implementation are finalized.

**Testing**: xUnit, strict TDD/Red-Green-Refactor per constitution Principle I (Test-First,
NON-NEGOTIABLE) — a failing test precedes every production-code change for FR-001 through FR-008.

**Target Platform**: Cross-platform .NET 9 class library. CI (`.github/workflows/dotnet.yml`,
build+test on push) is part of the target repo layout but not yet created — out of scope for this
feature; will be added when first needed, not invented here.

**Project Type**: Library (single project) — Phase 1, per constitution Principle IV (Layered
Architecture/Library-First). No API or frontend concerns in this plan.

**Performance Goals**: None specified. This is a take-home kata; correctness and clarity take
priority over throughput. No latency/throughput target is set.

**Constraints**: Coarse-grained internal locking for thread-safety — a single lock guards all
mutating `Scoreboard` operations, per CLAUDE.md's documented "simple and correct, not optimized
for throughput" trade-off.

**Scale/Scope**: Small, in-memory collection of matches typical of a kata/demo. No defined upper
bound on concurrent matches.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Result |
|---|---|---|
| I. Test-First (NON-NEGOTIABLE) | Every FR (001-008) must have a preceding failing test before implementation | **PASS** — enforced at `/speckit-tasks`/`/speckit-implement` time; no design change needed here |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | Process gate for handling test failures/bugs during implementation | **PASS** — not a design-time blocker; will be followed during implementation |
| III. Single-Concern Features | This spec must do exactly one thing (start a match) | **PASS** — clarify session explicitly resolved to keep this single-purpose (no separate schedule-then-begin split); no score-update/finish logic included |
| IV. Layered Architecture / Library-First (amended) | Business logic/validation must live only in the library; persistence abstracted behind `IMatchRepository`, business logic never depends on EF Core/SQLite directly | **PASS** — plan defines a pure class library; FR-004/FR-005/FR-006/FR-008 validation logic lives entirely inside `Scoreboard`, which depends only on `IMatchRepository`. Concrete EF Core/SQLite implementation lives in `Persistence/`, isolated from business logic; unit tests use a fake in-memory `IMatchRepository` |

No violations. Complexity Tracking table below is not applicable.

**Post-Phase-1 re-check**: `data-model.md`, `contracts/IScoreboard.md`, and `quickstart.md`
introduce no new dependencies, frameworks, or cross-layer coupling beyond the persistence
abstraction already accounted for above — `Scoreboard` still depends only on `IMatchRepository`,
never on EF Core/SQLite concretely. All four gates above still **PASS** unchanged.

**Amendment (post-implementation, pre-commit)**: this plan was updated after `001-start-match`
was already implemented against a plain `Dictionary<int, Match>`, once CLAUDE.md's Persistence
decision (SQLite via EF Core, behind `IMatchRepository`, starting at spec 001) was made. The
Constitution Check above reflects the amended design the implementation must be brought into
line with — it does not describe already-verified code. See "Outstanding" tracking in project
memory/chat history for the corresponding refactor task.

## Project Structure

### Documentation (this feature)

```text
specs/001-start-match/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
│   └── IScoreboard.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
WorldCupScoreboard.sln

src/WorldCupScoreboard/
├── IScoreboard.cs        # StartMatch, GetMatch (this feature only — grows in later specs)
├── Scoreboard.cs         # IScoreboard implementation; owns the coarse lock; depends only on
│                          # IMatchRepository (never EF Core/SQLite directly)
├── Match.cs              # Match entity (Id, HomeTeam, AwayTeam, ScheduledAt, Location, Status)
├── Team.cs               # Team value object (Name, Score) — owned by exactly one Match
├── MatchStatus.cs         # enum { InProgress } (this feature only — Finished added by 003)
└── Persistence/
    ├── IMatchRepository.cs      # Add/Get/GetAll/Update abstraction consumed by Scoreboard
    ├── ScoreboardDbContext.cs   # EF Core DbContext (Matches DbSet), SQLite-backed
    ├── SqliteMatchRepository.cs # IMatchRepository implementation over ScoreboardDbContext
    └── Migrations/              # EF Core migrations (generated via `dotnet ef migrations add`)

tests/WorldCupScoreboard.Tests/
├── StartMatchTests.cs           # FR-001..FR-003, Acceptance Scenario 1
├── StartMatchValidationTests.cs # FR-004
├── StartMatchConflictTests.cs   # FR-005, FR-006, FR-008, Acceptance Scenarios 2-3
├── GetMatchTests.cs             # FR-007, Acceptance Scenarios (User Story 2)
└── Fakes/
    └── InMemoryMatchRepository.cs # Fake IMatchRepository (no EF Core/SQLite) used by all
                                    # Scoreboard unit tests above, per constitution Principle I
```

**Structure Decision**: Single project (library), matching CLAUDE.md's target repo layout
(`src/WorldCupScoreboard/`, `tests/WorldCupScoreboard.Tests/`). No web/mobile structure applies —
Phase 2 (API) and Phase 3 (frontend) are separate future specs (006, 007) with their own plans.
This is the first feature implemented, so `WorldCupScoreboard.sln` and both projects are created
by this feature's tasks, not pre-existing. `Persistence/` (and `Persistence/Migrations/`) is
introduced here rather than in a later spec, per CLAUDE.md's Persistence decision to apply it
starting at spec 001 so `Scoreboard` and all later specs build on the repository abstraction
without rework — `IMatchRepository` is the only persistence-facing type the rest of the library
(and later specs 002-005) may depend on.

## Complexity Tracking

> No Constitution Check violations — this section is not applicable.
