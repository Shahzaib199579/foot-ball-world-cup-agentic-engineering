---

description: "Task list for 003-finish-match"
---

# Tasks: Finish Match

**Input**: Design documents from `/specs/003-finish-match/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/IScoreboard.md, quickstart.md

**Tests**: Included — constitution Principle I (Test-First, NON-NEGOTIABLE) mandates a failing
test before every production-code change, so test tasks are not optional here.

**Organization**: `spec.md` defines a single user story (US1 = P1, Finish an in-progress
match) — every task below belongs to it except Setup/Foundational/Polish.

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

**Purpose**: The one new enum member every part of this feature depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T001 Add `Finished` to the `MatchStatus` enum in `src/WorldCupScoreboard/MatchStatus.cs`
  (per data-model.md — `InProgress` unchanged; this is the value `001-start-match`'s own
  data-model.md reserved for this feature).

**Checkpoint**: `MatchStatus.Finished` exists and compiles — `FinishMatch`'s tests and
implementation, and the now-reachable `StartMatch`/`UpdateScore` checks, can all be exercised.

---

## Phase 3: User Story 1 - Finish an in-progress match (Priority: P1) 🎯 MVP

**Goal**: Callers can mark an in-progress match as finished; the match remains fully
retrievable with its final score; finishing again or updating its score afterward is rejected;
its location/time becomes reusable by a new match.

**Independent Test**: Start a match, update its score, finish it, verify via `GetMatch` that its
status is now finished and its data is unchanged. Attempt to finish it again, update its score,
or reuse its old (location, scheduledAt) for a new match — the first two are rejected, the third
succeeds.

### Tests for User Story 1

> **Write these tests FIRST — confirm they FAIL before writing implementation (T005-T006)**

- [X] T002 [P] [US1] Write failing tests for a successful finish (FR-001, FR-002, FR-007;
  Acceptance Scenario 1 — status becomes `Finished`, final score and every other attribute
  unchanged, still retrievable via `GetMatch`) in
  `tests/WorldCupScoreboard.Tests/FinishMatchTests.cs`.
- [X] T003 [P] [US1] Write failing tests for rejection (FR-004; Acceptance Scenarios 2-3 —
  finishing an already-finished match, and finishing a nonexistent match ID) in
  `tests/WorldCupScoreboard.Tests/FinishMatchRejectionTests.cs` — assert
  `MatchNotFoundException` (reused from `002-update-score`) is thrown in both cases, and the
  match's `Status` remains `Finished` (not reverted) in the already-finished case.
- [X] T004 [P] [US1] Write failing tests for the two cross-feature side effects (FR-005,
  FR-006; Acceptance Scenarios 4-5) in
  `tests/WorldCupScoreboard.Tests/FinishMatchSideEffectsTests.cs`: (a) `UpdateScore` against a
  finished match throws `MatchNotFoundException` and its final score stays unchanged; (b)
  `StartMatch` succeeds when reusing a finished match's exact team names and
  `(Location, ScheduledAt)` pair. These tests exercise the already-existing checks in
  `StartMatch`/`UpdateScore` (research.md §3) — no change to those methods is expected to make
  these pass, only T001 (the new enum member) and T006 (`FinishMatch` itself, to produce a
  finished match to test against).

### Implementation for User Story 1

- [X] T005 [US1] Add `Match FinishMatch(int matchId)` to
  `src/WorldCupScoreboard/IScoreboard.cs`, per `contracts/IScoreboard.md`.
- [X] T006 [US1] Implement `Scoreboard.FinishMatch` in `src/WorldCupScoreboard/Scoreboard.cs`
  under the existing lock: resolve the match via `repository.GetById`, throw
  `MatchNotFoundException` (reused, not a new type) if missing or not `InProgress` (FR-004); on
  success, set `match.Status = MatchStatus.Finished`, call `repository.Update(match)`, and
  return the same `Match` (FR-002/FR-007). Depends on T001 (enum member), T005 (interface
  signature). Do not modify `StartMatch`/`UpdateScore` — research.md §3 confirms their existing
  checks already satisfy FR-005/FR-006 once `Finished` exists.
- [X] T007 [US1] Run `dotnet test --filter FullyQualifiedName~FinishMatch`; confirm T002-T004
  all pass. Then run the full suite (`dotnet test`) to confirm no regression in
  `001-start-match`/`002-update-score`. On any failure, apply constitution Principle II
  (reproduce → state the fix in one sentence → minimal fix → re-run the FULL suite) before
  proceeding.

**Checkpoint**: User Story 1 is complete, independently functional, and fully tested — this is
the MVP (and the only story in this feature).

---

## Phase 4: Polish & Cross-Cutting Concerns

- [X] T008 Add a `finish <matchId>` command to `demo/ScoreboardCli/Program.cs` (per
  constitution Principle V) — calls `scoreboard.FinishMatch`, prints the resulting match on
  success, and prints a clear rejection message (including the exception's message) on
  `MatchNotFoundException`. Update `PrintHelp`'s command list and manual-test-scenario
  walkthrough to include it and the FR-005/FR-006 side-effect scenarios, mirroring the existing
  `start`/`get`/`update` entries.
- [X] T009 [P] Run `dotnet format` (or verify existing formatting) across
  `src/WorldCupScoreboard/` and `tests/WorldCupScoreboard.Tests/`.
- [X] T010 Walk through `specs/003-finish-match/quickstart.md`'s manual validation steps (1-5)
  against the built library and the CLI demo's new `finish` command, confirming the acceptance
  scenarios hold end-to-end. As a bonus, `001-start-match/quickstart.md`'s own step 4 ("a
  finished match frees its slot") — previously a documented forward reference — is now also
  exercisable; confirm it too, but do not edit that file (out of scope for this feature).

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: N/A — nothing to do, already satisfied by prior features.
- **Foundational (Phase 2)**: No dependencies — BLOCKS User Story 1 entirely (T001 must exist
  before any test or implementation task can compile/pass meaningfully).
- **User Story 1 (Phase 3)**: Depends on Foundational only. T005/T006 are the only same-file
  pair (`IScoreboard.cs`, `Scoreboard.cs`) with an ordering dependency; T002-T004 (tests) have
  no dependency on each other, only on Foundational.
- **Polish (Phase 4)**: Depends on User Story 1 being complete.

### Within User Story 1

- Tests (T002-T004) MUST be written and FAIL before their corresponding implementation
  (T005-T006).
- T005 (interface) before T006 (implementation) — same dependency chain as prior features'
  additions.

### Parallel Opportunities

- T002, T003, T004 (US1 tests) can run in parallel — three different files, each depending
  only on Foundational (T001), not on each other.
- T009 (Polish) has no dependency on T008/T010 beyond both needing US1 complete — can run in
  parallel with either.

---

## Parallel Example: User Story 1

```bash
# Launch all independent-file tests for User Story 1 together, once Foundational (T001) is done:
Task: "Write failing tests for a successful finish in tests/WorldCupScoreboard.Tests/FinishMatchTests.cs"
Task: "Write failing tests for rejection in tests/WorldCupScoreboard.Tests/FinishMatchRejectionTests.cs"
Task: "Write failing tests for cross-feature side effects in tests/WorldCupScoreboard.Tests/FinishMatchSideEffectsTests.cs"
```

---

## Implementation Strategy

### MVP First (and Only) — User Story 1

1. Complete Phase 2: Foundational (T001 — the one new enum member)
2. Complete Phase 3: User Story 1 (T002-T007)
3. **STOP and VALIDATE**: `dotnet test` green (including the full `001`/`002` suites),
   quickstart.md steps 1-5 confirmed manually
4. This is a shippable increment — `FinishMatch` alone completes the brief's third core
   operation
5. Complete Phase 4: Polish (T008-T010)

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions, this feature maps to one or a couple of small,
  reviewable commits — but do not commit without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing, then passing)
  before moving to the next task — no production code without a preceding failing test.
