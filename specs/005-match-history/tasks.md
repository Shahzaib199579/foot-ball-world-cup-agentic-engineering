---

description: "Task list for 005-match-history"
---

# Tasks: Match History

**Input**: Design documents from `/specs/005-match-history/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/IScoreboard.md, quickstart.md

**Tests**: Included — constitution Principle I (Test-First, NON-NEGOTIABLE) mandates a failing
test before every production-code change, so test tasks are not optional here.

**Organization**: `spec.md` defines a single user story (US1 = P1, Browse the full match
history, a page at a time) — every task below belongs to it except Setup/Foundational/Polish.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on an incomplete task)
- **[Story]**: Which user story this task belongs to (US1)
- File paths are exact, per plan.md's Project Structure

## Phase 1: Setup

**Purpose**: N/A — the solution, both projects, the demo CLI, and the EF Core/SQLite package
references all already exist. No new setup tasks are needed for this feature.

**Checkpoint**: `dotnet build` already succeeds on the current solution before this feature's
code is added — nothing to do here.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The new field, exception type, and persistence mapping every part of this feature
depends on. Unlike `003`/`004`, this feature has no "already-defensive code" to activate —
`StartMatch`/`UpdateScore`/`FinishMatch` genuinely need editing, which happens in Phase 3 (it's
behavior, not scaffolding, so it follows Test-First there instead of here).

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T001 [P] Add `public int ActivitySequence { get; internal set; }` to `Match` in
  `src/WorldCupScoreboard/Match.cs` (per data-model.md, research.md §1 — a real persisted
  field, not computed, unlike `004-live-summary`'s `TotalScore`).
- [X] T002 [P] Create `InvalidPageException` (constructor takes the invalid page number, builds
  a descriptive message) in `src/WorldCupScoreboard/Exceptions/InvalidPageException.cs` (per
  data-model.md, research.md §3).
- [X] T003 Update `ScoreboardDbContext.OnModelCreating` in
  `src/WorldCupScoreboard/Persistence/ScoreboardDbContext.cs` to map
  `Match.ActivitySequence` with `.ValueGeneratedNever()` (same reasoning as `Match.Id`).
  Depends on T001.
- [X] T004 Generate the EF Core migration (`dotnet ef migrations add AddActivitySequence
  --project src/WorldCupScoreboard`) into `src/WorldCupScoreboard/Persistence/Migrations/`.
  Depends on T003.

**Checkpoint**: `Match.ActivitySequence`, `InvalidPageException`, and the persistence mapping
all exist and compile — User Story 1's tests and implementation can now be written.

---

## Phase 3: User Story 1 - Browse the full match history, a page at a time (Priority: P1) 🎯 MVP

**Goal**: Callers can retrieve matches (in-progress and finished) 10 at a time, most recently
active first — where "active" means created, score-updated, or finished — with out-of-range
pages returning empty and invalid page numbers rejected.

**Independent Test**: Create more matches than fit on one page, update and finish some of them,
request page 1 and verify it contains exactly the 10 most-recently-active matches in the right
order; request further pages and verify they continue correctly, with an out-of-range page
returning an empty result.

### Tests for User Story 1

> **Write these tests FIRST — confirm they FAIL before writing implementation (T012-T016)**

- [X] T005 [P] [US1] Write failing tests for pagination mechanics (FR-001, FR-003, FR-004,
  FR-005; Acceptance Scenarios 1-2, 4, 6, and the invalid-page-number Edge Case) in
  `tests/WorldCupScoreboard.Tests/GetHistoryPaginationTests.cs` — assert `InvalidPageException`
  for `page < 1`.
- [X] T006 [P] [US1] Write failing tests for activity-based ordering (FR-002; Acceptance
  Scenario 3, Edge Cases 1 and 3) in
  `tests/WorldCupScoreboard.Tests/GetHistoryOrderingTests.cs` — including a case where an
  older match is updated (or finished) after a newer one is created, and must then rank ahead
  of it. These tests will only pass once `StartMatch`/`UpdateScore`/`FinishMatch` are all
  updated to bump `ActivitySequence` (T012-T014).
- [X] T007 [P] [US1] Write failing tests for scope and read-only behavior (FR-006, FR-007;
  Acceptance Scenario 5, Edge Case 4) in
  `tests/WorldCupScoreboard.Tests/GetHistoryScopeTests.cs` — including a finished match still
  appearing (contrast with `004-live-summary`'s `GetSummary`, which excludes it), and that
  calling `GetHistory` doesn't change any match's data.

### Implementation for User Story 1

- [X] T008 [US1] Add `IEnumerable<Match> GetHistory(int page)` to
  `src/WorldCupScoreboard/IScoreboard.cs`, per `contracts/IScoreboard.md`.
- [X] T009 [US1] In `src/WorldCupScoreboard/Scoreboard.cs`, add a private `_nextActivitySequence`
  field, initialized to `1` in the constructor and then bumped past any existing match's
  `ActivitySequence` using the exact same `foreach`/`>=` comparison loop already used to seed
  `_nextId` (i.e., iterate `repository.GetAll()` and set `_nextActivitySequence =
  existing.ActivitySequence + 1` whenever `existing.ActivitySequence >= _nextActivitySequence`)
  — **not** `.Max()` and **not** `.OrderByDescending().FirstOrDefault()`, both of which either
  throw or need extra guarding on an empty repository; the `foreach`/`>=` pattern already
  handles the empty case for free (`/speckit-analyze` finding I1;
  see [[feedback-monotonic-counter-seeding]]). This is what makes a persisted history survive
  process restarts correctly.
- [X] T010 [US1] In `Scoreboard.StartMatch`, assign the newly created match's
  `ActivitySequence` (research.md §2). Same file as T009 — sequential, not parallel.
- [X] T011 [US1] In `Scoreboard.UpdateScore`, bump `ActivitySequence` on the match being
  updated, on success only (research.md §2). Same file as T010 — sequential.
- [X] T012 [US1] In `Scoreboard.FinishMatch`, bump `ActivitySequence` on the match being
  finished, on success only (research.md §2). Same file as T011 — sequential.
- [X] T013 [US1] Implement `Scoreboard.GetHistory` under the existing lock: throw
  `InvalidPageException` if `page < 1` (FR-005); otherwise order `repository.GetAll()` by
  `ActivitySequence` descending, `.Skip((page - 1) * 10).Take(10)`, return the result — no
  status filtering (FR-007), never throws for an out-of-range page (FR-004). Same file as
  T009-T012 — depends on all of them (T008 for the interface signature too).
- [X] T014 [US1] Run `dotnet test --filter FullyQualifiedName~GetHistory`; confirm T005-T007
  all pass. Then run the full suite (`dotnet test`) to confirm no regression in `001`-`004`. On
  any failure, apply constitution Principle II (reproduce → state the fix in one sentence →
  minimal fix → re-run the FULL suite) before proceeding.

**Checkpoint**: User Story 1 is complete, independently functional, and fully tested — this is
the MVP (and the only story in this feature). The brief's chosen extra feature is now done.

---

## Phase 4: Polish & Cross-Cutting Concerns

- [X] T015 Add a `history <page>` command to `demo/ScoreboardCli/Program.cs` (per constitution
  Principle V) — calls `scoreboard.GetHistory(page)` and prints each match (including its
  `Status`, since both in-progress and finished appear here) on its own line, in order; prints
  a clear rejection message on `InvalidPageException`, and a clear "no matches on this page"
  message for an empty (but valid) page. Update `PrintHelp`'s command list and
  manual-test-scenario walkthrough to include it, mirroring the existing entries.
- [X] T016 [P] Run `dotnet format` (or verify existing formatting) across
  `src/WorldCupScoreboard/` and `tests/WorldCupScoreboard.Tests/`.
- [X] T017 Walk through `specs/005-match-history/quickstart.md`'s manual validation steps (1-6)
  against the built library and the CLI demo's new `history` command, confirming every
  acceptance scenario holds end-to-end against real SQLite — including that `ActivitySequence`
  persists correctly across a CLI restart (start several matches, exit, relaunch, confirm
  `history 1` still reflects the correct order).

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: N/A — nothing to do, already satisfied by prior features.
- **Foundational (Phase 2)**: T001→T003→T004 sequential (each depends on the previous); T002
  independent, can run in parallel with T001. BLOCKS User Story 1 entirely.
- **User Story 1 (Phase 3)**: Depends on Foundational only. T005-T007 (tests) have no
  dependency on each other, only on Foundational. T008 (interface) is independent of
  T009-T013 (all in `Scoreboard.cs`, strictly sequential: T009→T010→T011→T012→T013).
- **Polish (Phase 4)**: Depends on User Story 1 being complete.

### Within User Story 1

- Tests (T005-T007) MUST be written and FAIL before their corresponding implementation
  (T008-T013).
- T009 (activity-sequence helper/seeding) before T010-T012 (the three call sites that use it)
  before T013 (`GetHistory` itself, which reads what those three now write).

### Parallel Opportunities

- T001 and T002 (Foundational) can run in parallel — different files.
- T005, T006, T007 (US1 tests) can run in parallel — three different files, each depending
  only on Foundational, not on each other.
- T016 (Polish) has no dependency on T015/T017 beyond both needing US1 complete — can run in
  parallel with either.
- **Note**: T009-T013 (all in `Scoreboard.cs`) and T003-T004 (Foundational persistence) are
  the only strictly sequential chains in this feature — more so than prior features, since this
  one edits existing methods rather than only adding new ones.

---

## Parallel Example: Foundational + User Story 1 Tests

```bash
# Foundational, in parallel:
Task: "Add ActivitySequence to Match.cs"
Task: "Create InvalidPageException.cs"

