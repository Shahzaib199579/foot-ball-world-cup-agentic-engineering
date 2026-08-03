---

description: "Task list for 001-start-match"
---

# Tasks: Start New Match

**Input**: Design documents from `/specs/001-start-match/`

**Prerequisites**: plan.md (amended for the persistence decision — see plan.md's "Amendment
(post-implementation, pre-commit)" note), spec.md, research.md, data-model.md,
contracts/IScoreboard.md, quickstart.md

**Tests**: Included — constitution Principle I (Test-First, NON-NEGOTIABLE) mandates a failing
test before every production-code change, so test tasks are not optional here.

**Organization**: Tasks are grouped by user story (spec.md: US1 = P1 Start a match, US2 = P2
Retrieve a match) to enable independent implementation and testing of each.

**Regeneration note**: This file was regenerated after `plan.md` was amended to incorporate
CLAUDE.md's Persistence decision (SQLite via EF Core, behind `IMatchRepository`), made *after*
`001-start-match` was first implemented against a plain `Dictionary<int, Match>`. Tasks already
completed under the original in-memory design are marked `[X]`; a `Dictionary`→`IMatchRepository`
retrofit is added as new tasks below (marked `[ ]`) rather than rewriting history. Nothing in
`src/`, `tests/`, or `demo/` has been touched by this regeneration — it only updates this file.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependency on an incomplete task)
- **[Story]**: Which user story this task belongs to (US1, US2)
- File paths are exact, per plan.md's Project Structure

## Phase 1: Setup

**Purpose**: Create the solution/project skeleton and the persistence package dependency.

- [X] T001 Create `WorldCupScoreboard.sln` at the repo root; create
  `src/WorldCupScoreboard/WorldCupScoreboard.csproj` (net9.0 class library) and
  `tests/WorldCupScoreboard.Tests/WorldCupScoreboard.Tests.csproj` (net9.0, xUnit); add both
  projects to the solution; add a project reference from the test project to the library
  project.
- [X] T002 Delete the default scaffold-generated class file(s) (e.g.
  `src/WorldCupScoreboard/Class1.cs`) left by the project templates in T001.
- [X] T003 Add `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`
  NuGet package references to `src/WorldCupScoreboard/WorldCupScoreboard.csproj` (per the
  amended plan.md's Primary Dependencies — needed by Phase 2's `Persistence/` tasks).

**Checkpoint**: `dotnet build` succeeds on an empty solution before any feature code is added.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: The data entities both user stories depend on, plus the persistence abstraction
(`IMatchRepository`, its EF Core/SQLite implementation, and a fake for tests) required by the
amended constitution Principle IV before `Scoreboard` can be implemented/refactored against it.
See research.md §3 for why `IScoreboard`/`Scoreboard` themselves are NOT created here.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T004 [P] Create `MatchStatus` enum with only the `InProgress` value in
  `src/WorldCupScoreboard/MatchStatus.cs` (per data-model.md — `Finished` is deliberately not
  declared; it belongs to spec 003-finish-match).
- [X] T005 [P] Create `Team` class with `Name` (string) and `Score` (int, defaults to 0) in
  `src/WorldCupScoreboard/Team.cs`.
- [X] T006 Create `Match` class with `Id` (int), `HomeTeam`/`AwayTeam` (Team), `ScheduledAt`
  (DateTime), `Location` (string), `Status` (MatchStatus) in `src/WorldCupScoreboard/Match.cs`
  (depends on T004, T005 for the `MatchStatus`/`Team` types).
- [X] T007 [P] Create `IMatchRepository` interface (`Add(Match)`, `GetById(int) : Match?`,
  `GetAll() : IEnumerable<Match>`, `Update(Match)`) in
  `src/WorldCupScoreboard/Persistence/IMatchRepository.cs` — the only persistence-facing type
  `Scoreboard` may depend on (constitution Principle IV, amended). Depends on T006 (`Match`).
- [X] T008 Create `ScoreboardDbContext` (EF Core `DbContext` with a `Matches` `DbSet<Match>`,
  SQLite provider configured via `OnConfiguring`/constructor options) in
  `src/WorldCupScoreboard/Persistence/ScoreboardDbContext.cs`. Depends on T003 (EF Core/SQLite
  package), T006 (`Match` entity).
- [X] T009 Implement `SqliteMatchRepository : IMatchRepository` over `ScoreboardDbContext` in
  `src/WorldCupScoreboard/Persistence/SqliteMatchRepository.cs`. Depends on T007, T008.
- [X] T010 Generate the initial EF Core migration (`dotnet ef migrations add InitialCreate
  --project src/WorldCupScoreboard`) into `src/WorldCupScoreboard/Persistence/Migrations/`.
  Depends on T008.
- [X] T011 [P] Create a fake `InMemoryMatchRepository : IMatchRepository` (plain in-memory
  collection, no EF Core/SQLite) in
  `tests/WorldCupScoreboard.Tests/Fakes/InMemoryMatchRepository.cs`, for unit tests to use
  instead of a real database (constitution Principle I). Depends on T007 only — can run in
  parallel with T008-T010.

