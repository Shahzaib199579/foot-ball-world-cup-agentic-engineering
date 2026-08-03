# Implementation Plan: Finish Match

**Branch**: `003-finish-match` | **Date**: 2026-08-04 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/003-finish-match/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add a `FinishMatch` operation that transitions an in-progress match to a new, terminal
`Finished` status, keeping every other recorded attribute (teams, final score, scheduled
date/time, location) unchanged and retrievable. Rejects (via a raised error, reusing
`002-update-score`'s `MatchNotFoundException`) any attempt to finish a nonexistent or
already-finished match. This feature's actual new code surface is narrow: add `Finished` to
`MatchStatus` and implement `FinishMatch` itself — `Scoreboard.StartMatch`'s conflict checks and
`Scoreboard.UpdateScore`'s in-progress check were both already written defensively against
`Status != MatchStatus.InProgress` during `001-start-match`/`002-update-score`, anticipating
exactly this value, and need no code change to satisfy FR-005/FR-006.

## Technical Context

**Language/Version**: C# / .NET 9 (net9.0) — unchanged.

**Primary Dependencies**: None new. Reuses `001-start-match`'s
`Microsoft.EntityFrameworkCore.Sqlite`/`.Design` 9.0.10, `IMatchRepository`, and
`002-update-score`'s `MatchNotFoundException`; xUnit for `tests/WorldCupScoreboard.Tests`.

**Storage**: SQLite via Entity Framework Core, unchanged. No new EF Core migration is needed —
`Match.Status` is already an `INTEGER` column (mapping the `MatchStatus` enum by its underlying
value); adding a new enum member (`Finished = 1`) doesn't change the schema, only which integer
values are valid at the application layer.

**Testing**: xUnit, strict TDD/Red-Green-Refactor per constitution Principle I — a failing test
precedes every production-code change for FR-001 through FR-007.

**Target Platform**: Cross-platform .NET 9 class library, unchanged.

**Project Type**: Library (single project) — Phase 1, per constitution Principle IV.

**Performance Goals**: None specified, unchanged rationale from prior features.

**Constraints**: Reuses `Scoreboard`'s existing coarse-grained lock — `FinishMatch` is one more
mutating operation guarded by the same single lock, no new concurrency primitive.

**Scale/Scope**: Unchanged from prior features.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Result |
|---|---|---|
| I. Test-First (NON-NEGOTIABLE) | Every FR (001-007) must have a preceding failing test before implementation | **PASS** — enforced at `/speckit-tasks`/`/speckit-implement` time |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | Process gate for handling test failures/bugs during implementation | **PASS** — not a design-time blocker |
| III. Single-Concern Features | This spec must do exactly one thing (finish an existing match) | **PASS** — no "reopen"/"unfinish" operation is introduced (spec.md Assumptions); no live-summary/history logic included |
| IV. Layered Architecture / Library-First (persistence via `IMatchRepository`) | Business logic/validation must live only in the library; `Scoreboard` must depend only on `IMatchRepository` | **PASS** — `FinishMatch` is implemented entirely inside `Scoreboard`, reading/writing only through the existing `IMatchRepository`; no new persistence-facing type is needed |
| V. Runnable Local Verification (CLI Demo) | Every feature must be exercisable via `demo/ScoreboardCli`, updated in the same commit | **PASS** — plan requires a `finish` command added to the CLI demo |

No violations. Complexity Tracking table below is not applicable.

**Post-Phase-1 re-check**: `data-model.md`, `contracts/IScoreboard.md`, and `quickstart.md`
introduce no new types beyond one new `MatchStatus` enum member — no new exception, no new
persistence type. All five gates above still **PASS** unchanged.

## Project Structure

### Documentation (this feature)

```text
specs/003-finish-match/
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
├── MatchStatus.cs         # Adds Finished (the only new enum member; InProgress unchanged)
├── IScoreboard.cs          # Adds FinishMatch(int matchId) : Match
├── Scoreboard.cs           # Adds FinishMatch: resolve+confirm in-progress (reuse
│                          # MatchNotFoundException), set Status = Finished, persist
└── Exceptions/             # Unchanged — no new exception type, reuses MatchNotFoundException

tests/WorldCupScoreboard.Tests/
├── FinishMatchTests.cs            # FR-001, FR-002, FR-007, Acceptance Scenario 1
├── FinishMatchRejectionTests.cs   # FR-004, Acceptance Scenarios 2-3
└── FinishMatchSideEffectsTests.cs # FR-005, FR-006, Acceptance Scenarios 4-5 — proves the
                                    # already-defensive StartMatch/UpdateScore checks activate

demo/ScoreboardCli/
└── Program.cs              # Adds a `finish <matchId>` command
```

**Structure Decision**: Same single-project library layout as prior features. No new project,
no new persistence type, no new exception type. `MatchStatus.cs` gains its second value; every
other file listed already exists and is being extended, not created.

## Complexity Tracking

> No Constitution Check violations — this section is not applicable.