# Once Foundational is done, US1 tests in parallel:
Task: "Write failing pagination tests in tests/WorldCupScoreboard.Tests/GetHistoryPaginationTests.cs"
Task: "Write failing ordering tests in tests/WorldCupScoreboard.Tests/GetHistoryOrderingTests.cs"
Task: "Write failing scope tests in tests/WorldCupScoreboard.Tests/GetHistoryScopeTests.cs"
```

---

## Implementation Strategy

### MVP First (and Only) — User Story 1

1. Complete Phase 2: Foundational (T001-T004 — field, exception, mapping, migration)
2. Complete Phase 3: User Story 1 (T005-T014 — tests, then the sequential edits to
   `Scoreboard.cs`, then `GetHistory` itself)
3. **STOP and VALIDATE**: `dotnet test` green (including the full `001`-`004` suites),
   quickstart.md steps 1-6 confirmed manually, including the persistence-across-restart check
4. This is a shippable increment — `GetHistory` completes the brief's chosen extra feature;
   land it in its own distinct commit per CLAUDE.md/the brief
5. Complete Phase 4: Polish (T015-T017)

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions **and the brief itself**, this feature's commit(s) must
  be distinct from any other feature's — do not combine with unrelated changes. Do not commit
  without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing, then passing)
  before moving to the next task — no production code without a preceding failing test.