**Checkpoint**: Foundation ready — `Match`/`Team`/`MatchStatus` exist, and the persistence
abstraction (`IMatchRepository` + real SQLite implementation + test fake) exists. User story
implementation can now begin/be refactored.

---

## Phase 3: User Story 1 - Start a match between two teams (Priority: P1) 🎯 MVP

**Goal**: Callers can start a new match between two distinct teams (score 0-0), with a recorded
location and scheduled date/time, rejected via a non-throwing `null` result on any conflict —
now backed by `IMatchRepository` rather than an in-process `Dictionary`.

**Independent Test**: Call `StartMatch` with two distinct team names, a date/time, and a
location; assert a non-null `Match` with a unique `Id`, `Status == InProgress`, and both scores
`0`. Call it again with a conflicting team or location/time; assert `null` and no side effect.

### Tests for User Story 1

> Original tests (T006-T009 in the prior tasks.md) already exist and pass against the
> `Dictionary`-backed `Scoreboard`. They must be updated, FIRST, to construct `Scoreboard` via
> the fake repository (T011) — confirm they still FAIL to compile/pass until T017 lands.

- [X] T012 [P] [US1] Update `tests/WorldCupScoreboard.Tests/StartMatchTests.cs` to construct
  `Scoreboard` with `new InMemoryMatchRepository()` (T011) instead of a parameterless
  constructor; confirm the successful-start assertions (FR-001, FR-002, FR-003; Acceptance
  Scenario 1) are otherwise unchanged. Depends on T011.
- [X] T013 [P] [US1] Update `tests/WorldCupScoreboard.Tests/StartMatchValidationTests.cs` the
  same way for the input-validation-rejection tests (FR-004). Depends on T011.
- [X] T014 [P] [US1] Update `tests/WorldCupScoreboard.Tests/StartMatchConflictTests.cs` the same
  way for the team-conflict, location/time-conflict, and no-side-effect-on-rejection tests
  (FR-005, FR-006, FR-008). Depends on T011.

### Implementation for User Story 1

- [X] T015 [US1] Create the `IScoreboard` interface with the `StartMatch` signature in
  `src/WorldCupScoreboard/IScoreboard.cs`, per `contracts/IScoreboard.md`. (Unaffected by the
  persistence retrofit — `IScoreboard`'s public method signatures do not change.)
- [X] T016 [US1] Refactor the `Scoreboard` class in `src/WorldCupScoreboard/Scoreboard.cs`:
  remove the private `Dictionary<int, Match>` store; add a constructor taking an
  `IMatchRepository`; replace direct dictionary reads/writes in `StartMatch` with
  `repository.Add`/`repository.GetAll` (for the FR-005/FR-006 conflict scans); keep the
  existing coarse lock (research.md §4) and monotonic ID counter (research.md §1) unchanged.
  Depends on T007 (interface must exist to compile against), T009 and T011 (both concrete
  implementations must exist so production and test code can each construct a `Scoreboard`).
- [X] T017 [US1] Run `dotnet test --filter FullyQualifiedName~StartMatch`; confirm T012-T014
  all pass against the repository-backed `Scoreboard`. On any failure, apply constitution
  Principle II (reproduce → state the fix in one sentence → minimal fix → re-run the FULL
  suite) before proceeding.

**Checkpoint**: User Story 1 is complete, independently functional, fully tested, and backed by
`IMatchRepository` rather than a bare dictionary — this is the MVP.

---

## Phase 4: User Story 2 - Retrieve a started match's details (Priority: P2)

**Goal**: Callers can read back a started match's recorded data by its match ID, via the same
repository abstraction.

**Independent Test**: Start a match, then call `GetMatch` with its `Id` and assert every recorded
field matches what was provided at start. Call `GetMatch` with an unknown `Id` and assert `null`.

### Tests for User Story 2

- [X] T018 [P] [US2] Update `tests/WorldCupScoreboard.Tests/GetMatchTests.cs` to construct
  `Scoreboard` with `new InMemoryMatchRepository()` (T011) instead of a parameterless
  constructor; confirm the retrieval assertions (FR-007; Acceptance Scenarios 1-2) are
  otherwise unchanged. Depends on T011.

### Implementation for User Story 2

- [X] T019 [US2] Add the `GetMatch` signature to `src/WorldCupScoreboard/IScoreboard.cs`, per
  `contracts/IScoreboard.md`. (Unaffected by the persistence retrofit.)
- [X] T020 [US2] Update `Scoreboard.GetMatch` in `src/WorldCupScoreboard/Scoreboard.cs` to look
  up via `repository.GetById(matchId)` under the existing lock (research.md §4) instead of a
  dictionary lookup, returning `null` if not found. Depends on T016 (same file, same
  constructor/field changes).
- [X] T021 [US2] Run `dotnet test --filter FullyQualifiedName~GetMatch`; confirm T018 passes.
  Then run the full suite (`dotnet test`) to confirm no regression in User Story 1.

**Checkpoint**: User Stories 1 and 2 are both independently functional against the
repository-backed `Scoreboard`.

---

## Phase 5: Polish & Cross-Cutting Concerns

- [X] T022 Update `demo/ScoreboardCli/Program.cs` to construct `Scoreboard` with a real
  `SqliteMatchRepository`/`ScoreboardDbContext` (backed by a local SQLite file or connection
  string) instead of the old parameterless constructor, per constitution Principle V (every
  feature must stay runnable/observable via the CLI demo in the same commit). Depends on T009,
  T016.
- [X] T023 Update `specs/001-start-match/quickstart.md`'s manual-validation code snippets
  (steps 1-3, 5) to construct `Scoreboard` via a repository (matching T016's new constructor)
  instead of `new Scoreboard()`, so the quickstart stays executable as written.
- [X] T024 [P] Run `dotnet format` (or verify existing formatting) across
  `src/WorldCupScoreboard/` (including the new `Persistence/` folder) and
  `tests/WorldCupScoreboard.Tests/` (including the new `Fakes/` folder).
- [X] T025 Walk through `specs/001-start-match/quickstart.md`'s manual validation steps (1, 2,
  3, 5) against the built library, using the CLI demo (T022) to confirm the acceptance
  scenarios still hold end-to-end with a real SQLite-backed repository, not just the fake used
  in unit tests. (Step 4 — a finished match freeing its slot — remains out of scope; it depends
  on `003-finish-match`.)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — T003 (EF Core/SQLite packages) can run any time, but
  must land before Phase 2's T008.
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS both user stories. T007/T011 (the
  repository interface and fake) in particular block every US1/US2 task below, since
  `Scoreboard`'s constructor now requires an `IMatchRepository`.
- **User Story 1 (Phase 3)**: Depends on Foundational only.
- **User Story 2 (Phase 4)**: Depends on Foundational; T020 also depends on US1's T016 (same
  file — `Scoreboard.cs`). T018 (test) has no such dependency beyond T011 and can be updated
  any time after Foundational.
