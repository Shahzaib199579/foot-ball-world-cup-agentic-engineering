# Implementation Plan: Live Summary

**Branch**: `004-live-summary` | **Date**: 2026-08-04 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/004-live-summary/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add a `GetSummary` operation returning every in-progress match, ordered by total score
(descending) with most-recently-started-first as the tie-break. Adds a computed `TotalScore`
property to `Match` (home + away score, never independently settable, always correct by
construction) rather than a persisted column — since it's fully derivable from two values that
are already validated and persisted, storing it separately would only introduce a
sync-maintenance burden with no behavioral benefit. Reuses `Match.Id` (the existing monotonic
sequence from `001-start-match`) as the tie-break key — no new "start order" concept needed.

## Technical Context

**Language/Version**: C# / .NET 9 (net9.0) — unchanged.

**Primary Dependencies**: None new. Reuses `001-start-match`'s `IMatchRepository`/EF Core/SQLite
stack; xUnit for `tests/WorldCupScoreboard.Tests`.

**Storage**: SQLite via Entity Framework Core, unchanged — no new migration. `TotalScore` is a
get-only, expression-bodied C# property (`HomeTeam.Score + AwayTeam.Score`) with no setter and
no backing field; EF Core's model-building convention excludes such properties from the mapped
model automatically (confirmed against EF Core's documented convention — a property needs a
setter or a discoverable backing field to be included), so no `[NotMapped]` attribute or
`OnModelCreating` change is needed either.

**Testing**: xUnit, strict TDD/Red-Green-Refactor per constitution Principle I — a failing test
precedes every production-code change for FR-001 through FR-006, including the brief's worked
example as a literal test.

**Target Platform**: Cross-platform .NET 9 class library, unchanged.

**Project Type**: Library (single project) — Phase 1, per constitution Principle IV.

**Performance Goals**: None specified, unchanged rationale from prior features.

**Constraints**: `GetSummary` reuses `Scoreboard`'s existing coarse-grained lock, for a
consistent snapshot — same pattern as `GetMatch`, not a new concurrency primitive.

**Scale/Scope**: Unchanged from prior features. Sorting happens in-memory over
`IMatchRepository.GetAll()`'s result (already a fully materialized `List<Match>`, per
`SqliteMatchRepository`'s existing implementation) — no new query-composition concern, and
consistent with how `StartMatch` already iterates `GetAll()` for its own conflict checks.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Gate | Result |
|---|---|---|
| I. Test-First (NON-NEGOTIABLE) | Every FR (001-006) must have a preceding failing test before implementation, including the brief's worked example | **PASS** — enforced at `/speckit-tasks`/`/speckit-implement` time |
| II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) | Process gate for handling test failures/bugs during implementation | **PASS** — not a design-time blocker |
| III. Single-Concern Features | This spec must do exactly one thing (return an ordered live summary) | **PASS** — pagination/"browse all matches" was explicitly excluded from this spec (spec.md Assumptions) and folded into `005-match-history` instead, per the user's decision |
| IV. Layered Architecture / Library-First (persistence via `IMatchRepository`) | Business logic/validation must live only in the library; `Scoreboard` must depend only on `IMatchRepository` | **PASS** — `GetSummary` reads via `repository.GetAll()` and sorts in-memory in `Scoreboard`; `TotalScore` lives on `Match` itself with no EF Core dependency |
| V. Runnable Local Verification (CLI Demo) | Every feature must be exercisable via `demo/ScoreboardCli`, updated in the same commit | **PASS** — plan requires a `summary` command added to the CLI demo |

No violations. Complexity Tracking table below is not applicable.

**Post-Phase-1 re-check**: `data-model.md`, `contracts/IScoreboard.md`, and `quickstart.md`
introduce exactly one new member (`Match.TotalScore`, computed) and one new interface method
(`GetSummary`) — no new exception, no new persistence type, no new migration. All five gates
above still **PASS** unchanged.

## Project Structure

### Documentation (this feature)

```text
specs/004-live-summary/
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
├── Match.cs                # Adds `public int TotalScore => HomeTeam.Score + AwayTeam.Score;`
├── IScoreboard.cs           # Adds IEnumerable<Match> GetSummary()
└── Scoreboard.cs            # Adds GetSummary: filter to InProgress, order by TotalScore desc
                              # then Id desc (most-recently-started first), under the existing
                              # lock — no new exception type, this operation never rejects

tests/WorldCupScoreboard.Tests/
├── GetSummaryOrderingTests.cs     # FR-002, FR-003; Acceptance Scenarios 2-3, Edge Case 1 (3+-way ties)
├── GetSummaryLiveUpdateTests.cs   # FR-004; Acceptance Scenarios 4, Edge Case 2-3
├── GetSummaryScopeTests.cs        # FR-001, FR-005; Acceptance Scenarios 5-6 (empty result,
│                                   # finished matches excluded, read-only)
└── GetSummaryWorkedExampleTests.cs # FR-006; Acceptance Scenario 1 — the brief's literal
                                     # worked example, its own dedicated test file per
                                     # CLAUDE.md's explicit "treat as an acceptance test"
                                     # commitment

demo/ScoreboardCli/
└── Program.cs              # Adds a `summary` command
```

**Structure Decision**: Same single-project library layout as prior features. No new project,
no new persistence type, no new exception type. The brief's worked example gets its own
dedicated test file (`GetSummaryWorkedExampleTests.cs`) rather than being folded into one of the
others, so it stays trivially discoverable as *the* literal acceptance test CLAUDE.md commits
to — not buried among ordinary ordering tests.

## Complexity Tracking

> No Constitution Check violations — this section is not applicable.
