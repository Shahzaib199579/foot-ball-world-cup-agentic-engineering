---

description: "Task list for 002-update-score"
---

# Tasks: Update Score

**Input**: Design documents from `/specs/002-update-score/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/IScoreboard.md, quickstart.md

**Tests**: Included — constitution Principle I (Test-First, NON-NEGOTIABLE) mandates a failing
test before every production-code change, so test tasks are not optional here.

**Organization**: `spec.md` defines a single user story (US1 = P1, Update the score of an
in-progress match) — every task below belongs to it except Setup/Foundational/Polish.

**Refinement note**: `plan.md`'s Project Structure lists one test file
(`UpdateScoreTests.cs`). This file splits it into three, mirroring `001-start-match`'s own
established pattern (`StartMatchTests.cs`/`StartMatchValidationTests.cs`/
`StartMatchConflictTests.cs`) for clearer per-concern test organization and better `[P]`
parallelism — a task-generation-time refinement, not a contradiction of plan.md's design.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on an incomplete task)
- **[Story]**: Which user story this task belongs to (US1)
- File paths are exact, per plan.md's Project Structure

## Phase 1: Setup

**Purpose**: N/A — `WorldCupScoreboard.sln`, both projects, and the demo CLI already exist from
`001-start-match`. No new setup tasks are needed for this feature.

**Checkpoint**: `dotnet build` already succeeds on the current solution before this feature's
code is added — nothing to do here.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The two new exception types both `UpdateScore`'s tests and its implementation
depend on (research.md §1-§2, data-model.md).

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T001 [P] Create `MatchNotFoundException` (a plain `Exception` subclass, constructor takes
  the match ID and builds a descriptive message) in
  `src/WorldCupScoreboard/Exceptions/MatchNotFoundException.cs` — written generically (not
  `UpdateScore`-specific) so `003-finish-match` can reuse it later (research.md §2).
- [X] T002 [P] Create `InvalidScoreException` (a plain `Exception` subclass, constructor takes
  enough detail to build a descriptive message — which team, attempted value, current value) in
  `src/WorldCupScoreboard/Exceptions/InvalidScoreException.cs`.

**Checkpoint**: Both exception types exist and compile — `UpdateScore`'s tests and
implementation can now be written.

---

## Phase 3: User Story 1 - Update the score of an in-progress match (Priority: P1) 🎯 MVP

**Goal**: Callers can update an in-progress match's score to new absolute home/away values,
each greater than or equal to that team's current recorded score; any rejection (malformed
score, decrease, or a match ID that isn't an in-progress match) throws and leaves the match's
score completely unchanged.

**Independent Test**: Start a match, update its score upward, verify via `GetMatch`. Attempt a
decrease, a negative value, and a nonexistent match ID — each throws, and the match's recorded
score is confirmed unchanged after each attempt.

### Tests for User Story 1

> **Write these tests FIRST — confirm they FAIL before writing implementation (T007-T008)**

- [X] T003 [P] [US1] Write failing tests for successful score updates (FR-001, FR-006, FR-007;
  Acceptance Scenarios 1-2 — score increases, one team's score can stay the same while the
  other increases, every other recorded attribute of the match is unchanged, the update is
  visible via `GetMatch`) in `tests/WorldCupScoreboard.Tests/UpdateScoreTests.cs`.
- [X] T004 [P] [US1] Write failing tests for malformed-score and decrease rejection (FR-002,
  FR-003, FR-004; Acceptance Scenarios 3-4, plus the equal-value-is-accepted edge case) in
  `tests/WorldCupScoreboard.Tests/UpdateScoreValidationTests.cs` — assert
  `InvalidScoreException` is thrown and the match's recorded score is confirmed unchanged
  afterward via `GetMatch` in each rejection case (FR-004's atomicity). Include a one-line code
  comment noting that Acceptance Scenario 5 (letters/special characters) is not exercisable
  here — `homeScore`/`awayScore` are typed `int`, so that's a compile-time error, not a runtime
  case (spec.md Assumptions; quickstart.md).
- [X] T005 [P] [US1] Write failing tests for a nonexistent match ID (FR-005; Acceptance
  Scenario 6) in `tests/WorldCupScoreboard.Tests/UpdateScoreNotFoundTests.cs` — assert
  `MatchNotFoundException` is thrown.

### Implementation for User Story 1

- [X] T006 [US1] Add `Match UpdateScore(int matchId, int homeScore, int awayScore)` to
  `src/WorldCupScoreboard/IScoreboard.cs`, per `contracts/IScoreboard.md`.
- [X] T007 [US1] Implement `Scoreboard.UpdateScore` in `src/WorldCupScoreboard/Scoreboard.cs`
  under the existing lock (research.md §4, reused from `001-start-match`): resolve the match via
  `repository.GetById`, throw `MatchNotFoundException` if missing or not `InProgress` (FR-005);
  validate both `homeScore`/`awayScore` are non-negative and each `>=` the corresponding current
  `Team.Score`, throwing `InvalidScoreException` on any violation *before* mutating either score
  (FR-002/FR-003/FR-004, research.md §3-§4); on success, mutate `HomeTeam.Score`/
  `AwayTeam.Score`, call `repository.Update(match)`, and return the same `Match` (FR-006/FR-007).
  Depends on T001, T002 (exception types), T006 (interface signature).
- [X] T008 [US1] Run `dotnet test --filter FullyQualifiedName~UpdateScore`; confirm T003-T005 all
  pass. Then run the full suite (`dotnet test`) to confirm no regression in `001-start-match`. On
  any failure, apply constitution Principle II (reproduce → state the fix in one sentence →
  minimal fix → re-run the FULL suite) before proceeding.

**Checkpoint**: User Story 1 is complete, independently functional, and fully tested — this is
the MVP (and the only story in this feature).

---

## Phase 4: Polish & Cross-Cutting Concerns

- [X] T009 Add an `update <matchId> <homeScore> <awayScore>` command to
  `demo/ScoreboardCli/Program.cs` (per constitution Principle V) — calls `scoreboard.UpdateScore`,
  prints the resulting match on success, and prints a clear rejection message (including the
  exception's message) on `MatchNotFoundException`/`InvalidScoreException`. Update `PrintHelp`'s
  command list and manual-test-scenario walkthrough to include it, mirroring the existing
  `start`/`get` entries.
- [X] T010 [P] Run `dotnet format` (or verify existing formatting) across
  `src/WorldCupScoreboard/` (including the new `Exceptions/` folder) and
  `tests/WorldCupScoreboard.Tests/`.
- [X] T011 Walk through `specs/002-update-score/quickstart.md`'s manual validation steps (1-4,
  6) against the built library and the CLI demo's new `update` command, confirming the
  acceptance scenarios hold end-to-end. (Step 5 — letters/special characters — remains a
  documented non-runnable case per quickstart.md.)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: N/A — nothing to do, already satisfied by `001-start-match`.
- **Foundational (Phase 2)**: No dependencies — BLOCKS User Story 1 (its tests and
  implementation both need the exception types to exist and compile).
- **User Story 1 (Phase 3)**: Depends on Foundational only. T006/T007 are the only same-file
  pair (`IScoreboard.cs`, `Scoreboard.cs` respectively) with an ordering dependency; T003-T005
  (tests) have no dependency on each other, only on Foundational.
- **Polish (Phase 4)**: Depends on User Story 1 being complete (T009 calls the real
  `UpdateScore`; T011 exercises it end-to-end).

### Within User Story 1

- Tests (T003-T005) MUST be written and FAIL before their corresponding implementation
  (T006-T007).
- T006 (interface) before T007 (implementation) — same dependency chain as `001-start-match`'s
  `StartMatch`/`GetMatch` additions.

### Parallel Opportunities

- T001 and T002 (Foundational) can run in parallel — different files, no dependency between the
  two exception types.
- T003, T004, T005 (US1 tests) can run in parallel — three different files, each depending only
  on Foundational (T001/T002), not on each other.
- T010 (Polish) has no dependency on T009/T011 beyond both needing US1 complete — can run in
  parallel with either.

---

## Parallel Example: User Story 1

```bash
# Launch all independent-file tests for User Story 1 together, once Foundational (T001-T002) is done:
Task: "Write failing tests for successful score updates in tests/WorldCupScoreboard.Tests/UpdateScoreTests.cs"
Task: "Write failing tests for malformed-score and decrease rejection in tests/WorldCupScoreboard.Tests/UpdateScoreValidationTests.cs"
Task: "Write failing tests for a nonexistent match ID in tests/WorldCupScoreboard.Tests/UpdateScoreNotFoundTests.cs"
```

---

## Implementation Strategy

### MVP First (and Only) — User Story 1

1. Complete Phase 2: Foundational (T001-T002 — the two exception types)
2. Complete Phase 3: User Story 1 (T003-T008)
3. **STOP and VALIDATE**: `dotnet test` green (including the full `001-start-match` suite),
   quickstart.md steps 1-4 and 6 confirmed manually
4. This is a shippable increment — `UpdateScore` alone is a complete, useful addition to the
   already-shipped `001-start-match` MVP
5. Complete Phase 4: Polish (T009-T011)

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions, this feature maps to one or a couple of small,
  reviewable commits — but do not commit without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing, then passing)
  before moving to the next task — no production code without a preceding failing test.