- **Polish (Phase 5)**: Depends on both user stories being complete; T022/T025 additionally
  depend on T009 (real SQLite repository) since the CLI demo uses the production
  implementation, not the test fake.

### Within Each User Story

- Tests (T012-T014, T018) MUST be updated to compile against the new `IMatchRepository`-based
  constructor before their corresponding implementation task (T016, T020) is considered done.
- Foundational entities and persistence abstraction before `Scoreboard` refactor.
- US1's refactor (T016) before US2 extends the same file (T020).

### Parallel Opportunities

- T004 and T005 (Foundational) can run in parallel — different files, no dependency between
  `MatchStatus` and `Team`.
- T007 and T011 both depend only on T006, and T011 has no dependency on T008/T009/T010 — the
  fake repository can be built in parallel with the real EF Core/SQLite implementation.
- T012, T013, T014 (US1 test updates) can run in parallel — different files, same dependency
  (T011) only.
- T018 (US2 test update) can run in parallel with any US1 test-update task.
- T024 (Polish) can run in parallel with T025.

---

## Parallel Example: Foundational Persistence Layer

```bash
# Launch the real and fake IMatchRepository implementations together once T007 lands:
Task: "Implement ScoreboardDbContext + SqliteMatchRepository in src/WorldCupScoreboard/Persistence/"
Task: "Implement InMemoryMatchRepository fake in tests/WorldCupScoreboard.Tests/Fakes/"
```

---

## Implementation Strategy

### Retrofit First (Persistence), Then Resume Incremental Delivery

1. Complete Phase 1: Setup (T003 adds the EF Core/SQLite package reference)
2. Complete Phase 2: Foundational (T007-T011 establish `IMatchRepository` and both its
   implementations — this is the persistence retrofit itself)
3. Complete Phase 3: User Story 1 (T012-T017 — update tests, refactor `Scoreboard`, re-verify)
4. **STOP and VALIDATE**: `dotnet test` green, quickstart.md steps 1-3 confirmed manually against
   the SQLite-backed CLI demo
5. Complete Phase 4: User Story 2 (T018-T021)
6. Complete Phase 5: Polish (T022-T025)

### Incremental Delivery (unchanged shape, now persistence-aware)

1. Setup + Foundational → foundation ready, including the repository abstraction
2. Refactor User Story 1 onto `IMatchRepository` → test independently → MVP re-validated
3. Refactor User Story 2 onto `IMatchRepository` → test independently, confirm no US1 regression
4. Polish, including updating the CLI demo and quickstart.md to the new constructor shape

---

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Per CLAUDE.md's Working Conventions, this feature maps to one or a couple of small,
  reviewable commits (e.g., one per phase or one for the whole feature) — but do not commit
  without being asked, per standing instructions.
- Every checklist item above must be verified false→true (test written failing, then passing)
  before moving to the next task — no production code without a preceding failing test. For the
  persistence retrofit specifically, "failing" means the updated test files (T012-T014, T018)
  fail to compile/pass against the old `Dictionary`-backed `Scoreboard` until T016/T020 land.
