---

description: "Task list for 004-live-summary"
---

# Tasks: Live Summary

**Input**: Design documents from `/specs/004-live-summary/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/IScoreboard.md, quickstart.md

**Tests**: Included — constitution Principle I (Test-First, NON-NEGOTIABLE) mandates a failing
test before every production-code change, so test tasks are not optional here.

**Organization**: `spec.md` defines a single user story (US1 = P1, View live standings ordered
by score) — every task below belongs to it except Setup/Foundational/Polish.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on an incomplete task)
- **[Story]**: Which user story this task belongs to (US1)
- File paths are exact, per plan.md's Project Structure

## Phase 1: Setup

**Purpose**: N/A — the solution, both projects, and the demo CLI already exist. No new setup
tasks are needed for this feature.

**Checkpoint**: `dotnet build` already succeeds on the current solution before this feature's
code is added — nothing to do here.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The one new computed property every part of this feature depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T001 Add a get-only computed `TotalScore` property to `Match` in
  `src/WorldCupScoreboard/Match.cs`: `public int TotalScore => HomeTeam.Score +
  AwayTeam.Score;` (per data-model.md — no backing field, no setter, not mapped by EF Core;
  research.md §1-§2).

**Checkpoint**: `Match.TotalScore` exists and compiles — `GetSummary`'s tests and
implementation can now be written.

---

## Phase 3: User Story 1 - View live standings ordered by score (Priority: P1) 🎯 MVP

**Goal**: Callers can retrieve a summary of every in-progress match, ordered by total score
descending with most-recently-started-first as the tie-break, always reflecting the latest
score updates, excluding finished matches.

**Independent Test**: Start several matches, update their scores to known values, request the
summary, and verify the returned order matches total-score-descending with
most-recently-started-first on ties — including reproducing the brief's own worked example
exactly.

### Tests for User Story 1

> **Write these tests FIRST — confirm they FAIL before writing implementation (T006-T007)**

- [X] T002 [P] [US1] Write failing tests for ordering by total score and the tie-break rule
  (FR-002, FR-003; Acceptance Scenarios 2-3, Edge Case 1 — three-or-more-way ties) in
  `tests/WorldCupScoreboard.Tests/GetSummaryOrderingTests.cs`.
- [X] T003 [P] [US1] Write failing tests for the summary reflecting live score updates (FR-004;
  Acceptance Scenario 4, Edge Cases 2-3 — a same-total update not disturbing tie order, and a
  freshly-started 0-0 match appearing immediately) in
  `tests/WorldCupScoreboard.Tests/GetSummaryLiveUpdateTests.cs`.
- [X] T004 [P] [US1] Write failing tests for scope and read-only behavior (FR-001, FR-005;
  Acceptance Scenarios 5-6 — empty result when nothing is in-progress, a finished match
  excluded, and that calling `GetSummary` doesn't change any match's data) in
  `tests/WorldCupScoreboard.Tests/GetSummaryScopeTests.cs`.
- [X] T005 [P] [US1] Write a failing test reproducing the brief's exact worked example (FR-006;
  Acceptance Scenario 1 — Mexico 0–Canada 5, Spain 10–Brazil 2, Germany 2–France 2, Uruguay
  6–Italy 6, Argentina 3–Australia 1, started in that order, expecting exactly: Uruguay, Spain,
  Mexico, Argentina, Germany) in its own dedicated file,
  `tests/WorldCupScoreboard.Tests/GetSummaryWorkedExampleTests.cs`, per CLAUDE.md's commitment
  to treat this as a literal acceptance test.

### Implementation for User Story 1

- [X] T006 [US1] Add `IEnumerable<Match> GetSummary()` to
  `src/WorldCupScoreboard/IScoreboard.cs`, per `contracts/IScoreboard.md`.
- [X] T007 [US1] Implement `Scoreboard.GetSummary` in `src/WorldCupScoreboard/Scoreboard.cs`
  under the existing lock: filter `repository.GetAll()` to `Status == MatchStatus.InProgress`,
  order by `TotalScore` descending then `Id` descending (FR-002/FR-003, research.md §3), return
  the result. Depends on T001 (`TotalScore`), T006 (interface signature). Never throws — an
  empty result is a normal outcome (FR-001/spec.md Edge Cases).
- [X] T008 [US1] Run `dotnet test --filter FullyQualifiedName~GetSummary`; confirm T002-T005
  all pass. Then run the full suite (`dotnet test`) to confirm no regression in
  `001`-`003`. On any failure, apply constitution Principle II (reproduce → state the fix in
  one sentence → minimal fix → re-run the FULL suite) before proceeding.

**Checkpoint**: User Story 1 is complete, independently functional, and fully tested — this is
the MVP (and the only story in this feature), including the brief's literal worked example.

---

## Phase 4: Polish & Cross-Cutting Concerns

- [X] T009 Add a `summary` command (no arguments) to `demo/ScoreboardCli/Program.cs` (per
  constitution Principle V) — calls `scoreboard.GetSummary()` and prints each match on its own
  line, in order, with its total score visible; prints a clear message when the result is
  empty. Update `PrintHelp`'s command list and manual-test-scenario walkthrough to include it,
  including a scenario that reproduces the brief's worked example via the CLI itself, mirroring
  the existing `start`/`get`/`update`/`finish` entries.
- [X] T010 [P] Run `dotnet format` (or verify existing formatting) across
  `src/WorldCupScoreboard/` and `tests/WorldCupScoreboard.Tests/`.
- [X] T011 Walk through `specs/004-live-summary/quickstart.md`'s manual validation steps (1-6)
  against the built library and the CLI demo's new `summary` command, confirming the brief's
  worked example and every other acceptance scenario hold end-to-end against real SQLite.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: N/A — nothing to do, already satisfied by prior features.
- **Foundational (Phase 2)**: No dependencies — BLOCKS User Story 1 entirely (T001 must exist
  before any test or implementation task can compile/pass meaningfully).
- **User Story 1 (Phase 3)**: Depends on Foundational only. T006/T007 are the only same-file
  pair (`IScoreboard.cs`, `Scoreboard.cs`) with an ordering dependency; T002-T005 (tests) have
  no dependency on each other, only on Foundational.
- **Polish (Phase 4)**: Depends on User Story 1 being complete.

### Within User Story 1

- Tests (T002-T005) MUST be written and FAIL before their corresponding implementation
  (T006-T007).
- T006 (interface) before T007 (implementation) — same dependency chain as prior features'
  additions.

### Parallel Opportunities

- T002, T003, T004, T005 (US1 tests) can run in parallel — four different files, each
  depending only on Foundational (T001), not on each other.
- T010 (Polish) has no dependency on T009/T011 beyond both needing US1 complete — can run in
  parallel with either.

---

## Parallel Example: User Story 1

```bash
# Launch all independent-file tests for User Story 1 together, once Foundational (T001) is done:
Task: "Write failing tests for ordering/tie-break in tests/WorldCupScoreboard.Tests/GetSummaryOrderingTests.cs"
Task: "Write failing tests for live-update reflection in tests/WorldCupScoreboard.Tests/GetSummaryLiveUpdateTests.cs"
Task: "Write failing tests for scope/read-only behavior in tests/WorldCupScoreboard.Tests/GetSummaryScopeTests.cs"
Task: "Write the brief's worked example test in tests/WorldCupScoreboard.Tests/GetSummaryWorkedExampleTests.cs"
```

---

## Implementation Strategy

### MVP First (and Only) — User Story 1

1. Complete Phase 2: Foundational (T001 — the one new computed property)
2. Complete Phase 3: User Story 1 (T002-T008)
3. **STOP and VALIDATE**: `dotnet test` green (including the full `001`-`003` suites, and the
   brief's worked example passing exactly), quickstart.md steps 1-6 confirmed manually
4. This is a shippable increment — `GetSummary` completes the brief's fourth and final required
   core operation
5. Complete Phase 4: Polish (T009-T011)

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions, this feature maps to one or a couple of small,
  reviewable commits — but do not commit without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing, then passing)
  before moving to the next task — no production code without a preceding failing test.
