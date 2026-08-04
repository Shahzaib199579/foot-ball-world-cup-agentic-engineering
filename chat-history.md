# Chat History (working log)

Gitignored scratch file — freely edited/deleted while work is in progress. Final, reviewed
excerpts get hand-picked into `AI.md` at the end (see CLAUDE.md "Chat history capture
mechanism").

## 2026-08-03 — Bootstrap: tooling + Spec-Kit scaffold + chat-history setup

- Confirmed prereqs: `dotnet` 9.0.306 already installed; `uv`/`uvx` were missing, installed
  via `brew install uv` (0.12.1).
- Planned and ran (via plan mode approval) the Spec-Kit bootstrap:
  ```
  uvx --from git+https://github.com/github/spec-kit.git specify init --here --integration claude --script sh --force
  ```
- **Deviation from CLAUDE.md's documented target layout**: the current spec-kit CLI nests
  scaffolding under `.specify/` (`.specify/memory/constitution.md`, not root-level
  `memory/`) and installs commands as Claude **skills** at `.claude/skills/speckit-*`,
  invoked as `/speckit-constitution`, `/speckit-specify`, `/speckit-clarify`,
  `/speckit-plan`, `/speckit-tasks`, `/speckit-implement` (hyphenated) — not the
  dot-notation (`/speckit.constitution`) or root-level `memory/`/`specs/` layout CLAUDE.md
  describes. Likely CLAUDE.md was written against an older spec-kit release. Treating the
  installed tool's actual behavior as source of truth; flagged to the user rather than
  silently overriding CLAUDE.md's layout section.
- Verified scaffold contents (`.specify/`, `.claude/skills/speckit-*/SKILL.md`,
  `.specify/memory/constitution.md`) are legitimate unfilled templates, not something
  unexpected. `.claude/settings.local.json` untouched.
- Not yet committed — awaiting user review/go-ahead per "review every spec-kit/AI-generated
  file before accepting it into a commit."

## 2026-08-03 — Correction: CLAUDE.md updated to match actual installed Spec-Kit

- User asked to fix CLAUDE.md's guessed-before-setup command names/paths so the doc matches
  what `specify init --here --integration claude --script sh --force` actually installed.
- Edited "## Confirmed decisions" → Process bullet: `/constitution → /specify → /clarify →
  /plan → /tasks → /implement` replaced with `/speckit-constitution → /speckit-specify →
  /speckit-clarify → /speckit-plan → /speckit-tasks → /speckit-implement`; added a note
  that these are Claude Code skills under `.claude/skills/speckit-*`, not native slash
  commands; fixed the constitution artifact path from `memory/constitution.md` to
  `.specify/memory/constitution.md`.
- Edited "## Chat history capture mechanism" checkpoint list to the same `/speckit-*` names.
- Left "## Repo layout (target state)" untouched (already correct — shows `.specify/`).
- Deliberately did **not** touch two other stale references outside the requested scope:
  line ~100 (`AI.md` acceptance criteria — generic stage-name list, no path/slash) and line
  ~103 (`AI.md` acceptance criteria — `memory/constitution.md` path, same staleness as the
  fixed one, but in a section the user didn't list). Flagging for a follow-up pass if
  desired.
- Pending: once `/speckit-specify` actually runs, verify the real `specs/<NNN-name>/` path
  (root-level vs nested under `.specify/`) and correct CLAUDE.md if it differs.

## 2026-08-03 — Restructuring: finer-grained Spec-Kit decomposition (7 specs)

- User asked to decompose the single "Phase 1: core library" spec into 5 specs (one per
  brief operation) plus the 2 already-implied later phases, referenced via a "## Roadmap"
  section — but no such section existed anywhere in CLAUDE.md (confirmed via grep for
  `Roadmap`/`Phase 1/2/3`). Asked the user how to proceed; they chose to have me create the
  Roadmap section from scratch rather than pause.
- Added a new "## Roadmap" section (between "## What this repo is" and "## Confirmed
  decisions") with a table of 7 specs:
  `001-start-match`, `002-update-score`, `003-finish-match`, `004-live-summary`,
  `005-match-history` (Phase 1, brief), `006-scoreboard-api` (Phase 2, REST API, beyond the
  brief), `007-scoreboard-frontend` (Phase 3, Angular/React client, beyond the brief).
- Updated "## Repo layout (target state)" `specs/` line from a single
  `specs/001-live-football-scoreboard/` folder to the 7 numbered folders above.
- Added a line to "## Working conventions": one Spec-Kit feature ≈ one reviewable commit (or
  a couple small ones), noting this is how the brief's "distinct commit for the extra
  feature" requirement is satisfied naturally by spec `005-match-history` — no special-casing
  needed.
- Scope note: only the three sections above were touched, per the user's explicit
  instruction not to change anything else.

## 2026-08-03 — /speckit-constitution: filled Core Principles (4)

- Ran `/speckit-constitution` with 4 principles supplied verbatim by the user: I. Test-First
  (NON-NEGOTIABLE), II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE), III. Single-Concern
  Features, IV. Layered Architecture / Library-First.
- Template (`.specify/memory/constitution.md`) had 5 principle slots; only 4 were supplied,
  so the 5th slot was dropped rather than left half-filled (per skill instructions: respect
  the count the user specifies).
- User explicitly said: "Do not remove or alter any placeholder sections beyond filling in
  these four principles." Interpreted literally — left `[PROJECT_NAME]`, `[SECTION_2_NAME]`/
  `[SECTION_2_CONTENT]`, `[SECTION_3_NAME]`/`[SECTION_3_CONTENT]`, `[GOVERNANCE_RULES]`, and
  the `Version`/`Ratified`/`Last Amended` line completely untouched — still raw bracket
  placeholders. Did not bump the constitution version or set ratification dates, even though
  the skill's own workflow normally requires this on every amendment, since the user's
  literal instruction took precedence over the generic skill default.
- Prepended a Sync Impact Report (HTML comment) documenting: which principles were filled,
  the dropped 5th slot, and every deferred placeholder as a `TODO(...)` with rationale —
  this is additive documentation, not an alteration of the placeholder sections themselves.
- Deferred non-governance intent extracted from the input (per Scope Guard — not executed,
  just surfaced as Next Actions): choosing the Phase 2 API framework and the Phase 3
  Angular-vs-React framework are explicitly deferred to those specs' own `/speckit-plan`
  runs (`006-scoreboard-api`, `007-scoreboard-frontend`), not decided in the constitution.
- Not committed — no request to commit yet.

## 2026-08-03 — /speckit-specify: 001-start-match

- Ran `/speckit-specify` for Roadmap spec `001-start-match` from the user's freeform
  description (Match/Team classes, score 0-0, date/time + location tracking, "can't start a
  match at the same location and same time").
- Created `specs/001-start-match/spec.md`, `specs/001-start-match/checklists/requirements.md`,
  and `.specify/feature.json` (feature_directory: `specs/001-start-match`). No git branch
  created (no `.specify/extensions.yml`, so no `before_specify`/`after_specify` hooks ran).
- Translated the user's implementation-flavored request (classes/members/methods) into
  business-level entity language (Match, Team) for the spec, per Spec-Kit's "no
  implementation details" guideline — deferred actual C# class shape to `/speckit-plan`.
- Two [NEEDS CLARIFICATION] markers were raised and resolved via AskUserQuestion:
  - FR-003 (scheduling model): user chose "future scheduling supported" over the recommended
    "immediate-start only."
  - FR-006 (conflict-rule semantics): user chose the recommended "exact date/time equality"
    (no overlapping-window/duration concept).
- The FR-003 answer opened a second, more consequential question: "future scheduling
  supported" implies two lifecycle steps (schedule, then begin later) — which would be a
  second operation, in tension with the Single-Concern-Features constitution principle and
  the Roadmap's single-purpose framing of spec 001. Flagged this explicitly rather than
  silently resolving it either way, and asked the user to choose between keeping 001
  single-purpose vs. splitting into two specs (expanding the Roadmap to 8 specs).
  User chose to keep 001 single-purpose (recommended): starting a match records a
  (possibly future-dated) `ScheduledAt` as descriptive metadata, but always creates *and*
  activates the match in one call — no separate "begin a previously-scheduled match" action
  exists in this spec. A true two-step schedule-then-begin workflow was deliberately deferred
  to a possible future spec, not added to the current Roadmap.
- Checklist `requirements.md` now passes all items. Spec is ready for `/speckit-clarify`
  (optional) or `/speckit-plan`. Not committed yet.

## 2026-08-03 — /speckit-clarify: 001-start-match (3 questions)

- Ran the ambiguity/coverage taxonomy scan against `specs/001-start-match/spec.md`. Most
  categories were Clear; three gaps were high-impact enough to ask about (one question at a
  time, per protocol):
  1. FR-006 conflict scope: does the location+time rule block against all matches ever, or
     only currently in-progress ones? → User chose **in-progress only** (recommended) — once
     a match finishes, its location/time slot frees up again. (Otherwise a venue/kickoff-time
     combo could never be reused after a single historical match — clearly not intended.)
  2. Match identity: how is a specific match referenced for retrieval / by future specs
     (002-005)? → User chose **system-generated unique match ID**, assigned at start
     (recommended over team-name-pair identity, which breaks down for repeat fixtures/history).
  3. Failure signaling on a rejected start: exception vs non-throwing result? → User chose
     **non-throwing result** (recommended — matches the original request's literal "returns if
     successful" wording over the project's general "throws on invalid ops" convention).
- Integrated each answer into the spec immediately after acceptance (`## Clarifications`
  session log + updated FR-001, FR-006, FR-007, added FR-008, updated Key Entities, added
  acceptance scenarios for the finished-match-frees-slot case and the not-found retrieval
  case).
- Flagged a real divergence in the spec's Assumptions: CLAUDE.md's project-wide convention is
  "invalid operations throw," but this spec's start operation now explicitly does not throw.
  Noted as unresolved for specs 002/003 to confirm independently — not assumed settled here.
- Re-validated `checklists/requirements.md` against the updated spec: still 16/16 items
  passing, no regressions. Updated its Notes section to record the session.
- Spec is ready for `/speckit-plan`. Not committed yet.

## 2026-08-03 — /speckit-plan: 001-start-match

- Ran `.specify/scripts/bash/setup-plan.sh --json` (no extensions.yml, no hooks) — copied
  `plan.md` template, resolved `BRANCH=001-start-match` (note: no actual git branch exists,
  we're still on `main` — the script derives this name from `feature.json`/naming convention,
  not an actual checked-out branch; harmless for docs-only artifacts but worth knowing before
  any future automation assumes a real branch exists).
- Filled Technical Context directly from CLAUDE.md's already-Confirmed Decisions (.NET 9,
  xUnit, in-memory only, coarse locking) — no genuine unknowns there.
- Constitution Check: all 4 principles PASS, no violations, Complexity Tracking table N/A.
- Phase 0 `research.md`: 4 decisions — (1) reuse the monotonic sequence counter (already
  committed to for spec 004's ordering) as the `Match.Id`, rejecting `Guid`/composite-key
  identity; (2) `StartMatch` returns `Match?` (null on failure) rather than a `TryXxx` pattern
  or a new `Result<T>` wrapper — simplest shape satisfying the clarify session's non-throwing
  decision; (3) `IScoreboard` grows one method per spec (002-005 each add their own) rather
  than pre-declaring all 5 core methods now — avoids untested stub surface, honors Test-First
  + Single-Concern-Features; (4) restated the coarse-lock concurrency decision concretely.
- Phase 1: `data-model.md` (Match/Team/MatchStatus, deliberately declaring only the
  `InProgress` enum value — `Finished` deferred to spec 003, same untested-surface reasoning
  as decision 3), `contracts/IScoreboard.md` (StartMatch/GetMatch signatures + pre/post
  conditions), `quickstart.md` (build/test commands + manual walkthrough of each acceptance
  scenario; the "finished match frees its slot" scenario is flagged as not yet exercisable
  since `FinishMatch` doesn't exist yet).
- Post-Phase-1 Constitution re-check: still PASS, no new violations from the design artifacts.
- Not committed yet. No `src/`/`tests/` code created — that's `/speckit-tasks` +
  `/speckit-implement`'s job, per Test-First (a failing test must precede it).

## 2026-08-03 — /speckit-tasks: 001-start-match

- Ran `.specify/scripts/bash/setup-tasks.sh --json` (no extensions.yml) — all design docs
  available (research.md, data-model.md, contracts/, quickstart.md).
- Tests were treated as explicitly requested (not optional) — constitution Principle I
  (Test-First, NON-NEGOTIABLE) mandates a failing test before every production-code change,
  so test tasks are included and ordered before their implementation tasks throughout.
- Generated `tasks.md`: 18 tasks — Setup (T001-T002, create the .sln/.csproj skeleton, which
  doesn't exist yet), Foundational (T003-T005, MatchStatus/Team/Match — pure data, no
  behavior), User Story 1/MVP (T006-T012, tests then StartMatch implementation), User Story 2
  (T013-T016, tests then GetMatch — extends the same `IScoreboard.cs`/`Scoreboard.cs` files
  US1 created), Polish (T017-T018, formatting + quickstart walkthrough).
- Deliberately deferred creating `IScoreboard.cs`/`Scoreboard.cs` to US1's phase rather than
  Foundational — they carry actual behavior (StartMatch), and Foundational should only hold
  entities both stories need with zero behavior of their own (research.md §3's reasoning
  extended here).
- Minor plan.md consistency fix: updated its Source Code tree from one `StartMatchTests.cs`
  file to the 4 test files tasks.md actually specifies (split by concern: happy-path,
  validation, conflicts, retrieval) — plan.md's original single-file listing was illustrative,
  not binding, but kept it accurate rather than leaving a stale mismatch.
- tasks.md's own Notes section explicitly reiterates: do not commit without being asked, per
  standing instructions, even though CLAUDE.md's convention is one-feature-per-commit.
- Not committed. No code written yet — that's `/speckit-implement`.

## 2026-08-03 — /speckit-implement: 001-start-match (all 18 tasks)

- Checklist gate: `requirements.md` 16/16 passing — proceeded straight to implementation.
- Discovered the user has been committing checkpoints themselves outside this conversation
  (`da5950c` = raw spec-kit scaffold, `e0ae779` = my earlier `/speckit-constitution` fill —
  diffed both against my own context and confirmed byte-for-byte match, no surprise drift).
  Not a problem — matches CLAUDE.md's own "each stage's artifact is committed as its own
  commit" convention, just executed by the user rather than requested of me.
- Phase 1 (T001-T002): `dotnet new sln`/`classlib`/`xunit` scaffolded `WorldCupScoreboard.sln`,
  `src/WorldCupScoreboard/`, `tests/WorldCupScoreboard.Tests/` (net9.0 both), wired the test→lib
  project reference, deleted the default `Class1.cs`/`UnitTest1.cs` scaffold files. One NuGet
  warning (NU1900, an unreachable private CodeArtifact feed from prior machine config) — restore
  still succeeded via the public feed; non-blocking, ignored.
- Phase 2 (T003-T005): `MatchStatus` (enum, `InProgress` only), `Team` (`Name`, `Score` with an
  `internal set` so a future `002-update-score` can mutate it without exposing a public setter
  now), `Match` (Id/HomeTeam/AwayTeam/ScheduledAt/Location/Status).
  Build succeeds — checkpoint met.
- Phase 3 (T006-T012, US1/MVP): wrote all 4 US1 test files first (17 tests). Since C# is
  statically compiled, "confirm it fails for the right reason" required a minimal `IScoreboard`
  + `Scoreboard` stub (throwing `NotImplementedException`) to get the suite compiling and red
  — confirmed all 17 failed with `NotImplementedException` (not a compile error) before writing
  the real `StartMatch` (single lock, `Dictionary<int, Match>`, monotonic `_nextId` counter, all
  FR-002/004/005/006/008 validation). Reran — 17/17 green.
- Phase 4 (T013-T016, US2): same red→green pattern for `GetMatch` (2 tests). Full suite: 19/19
  green, no US1 regression.
- Phase 5 (T017-T018): `dotnet format --verify-no-changes` clean (exit 0). Manually executed
  quickstart.md's steps 1/2/3/5 via a throwaway scratch console app referencing the built
  library (not just re-pointing at the already-passing test suite) — all 10 checks passed,
  scratch project deleted afterward. Step 4 (finished match frees its slot) is correctly out of
  scope — no `FinishMatch` exists yet.
- Final state: `dotnet build` clean, `dotnet test` 19/19 passing, all 18 tasks marked `[X]` in
  `tasks.md`. Nothing committed by me — that decision is the user's, per standing instructions.

## 2026-08-03 — Amendment: added Constitution Principle V (Runnable Local Verification)

- User asked to add a 5th principle — "Runnable Local Verification (CLI Demo)": every
  feature needs a thin `demo/ScoreboardCli` console project (zero business logic, just calls
  `IScoreboard` and prints results) updated in the *same commit* as the feature; distinct
  from the Phase 2 API / Phase 3 frontend, exists from Phase 1 onward.
- Inserted as `### V.` after Principle IV in `.specify/memory/constitution.md`. Per explicit
  instruction ("do not touch the other 4 principles or any other section"), left everything
  else exactly as before — `[PROJECT_NAME]`, `[SECTION_2_NAME]`/`[SECTION_3_NAME]`,
  `[GOVERNANCE_RULES]`, and `Version`/`Ratified`/`Last Amended` are still the original
  deferred placeholders. Updated only the Sync Impact Report comment (additive documentation,
  not one of the protected sections) to record the new principle as "Added."
- **Flagging a real consequence, not yet acted on**: `001-start-match` was fully implemented
  and tested (19/19 passing) in the previous turn, but it predates Principle V and has no
  `demo/ScoreboardCli` coverage. Since nothing was committed yet, this now means
  `001-start-match` doesn't meet the *current* constitution's completion bar. Did not
  build the demo project myself — this request was scoped only to adding the principle text;
  raised it for the user to decide (retrofit a demo now before committing 001, or handle it
  as an explicit follow-up).

## 2026-08-03 — CLAUDE.md updated to reflect Principle V (demo/ScoreboardCli)

- Added `demo/ScoreboardCli/` to "## Repo layout (target state)"'s tree, placed after
  `tests/WorldCupScoreboard.Tests/` — net9.0 console app, thin CLI wrapping `IScoreboard`,
  run via `dotnet run --project demo/ScoreboardCli`, updated alongside every feature per
  constitution Principle V.
- Added a bullet to "## Working conventions": each feature's commit(s) include updating
  `demo/ScoreboardCli` to exercise the new operation — not done until it runs locally, not
  just passing tests.
- Added a checkbox to the README.md acceptance-criteria checklist: states how to run the CLI
  demo locally to manually verify each operation.
- Only these three additions were made; nothing else in CLAUDE.md touched. Still outstanding
  from the previous entry: `001-start-match` has no demo coverage yet and isn't committed —
  unresolved, not addressed by this docs-only update.

## 2026-08-03 — demo/ScoreboardCli added, covering 001-start-match

- Resolved the outstanding gap flagged in the two previous entries: scaffolded
  `demo/ScoreboardCli` (net9.0 console app, referencing `src/WorldCupScoreboard`, added to the
  solution), satisfying constitution Principle V for `001-start-match`.
- Built an interactive REPL (not one-shot CLI args) — necessary because the library is
  in-memory only; a single process invocation per command couldn't chain "start, then get,
  then try a conflicting start" scenarios the way a REPL session can. Commands: `start
  <home> <away> <location> [scheduledAt]`, `get <matchId>`, `ids` (session-local bookkeeping
  of started Ids, not a library call — no business logic added), `help`, `exit`/`quit`.
  Simple quote-aware tokenizer for multi-word args (e.g. `"Estadio Azteca"`) and for
  explicit-empty-string args (`""`) needed to manually trigger FR-002/FR-004 validation.
- `help` prints all 10 manual test scenarios mapped to the spec's FRs, with exact commands
  to type and expected outcomes — ran all 10 through the built CLI via piped stdin to verify
  each behaves as documented before presenting it (all passed, including the sequential-Id
  proof for FR-008: the rejected Netherlands/Portugal attempt never consumed an Id).
- Named the local per-iteration variable `commandArgs` instead of `args` — top-level
  statement programs have an implicit `args` (Main's parameter) in scope, and shadowing it
  inside the REPL loop was a compile error (CS0136), caught and fixed immediately.
- Noted a real limitation in the CLI's own help text: it can only ever pass an empty string,
  never a true null, for a missing argument — the null-argument branch of FR-002/FR-004 stays
  covered by the automated tests only, not manually reachable here.
- Verified after adding: `dotnet build` clean, `dotnet test` still 19/19 (no regression),
  `dotnet format --verify-no-changes` clean. Not committed — that's the user's call.

## 2026-08-03 — CLAUDE.md: added "Definition of Done" to Working conventions

- Added a `### Definition of Done` subsection after the existing "## Working conventions"
  bullets: a feature isn't committed until, in order, `/speckit-converge` reports nothing
  left to build, `/speckit-analyze` reports spec/plan/tasks consistency + constitution
  compliance, `dotnet build`/`dotnet test` both succeed with zero failures, the feature is
  manually run via `dotnet run --project demo/ScoreboardCli` (Principle V), and every
  `tasks.md` item is checked off.
- **Consequence, not yet acted on**: `001-start-match` has never had `/speckit-converge` or
  `/speckit-analyze` run against it. It now can't be considered committable under this new
  gate until both run clean, in addition to the already-flagged demo-coverage requirement
  (which is now satisfied — see the previous entry). Flagging, not running them unprompted.
- Only this one subsection was added; nothing else in CLAUDE.md touched.

## 2026-08-03 — Amendment: Principle IV extended with persistence abstraction (IMatchRepository)

- Extended (not replaced) constitution Principle IV — appended a paragraph requiring
  persistence behind `IMatchRepository`, with `Scoreboard`'s business logic never depending
  on EF Core/SQLite directly, and unit tests using a fake/in-memory `IMatchRepository`.
  Principles I, II, III, V untouched, per explicit instruction.
- Updated the Sync Impact Report comment with a new "Amended principles" category
  (distinct from the existing "Modified principles" list from the initial fill) documenting
  this as a later amendment.
- CLAUDE.md: added a **Persistence** bullet to "## Confirmed decisions" (after Concurrency,
  before Process) — SQLite via EF Core from spec 001 onward, behind `IMatchRepository`,
  explicitly flagged (like the .NET-over-Java deviation) as beyond the brief's "simple
  library" framing, to be documented in README.md. Added a `Persistence/` sub-entry under
  `src/WorldCupScoreboard/` in "## Repo layout (target state)". Added a bullet to "## Working
  conventions": unit tests use a fake/in-memory `IMatchRepository`.
- **Significant flag, not acted on**: this reverses the project's prior "in-memory-only"
  framing. Two concrete consequences I did not address, since this request was scoped to
  docs only:
  1. `001-start-match`'s actual implementation (`Scoreboard.cs`) stores matches directly in
     a `Dictionary<int, Match>` with no `IMatchRepository` abstraction at all — it no longer
     complies with the amended Principle IV. Combined with the Definition of Done gate added
     last turn, `/speckit-analyze` would very likely flag this the moment it's run.
  2. Two other CLAUDE.md lines still describe the old framing and weren't touched, since
     they weren't in the requested scope: the README acceptance-criteria bullet asking to
     document "in-memory-only vs persistence" as a trade-off, and the **Concurrency** bullet
     (coarse-grained in-process locking), which was designed around direct in-memory access
     and may need reconsidering once a real DB and repository abstraction are in the picture.
  Not fixing either without being asked — raising both for a decision.

## 2026-08-03 — Saved a cross-session memory checkpoint ("foot-ball-score-board-1")

- User asked to persist current progress + outstanding/blocking issues outside this
  conversation, so a future session can resume just by saying "foot-ball-score-board-1".
- Wrote `foot-ball-score-board-1.md` (type: project) to this project's memory directory
  (`~/.claude/projects/<encoded-repo-path>/memory/`), plus a `MEMORY.md` index (neither
  existed yet for this project). Content mirrors this file's running log at a condensed
  level: Roadmap state (001 done, 002-007 not started), the 5 ratified constitution
  principles, the Definition of Done gate, and the 5-item outstanding/blocking list from the
  previous entry (persistence non-compliance in `Scoreboard.cs`, `/speckit-converge` and
  `/speckit-analyze` never run, the two untouched "in-memory-only" CLAUDE.md spots, nothing
  committed by the assistant yet).
- This memory is explicitly a pointer, not a replacement for `CLAUDE.md`/
  `.specify/memory/constitution.md` (durable, committed) or this file (`chat-history.md`,
  gitignored/free-editing) — noted that in the memory itself so a future session re-verifies
  against those rather than trusting a possibly-stale snapshot.

## 2026-08-03 — Saved verbatim prompt log for CLAUDE.md/constitution.md edits

- User asked to log, in memory, the exact prompts that produced every `CLAUDE.md` and
  `.specify/memory/constitution.md` edit this session — source material for `AI.md`'s
  required prompt-history section later. Wrote `foot-ball-score-board-prompt-log.md`
  (type: project) with all 7 prompts verbatim, in order: (1) fix guessed Spec-Kit
  command-names/paths, (2) decompose Phase 1 into 5 specs + add Roadmap, (3) the
  `/speckit-constitution` args filling principles I-IV, (4) add Principle V, (5) reflect
  Principle V in repo layout/working conventions/README checklist, (6) add Definition of
  Done, (7) extend Principle IV with `IMatchRepository`/persistence + reflect it in
  CLAUDE.md.
- Practical implication surfaced, not yet acted on: the guidance session says it already
  handed the user prompts for the persistence retrofit (re-run `/speckit-plan` for 001, then
  `/speckit-tasks`, `/speckit-analyze`, `/speckit-implement`) — none of those have been run in
  *this* session yet. If the user pastes something that reads like a prepared retrofit prompt
  next, that's very likely what it is.

## 2026-08-03 — Amended `specs/001-start-match/plan.md` for the persistence decision

- User caught up on the `foot-ball-score-board-1` checkpoint, then asked to amend
  `plan.md` (only — explicitly not `tasks.md` yet) to incorporate the Persistence decision
  made after `001-start-match` was already implemented: replace the plain
  `Dictionary<int, Match>` design with `IMatchRepository`, an EF Core `DbContext`, and a
  SQLite implementation under `src/WorldCupScoreboard/Persistence/` and
  `Persistence/Migrations/`, plus a fake in-memory `IMatchRepository` for unit tests;
  `Scoreboard` must depend only on `IMatchRepository`, never EF Core directly, per
  constitution Principle IV.
- Edited three sections of `plan.md`:
  1. **Technical Context**: `Primary Dependencies` now lists EF Core + SQLite provider
     behind `IMatchRepository`; `Storage` now describes SQLite via EF Core (was "N/A —
     in-memory only") and adds an explicit amendment note explaining the plan originally
     said in-memory-only, and why it changed.
  2. **Constitution Check**: Principle IV row updated to reflect the amended gate text
     (persistence abstracted behind `IMatchRepository`) and cites where the abstraction/
     concrete impl each live; added an "Amendment (post-implementation, pre-commit)"
     paragraph after the Post-Phase-1 re-check explicitly flagging that this plan was
     updated *after* the code already existed against a `Dictionary`, so the Constitution
     Check describes the target design, not verified-in-place code.
  3. **Project Structure**: added `Persistence/` (`IMatchRepository.cs`,
     `ScoreboardDbContext.cs`, `SqliteMatchRepository.cs`, `Migrations/`) under
     `src/WorldCupScoreboard/`, and `tests/.../Fakes/InMemoryMatchRepository.cs`; extended
     the `Structure Decision` paragraph to say why `Persistence/` lands at spec 001 instead
     of later (so specs 002-005 build on the abstraction without rework).
- Did not touch `tasks.md`, `data-model.md`, `contracts/IScoreboard.md`, `research.md`, or
  any source/test code — scope was plan.md only, per explicit instruction. The actual
  `Scoreboard.cs` refactor to `IMatchRepository` is still outstanding (tracked in
  `foot-ball-score-board-1` memory's "Outstanding" list) and will need its own
  `/speckit-tasks` pass once tasks.md is updated to match.

## 2026-08-03 — Ran `/speckit-tasks` for 001-start-match (persistence retrofit)

- User asked to run `/speckit-tasks` for `001-start-match` to regenerate `tasks.md` against
  the plan.md amendment above. No `.specify/extensions.yml` exists, so no before/after-tasks
  hooks fired.
- Read `spec.md`, `data-model.md`, `research.md`, `quickstart.md`, `contracts/IScoreboard.md`,
  the current `tasks.md`, and the actual file listing under `src/`, `tests/`, `demo/` to see
  what's really on disk (all of 001's original Dictionary-backed implementation already
  exists and its original T001-T018 were all checked off).
- Regenerated `specs/001-start-match/tasks.md`: kept every already-completed task ([X]) that
  the persistence retrofit doesn't invalidate (sln/csproj setup, `MatchStatus`/`Team`/`Match`,
  `IScoreboard`'s two method signatures), and added new `[ ]` tasks for the retrofit itself:
  - Phase 1: T003 adds the EF Core Sqlite + Design NuGet packages.
  - Phase 2 (Foundational): T007 `IMatchRepository` interface, T008 `ScoreboardDbContext`,
    T009 `SqliteMatchRepository`, T010 initial EF Core migration, T011 a fake
    `InMemoryMatchRepository` test double (parallel to T008-T010, both depend only on T007).
  - Phase 3 (US1): T012-T014 update the three existing StartMatch test files to construct
    `Scoreboard` via the fake repository instead of a parameterless constructor; T016 is the
    actual `Scoreboard.cs` refactor (drop the `Dictionary`, take `IMatchRepository` in the
    constructor, route `StartMatch` through it) — explicitly marked as depending on both T009
    and T011 so prod and test code can each construct a `Scoreboard`.
  - Phase 4 (US2): T018 updates `GetMatchTests.cs` the same way; T020 updates
    `Scoreboard.GetMatch` to call `repository.GetById` instead of a dictionary lookup.
  - Phase 5 (Polish): added T022 (update `demo/ScoreboardCli/Program.cs` to wire the real
    `SqliteMatchRepository`, per constitution Principle V) and T023 (update quickstart.md's
    `new Scoreboard()` snippets to the new constructor shape) ahead of the pre-existing
    format/manual-walkthrough tasks (renumbered T024-T025).
- Added an explicit "Regeneration note" near the top of the file explaining why some tasks
  are `[X]` (pre-persistence-decision work, still valid) and why others are new `[ ]` tasks
  (the retrofit) rather than silently rewriting history.
- Did not touch any source/test/demo code, `spec.md`, `data-model.md`, `research.md`,
  `contracts/IScoreboard.md`, or `quickstart.md` itself — only `tasks.md` was regenerated,
  per the skill's stated scope. Actually implementing T003/T007-T011/T012-T014/T016/T018/
  T020/T022-T025 is still outstanding — next natural step is `/speckit-analyze` to check
  this tasks.md against the amended plan.md and spec.md before implementing.

## 2026-08-03 — Ran `/speckit-analyze` for 001-start-match

- Read-only cross-artifact check across `spec.md`, `plan.md` (amended), `tasks.md`
  (regenerated), `data-model.md`, `research.md`, `contracts/IScoreboard.md`,
  `quickstart.md`, and `.specify/memory/constitution.md`. No `.specify/extensions.yml`, so
  no pre/post-analyze hooks fired.
- 4 findings, 0 CRITICAL: **E1** (HIGH, later retracted — see below) flagged
  `IMatchRepository.Update` as unused/untested surface in this feature, echoing research.md
  §3's stated policy against pre-declaring methods ahead of need; **A1** (MEDIUM) flagged
  that the SQLite storage location/connection string for `ScoreboardDbContext` (T008) and
  the CLI demo (T022) is unspecified; **I1** (LOW, already tracked) flagged that
  `quickstart.md`'s snippets still call `new Scoreboard()` with no repository argument,
  which won't compile once T016 lands — no new action needed, T023 already covers it; **A2**
  (LOW, informational) flagged the constitution's still-unresolved template placeholders
  (`[PROJECT_NAME]`, etc.) — explicitly an intentional deferral per CLAUDE.md, not a defect.
- Coverage: 11/11 requirements (8 FR + 3 SC) had ≥1 mapped task; 0 duplication; 0
  unmapped-without-explanation tasks. Offered optional remediation edits for E1/A1 at the
  end; user did not request them in this turn.

## 2026-08-03 — Retracted finding E1 after reading the brief PDF and cross-checking existing docs

- User asked to (1) check `~/Downloads/Coding Exercise version 2.01.pdf` for whether
  `IMatchRepository.Update` is genuinely needed by a future spec, and (2) check whether this
  had already been documented somewhere before treating it as a fresh finding.
- Read the PDF: "Update the score" is Core Operation #2 of the brief's required 4 core
  operations (p.4) — confirms `IMatchRepository.Update` is needed by roadmap spec
  `002-update-score` (and likely `003-finish-match`, which also mutates an existing match's
  status), not speculative.
- Grepped `specs/001-start-match/*.md`, `CLAUDE.md`, and the constitution for "Update":
  found `plan.md:112` already documented `IMatchRepository.cs` as an
  "Add/Get/GetAll/Update abstraction" — from my own earlier plan.md amendment — so the
  future need had already been written down, just not cross-referenced against the brief
  during the `/speckit-analyze` pass.
- Retracted E1's HIGH severity down to informational/no-action: explained that research.md
  §3's "don't pre-declare untested methods" concern is scoped to `IScoreboard` (the public
  library contract exposed to callers), not `IMatchRepository` (an internal persistence
  abstraction) — defining a repository's full CRUD shape upfront so later specs can consume
  it without another interface change is normal repository-pattern practice, not scope
  creep. The only real artifact — `SqliteMatchRepository.Update` needing an implementation
  body in T009 before `002` tests it — is an unavoidable consequence of C# interface
  implementation, not untested *behavior* shipping ahead of need. A1/I1/A2 stand as
  originally reported.

## 2026-08-03 — Ran `/speckit-converge` for 001-start-match

- Read-only assessment of the actual codebase against `spec.md`/`plan.md`/`tasks.md` intent
  (constitution as governing constraint). No `.specify/extensions.yml`, so no pre/post-hooks.
- Verified via direct file reads that the code is completely unchanged since the persistence
  decision: `WorldCupScoreboard.csproj` still has no EF Core reference,
  `src/WorldCupScoreboard/Scoreboard.cs` still uses a plain `Dictionary<int, Match>`,
  `demo/ScoreboardCli/Program.cs` still constructs `new Scoreboard()` with no repository, and
  no `Persistence/`/`Fakes/` folders exist anywhere in `src/`, `tests/`, or `demo/` — grepped
  for `IMatchRepository`/`InMemoryMatchRepository` across all three and got zero hits.
- Outcome: **Converged** — zero actionable findings appended, `tasks.md` left byte-for-byte
  unchanged. Reasoning: the code does violate the amended constitution Principle IV right
  now (`Scoreboard` depends on `Dictionary`, not `IMatchRepository` — a real, CRITICAL-grade
  gap), but that exact gap is already fully and specifically tracked by the unchecked tasks
  already sitting in `tasks.md` (T003, T007-T025) from the earlier `/speckit-tasks`
  regeneration. Converge only appends *new*, currently-untracked work; since every gap found
  maps 1:1 onto an existing task, there was nothing new to add.
- Explicitly flagged the distinction to the user: "converged" here means task-coverage is
  complete, not that the feature is done — most of T003-T025 is still unimplemented. Next
  recommended step is `/speckit-implement` to actually execute those tasks, with a follow-up
  `/speckit-converge` afterward to catch any real implementation-time drift.

## 2026-08-03 — Ran `/speckit-implement` for 001-start-match (executed the persistence retrofit, T003-T025)

- Checklist gate: `specs/001-start-match/checklists/requirements.md` — 12/12 items pass, no
  stop-and-ask needed. No `.specify/extensions.yml`, so no pre/post-implement hooks fired.
- **NuGet blocker**: the machine's global `~/.nuget/NuGet/NuGet.Config` has a work CodeArtifact
  feed (`cko-packages`) with an expired auth token, returning 401 and aborting `dotnet add
  package` even though nuget.org itself resolved fine. Worked around with
  `--source https://api.nuget.org/v3/index.json` on each `dotnet add package` call rather than
  editing the global NuGet config (out of scope, shared-machine file). Also had to pin
  `Microsoft.EntityFrameworkCore.Sqlite`/`.Design` to **9.0.10** explicitly — the unpinned
  `dotnet add package` default resolved 10.0.10, which targets net10.0 and is incompatible with
  this project's net9.0.
- **T003**: added `Microsoft.EntityFrameworkCore.Sqlite`/`.Design` 9.0.10 to
  `WorldCupScoreboard.csproj`.
- **T007-T009**: added `Persistence/IMatchRepository.cs` (`Add`/`GetById`/`GetAll`/`Update`),
  `Persistence/ScoreboardDbContext.cs` (owns `Matches` `DbSet<Match>`, `OwnsOne` for
  `HomeTeam`/`AwayTeam`), `Persistence/SqliteMatchRepository.cs`. Also added
  `Persistence/ScoreboardDbContextFactory.cs` (`IDesignTimeDbContextFactory`, not in the original
  task list) — needed so `dotnet ef migrations add` has a way to construct the context at design
  time, since `ScoreboardDbContext`'s only constructor takes `DbContextOptions`. This factory also
  became the one place the SQLite connection string (`Data Source=scoreboard.db`) is defined,
  resolving `/speckit-analyze`'s A1 finding about the storage path being unspecified — Program.cs
  and the factory now both reference `ScoreboardDbContextFactory.DefaultConnectionString`.
- **T010**: generated the `InitialCreate` migration via `dotnet ef migrations add` (global
  `dotnet-ef` tool is 8.0.7, one major behind the 9.0.10 packages — worked anyway, just printed a
  version-mismatch warning each run). Caught and fixed a real bug pre-emptively: EF Core's
  convention default made `Match.Id` a database-autoincrement column
  (`Sqlite:Autoincrement`), which would fight the app's own monotonic ID counter in
  `Scoreboard`. Added `match.Property(m => m.Id).ValueGeneratedNever()` to
  `ScoreboardDbContext.OnModelCreating` and regenerated the migration before this could ever
  reach a test/demo run.
- **T011-T014, T018**: added `tests/.../Fakes/InMemoryMatchRepository.cs`; updated
  `StartMatchTests.cs`, `StartMatchValidationTests.cs`, `StartMatchConflictTests.cs`,
  `GetMatchTests.cs` to construct `Scoreboard` via `new InMemoryMatchRepository()`.
- **T016**: refactored `Scoreboard` to take `IMatchRepository` in its constructor, replacing the
  `Dictionary` with `_repository.Add`/`GetAll`. Added one thing not spelled out verbatim in
  tasks.md but necessary for correctness once storage is real and can outlive a process: the
  constructor now seeds `_nextId` from `repository.GetAll().Max(id) + 1` (or `1` if empty) instead
  of always starting at `1`, so restarting the CLI against an existing `scoreboard.db` doesn't
  reissue IDs already in use.
- **Real bug found and fixed via actual EF Core testing, not just code review**: the first
  end-to-end CLI run (`dotnet run --project demo/ScoreboardCli`) showed `Location` coming back
  empty and `ScheduledAt` coming back as `0001-01-01` after a real SQLite round-trip, even though
  `HomeTeam`/`AwayTeam`/`Id` loaded correctly. Root cause: `Match`'s original get-only auto-properties
  had no setter for EF to write through after using the parameterless constructor EF needs
  (the public 5-arg constructor can't be used by EF because it can't bind owned-navigation
  constructor parameters like `homeTeam`/`awayTeam`). `Id`/owned `Team` navigations have
  special-cased EF materialization paths that worked anyway; plain scalar properties
  (`Location`, `ScheduledAt`) didn't and silently kept the placeholder defaults the private
  constructor set. Fixed by changing `Match.Id`/`HomeTeam`/`AwayTeam`/`ScheduledAt`/`Location`
  and `Team.Name` to `{ get; internal set; }` — consistent with the existing `Status`/`Score`
  pattern already in the codebase — then regenerated the `InitialCreate` migration (the model
  snapshot was now stale) and re-verified with a fresh `scoreboard.db`. Re-ran the full CLI
  walkthrough twice across two separate process runs to confirm both correct data round-tripping
  and correct `_nextId` continuation (second run's new match got Id=3 after a prior run's 1, 2).
- **T022**: `demo/ScoreboardCli/Program.cs` now builds a `ScoreboardDbContext` via
  `DbContextOptionsBuilder<ScoreboardDbContext>().UseSqlite(...)`, calls
  `dbContext.Database.Migrate()`, and constructs `Scoreboard(new SqliteMatchRepository(dbContext))`;
  disposes `dbContext` on both exit paths (the `exit`/`quit` command and falling out of the REPL
  loop on EOF).
- **T023**: updated `quickstart.md`'s manual-validation snippets to construct `Scoreboard` via
  `ScoreboardDbContext`/`SqliteMatchRepository` (mirroring Program.cs) instead of the stale
  `new Scoreboard()`.
- **T024/T025**: `dotnet format --verify-no-changes` clean; walked all of quickstart.md's manual
  scenarios (1-3, 5) through the real CLI demo against actual SQLite, confirmed correct behavior,
  then deleted the scratch `scoreboard.db` test artifact. Added `scoreboard.db`/`-shm`/`-wal` to
  `.gitignore` (runtime-generated, shouldn't be committed).
- Final state: `dotnet build` and `dotnet test` both clean (19/19 passing) against the
  repository-backed `Scoreboard`; all 25 tasks in `specs/001-start-match/tasks.md` marked `[X]`.
  Nothing has been committed yet — still pending the user's explicit request per standing
  instructions.

## 2026-08-03 — Saved progress to memory, then added a project-scoped `nuget.config`

- Saved two memories after the implement pass: updated `foot-ball-score-board-1.md` in place
  (001-start-match + persistence retrofit fully done, build/test green, six concrete
  decisions made this session, nothing committed yet) and added a new
  `feedback-nuget-codeartifact-401.md` covering the expired global CodeArtifact token
  workaround and the EF Core version-pinning note — flagged as a machine-level issue likely
  to recur on unrelated .NET projects, not specific to this repo. Updated `MEMORY.md`'s index
  to point to both.
- User then asked to add `nuget.config` at the repo root (exact contents supplied verbatim:
  `<clear />` + `nuget.org` only) to scope restore to nuget.org for this project permanently,
  instead of passing `--source` on every `dotnet add package` call — without touching the
  global `~/.nuget/NuGet/NuGet.Config` (shared machine file, not this project's to fix).
- Created the file exactly as given. Verified: `git check-ignore -v nuget.config` confirms
  it's **not** gitignored (correct — it's project config, should be committed); `dotnet
  nuget list source` now shows only `nuget.org` registered (the dead `cko-packages`
  CodeArtifact feed no longer appears at all for this project); `dotnet restore` and `dotnet
  build` both succeed with no `--source` flag and — bonus — the earlier NU1900
  vulnerability-lookup warnings (from the unreachable CodeArtifact feed) are gone too, since
  it's no longer a configured source for this repo. `dotnet test` still 19/19 passing.
- Updated `feedback-nuget-codeartifact-401.md` with a dated addendum: this repo now has a
  permanent fix via `nuget.config`, not just a per-command workaround; the original
  `--source` workaround still applies to *other* .NET projects on this machine that lack
  such a file.
- `nuget.config` is currently untracked (`git status` shows `?? nuget.config`) — not
  committed yet, same as the rest of `001-start-match`'s work, pending the user's explicit
  go-ahead.

## 2026-08-03 — Ran `/speckit-specify` for 002-update-score

- User input verbatim: "002-update-score. It should be possible to update the score for each
  team in the match. Score can only go up and never down."
- Created `specs/002-update-score/spec.md` (sequential numbering, next available per
  `.specify/init-options.json`), updated `.specify/feature.json` to point at it.
- Wrote a single P1 user story (update the score of an in-progress match) with 5 acceptance
  scenarios, 4 edge cases, 7 FRs, 3 measurable Success Criteria, and an Assumptions section
  documenting every design call made without asking: absolute combined per-match update (not
  per-team, not deltas), "never down" = non-decrease not strict-increase, rejection via a
  raised error (not a non-throwing result like `001-start-match`'s own carve-out), and
  already-finished-match rejection specified now even though `MatchStatus.Finished` doesn't
  exist until `003-finish-match`.
- **Flagged a real conflict rather than silently resolving it**: the user's instruction
  ("score can only go up and never down") directly contradicted CLAUDE.md's existing
  Confirmed Decisions text ("Not enforcing monotonic non-decrease — keeps the library
  simple"). Followed the user's explicit instruction for the spec itself, documented it as a
  noted divergence in Assumptions, but did **not** edit CLAUDE.md unilaterally — surfaced it
  in the completion report for the user to decide.
- Checklist (`specs/002-update-score/checklists/requirements.md`): 16/16 items pass, zero
  `[NEEDS CLARIFICATION]` markers — every ambiguity had a reasonable, well-precedented
  default (existing CLAUDE.md conventions or the brief's own worked example).

## 2026-08-03 — Updated CLAUDE.md's Confirmed Decisions, then ran `/speckit-clarify` for 002-update-score

- User asked to update CLAUDE.md to match the new spec's behavior, then run `/speckit-clarify`.
- Edited CLAUDE.md's "Validation rules" bullet: replaced "Not enforcing monotonic non-decrease
  — keeps the library simple" with explicit non-decrease-enforced wording (equal accepted,
  decrease rejected) attributed to `002-update-score`'s explicit brief; added a new bullet
  stating rejection raises an error, consistent with the existing general throwing convention
  and distinct from `001-start-match`'s own non-throwing carve-out.
- `/speckit-clarify` for `002-update-score`: no `.specify/extensions.yml`, so no hooks. Ran
  the full ambiguity/coverage taxonomy scan against `spec.md` — every category came back
  Clear; the prior `/speckit-specify` pass had already resolved every genuinely open design
  choice as a documented Assumption. **Zero questions asked** — explicitly reported "no
  critical ambiguities detected" per the skill's own behavior rule for full coverage, rather
  than manufacturing a question to look thorough.
- One real fix made during the scan (not a formal Q&A): the spec's "Noted divergence from
  CLAUDE.md" Assumptions bullet was now stale — it still said CLAUDE.md *should be* updated,
  written before the CLAUDE.md edit above in this same turn. Corrected it to state the
  divergence is now reconciled, dated 2026-08-03.
- Checklist re-validated: still 16/16 passing, no state changes (the fix above was wording,
  not a structural/ambiguity resolution).
- Next suggested step: `/speckit-plan` for `002-update-score`.

## 2026-08-03 — Manually broadened 002-update-score's FR-002 to reject letters/special characters

- User added a requirement directly (no `/speckit-*` command invoked): score updates must also
  reject letters and special characters, not just negative numbers — the previous
  `/speckit-specify` pass had explicitly scoped malformed/non-numeric input as *out of scope*
  for this spec (caller's API/UI boundary concern instead).
- Edited `specs/002-update-score/spec.md` in place: broadened **FR-002** from "reject a negative
  integer" to "reject any value that isn't a valid non-negative integer — negative numbers,
  letters, special characters"; added **Acceptance Scenario 5** (letters/special characters
  rejected, e.g. `"two"`, `"2-1"`, `"2.5"`, `"2!"`), renumbering the old nonexistent-match-ID
  scenario to 6; replaced the now-contradicted "out of scope" Edge Case bullet with one stating
  the rejection explicitly; broadened **SC-002** to cover malformed values alongside decreases.
- Added a new Assumptions bullet flagging a real nuance: given CLAUDE.md's confirmed C# stack,
  typing the score parameters as `int` makes "letters" a **compile-time** type error, not a
  runtime case this library's own tests can exercise — the requirement is still stated
  technology-agnostically because it constrains the contract (integers, not strings) and because
  `006-scoreboard-api` (which will parse raw HTTP/JSON) must perform this exact rejection at its
  own boundary, honoring this spec's FR-002.
- Did not re-run `/speckit-clarify`/`/speckit-analyze` for this change — it was a direct,
  unambiguous edit per explicit user instruction, not a fresh ambiguity needing a Q&A pass.
  Checklist (`checklists/requirements.md`) not re-validated this turn — no new ambiguity was
  introduced, so no expected state change; worth a quick re-check before `/speckit-plan`.

## 2026-08-03 — Ran `/speckit-plan` for 002-update-score

- No `.specify/extensions.yml`, so no pre/post-plan hooks. Generated `plan.md`, `research.md`,
  `data-model.md`, `contracts/IScoreboard.md`, `quickstart.md` under
  `specs/002-update-score/`.
- **Key design decisions** (research.md): `UpdateScore(int matchId, int homeScore, int
  awayScore) : Match` — returns the updated `Match` on success (mirrors `StartMatch`'s
  "return what you just did" pattern) and throws on failure, per the spec's own
  already-decided throwing convention. Two new exception types under a new
  `Exceptions/` folder — `MatchNotFoundException` (FR-005) and `InvalidScoreException`
  (FR-002/FR-003) — deliberately split rather than one generic exception, so a future
  `006-scoreboard-api` can map them to different HTTP status codes without a breaking
  change later. `MatchNotFoundException` is written generically (keyed on match ID) so
  `003-finish-match` can reuse it. Validation order: resolve+confirm in-progress match
  first (FR-005), then validate both new scores against current scores (FR-002/FR-003) —
  decrease can't be evaluated without a resolved match. Atomicity (FR-004): validate both
  new scores fully before mutating either `Team.Score`, avoiding a rollback-on-partial-
  failure design.
- **Nice payoff from `001-start-match`'s forward-looking design**: no new EF Core migration
  needed — `HomeTeamScore`/`AwayTeamScore` columns and `Team.Score`'s `internal set`
  accessor already exist from `001-start-match`'s persistence retrofit, and
  `IMatchRepository.Update` (the method `/speckit-analyze` flagged as possibly premature
  during `001-start-match`, then retracted after checking the brief PDF) is exactly what
  this feature now consumes.
- Constitution Check: all 5 principles PASS, no violations, Complexity Tracking N/A.
  Project Structure: only new folder is `src/WorldCupScoreboard/Exceptions/` — first
  feature whose rejection behavior needs it, matching CLAUDE.md's target repo layout.
  `demo/ScoreboardCli/Program.cs` plan-listed for a new `update` command (Principle V).
- Next suggested step: `/speckit-tasks` for `002-update-score`.

## 2026-08-03 — Ran `/speckit-tasks` for 002-update-score

- No `.specify/extensions.yml`, so no hooks. Generated `specs/002-update-score/tasks.md` — 11
  tasks (T001-T011) across Setup (N/A, nothing new needed), Foundational (T001-T002: the two
  exception types), User Story 1 (T003-T008: tests + `UpdateScore` implementation — the
  feature's only user story), and Polish (T009-T011: CLI demo `update` command, `dotnet
  format`, manual quickstart walkthrough).
- **Refinement over plan.md**: plan.md listed a single test file
  (`UpdateScoreTests.cs`); tasks.md splits it into three
  (`UpdateScoreTests.cs`/`UpdateScoreValidationTests.cs`/`UpdateScoreNotFoundTests.cs`),
  mirroring `001-start-match`'s own established multi-file-per-concern test pattern for
  better `[P]` parallelism — flagged explicitly in tasks.md as a task-generation-time
  refinement, not a contradiction of plan.md's design.
- T004 (validation tests) includes an explicit note that Acceptance Scenario 5
  (letters/special characters) has no runnable test here — `homeScore`/`awayScore` are `int`,
  so that's a compile-time error, consistent with quickstart.md's earlier documentation of
  the same limitation.
- Parallel opportunities: T001/T002 (exception types), T003/T004/T005 (three US1 test files),
  T010 alongside T009/T011.
- MVP scope: the entire feature (T001-T008) — `002-update-score` has only one user story, so
  there's no smaller independently-shippable slice within it; T009-T011 (Polish) round it out.
- Next suggested step: `/speckit-analyze`, then `/speckit-implement`.

## 2026-08-03 — Ran `/speckit-analyze` for 002-update-score

- Cross-checked `spec.md`/`plan.md`/`tasks.md`/`research.md`/`data-model.md`/
  `contracts/IScoreboard.md`/`quickstart.md`/constitution. 2 LOW/informational findings only
  (both already self-flagged or non-issues), 0 CRITICAL/HIGH: tasks.md's 3-file test split vs
  plan.md's single-file listing (already justified in tasks.md itself), and a false-positive
  "intuitive" match in research.md prose (design rationale, not a vague requirement).
  10/10 requirements (7 FR + 3 SC) mapped to tasks, 100% coverage. Recommended proceeding
  straight to `/speckit-implement`.

## 2026-08-03 — Ran `/speckit-implement` for 002-update-score (full TDD cycle, all 11 tasks)

- Checklist gate: 16/16 passing, no stop-and-ask. No `.specify/extensions.yml`, no hooks.
- **T001-T002**: added `src/WorldCupScoreboard/Exceptions/MatchNotFoundException.cs` (keyed on
  match ID, generic per research.md §2) and `InvalidScoreException.cs` (carries team name,
  attempted value, current value; message distinguishes "negative" from "lower than current").
- **T003-T005**: wrote `UpdateScoreTests.cs`, `UpdateScoreValidationTests.cs`,
  `UpdateScoreNotFoundTests.cs` against a not-yet-existing `UpdateScore` — confirmed they fail
  to *compile* (`CS1061: 'Scoreboard' does not contain a definition for 'UpdateScore'`) before
  writing any implementation, satisfying Principle I literally.
- **T006-T007**: added `Match UpdateScore(int matchId, int homeScore, int awayScore)` to
  `IScoreboard.cs`; implemented it in `Scoreboard.cs` under the existing lock — resolve+confirm
  in-progress (throw `MatchNotFoundException`), validate both scores non-negative and `>=`
  current before mutating either (throw `InvalidScoreException` otherwise, per research.md's
  validate-before-mutate atomicity decision), then mutate both `Team.Score` and call
  `repository.Update(match)`.
  - **T008**: 11/11 new tests pass; full suite 30/30 (19 from `001-start-match` + 11 new), no
  regression.
- **T009**: added an `update <matchId> <homeScore> <awayScore>` command to
  `demo/ScoreboardCli/Program.cs` — catches both new exception types and prints their message
  as a `REJECTED —` line; extended `PrintWelcome`/`PrintHelp` with the new command and 4 new
  manual-test scenarios (11-14), plus a note that malformed values can't reach the library
  through this CLI either (the CLI's own `int.TryParse` rejects them first).
- **Verified live against real SQLite**, not just unit tests: ran the CLI through start →
  update upward (succeeds) → attempt decrease (rejected, score confirmed unchanged via `get`)
  → attempt negative (rejected, unchanged) → update a nonexistent ID (rejected) — all four
  outcomes matched exactly, then deleted the scratch `scoreboard.db`.
- **T010-T011**: `dotnet format --verify-no-changes` clean; manual walkthrough done as part of
  the live CLI verification above.
- All 11 tasks in `tasks.md` marked `[X]`. Final state: `dotnet build`/`dotnet test` clean
  (30/30). Nothing committed yet — pending the user's explicit go-ahead, per standing
  instructions.

## 2026-08-04 — Ran `/speckit-specify` for 003-finish-match

- User input verbatim: "003-finish-match. A match's data if marked finished still exists in
  db. A finish match's status can't be changed to in-progress. One a match is marked finished
  then score can't be updated."
- Created `specs/003-finish-match/spec.md` (sequential numbering), updated
  `.specify/feature.json`. Single P1 user story (finish an in-progress match), 5 acceptance
  scenarios, 3 edge cases, 7 FRs, 4 measurable Success Criteria, zero
  `[NEEDS CLARIFICATION]` markers — every design choice had a strong precedent already set by
  001/002 (throwing convention, exception reuse, one-way transition per constitution
  Single-Concern Features).
- **Key insight surfaced in Assumptions** (not asked for, spotted while writing the spec):
  `Scoreboard.StartMatch`'s conflict checks and `Scoreboard.UpdateScore`'s in-progress check
  were both already written in 001/002 against `existing.Status != MatchStatus.InProgress`,
  anticipating a `Finished` value that didn't exist until now — FR-005 (reject score update on
  a finished match) and FR-006 (finished match's location/time becomes reusable) describe
  behavior those two features already implemented defensively. This feature's real new code
  surface is narrower than the FR count suggests: add `Finished` to `MatchStatus`, add
  `FinishMatch` itself — `StartMatch`/`UpdateScore` shouldn't need to change.
- Checklist (`specs/003-finish-match/checklists/requirements.md`): 16/16 items pass.
- Next suggested step: `/speckit-clarify` (likely to find nothing, given how precedented every
  choice already is) or straight to `/speckit-plan`.

## 2026-08-04 — Ran `/speckit-clarify` for 003-finish-match

- No hooks. Full ambiguity/coverage scan against `spec.md` — every category Clear, as
  predicted in the `/speckit-specify` completion report. **Zero questions asked.** Checklist
  unchanged, still 16/16. No spec edits made this pass.

## 2026-08-04 — Ran `/speckit-plan` for 003-finish-match

- No hooks. Before writing the plan, re-verified the "already-defensive code" claim from
  spec.md's Assumptions directly against current source (`MatchStatus.cs`, `Scoreboard.cs`,
  `IScoreboard.cs`) rather than trusting memory — confirmed `StartMatch`'s conflict loop and
  `UpdateScore`'s precondition both already check `Status != MatchStatus.InProgress` exactly as
  claimed.
- Generated `plan.md`, `research.md`, `data-model.md`, `contracts/IScoreboard.md`,
  `quickstart.md`. **Key decisions**: `FinishMatch(int matchId) : Match` — throws the *same*
  `MatchNotFoundException` from `002-update-score` (unchanged, not a new type) for both
  "doesn't exist" and "already finished," confirming that exception's generic design was the
  right call. No new EF Core migration — `Status` has been an `INTEGER` column since 001;
  adding a second enum member changes no schema. Explicitly decided *not* to touch
  `Scoreboard.StartMatch`/`UpdateScore` — their existing checks just need `Finished` to exist to
  activate. New dedicated `FinishMatchSideEffectsTests.cs` (not folded into 001/002's test
  files) to keep FR-to-test-file traceability clean for FR-005/FR-006's cross-feature effects.
- Constitution Check: all 5 principles PASS. Only new source change beyond the two new methods
  is `MatchStatus.cs` gaining `Finished`; no new exception, no new persistence type.
- Next suggested step: `/speckit-tasks` for `003-finish-match`.

## 2026-08-04 — Ran `/speckit-tasks` for 003-finish-match

- No hooks. Generated `specs/003-finish-match/tasks.md` — 10 tasks (T001-T010) across
  Foundational (T001: add `Finished` to `MatchStatus`), User Story 1 (T002-T007: three test
  files + `FinishMatch` implementation + verification — the feature's only story), and Polish
  (T008-T010: CLI demo `finish` command, `dotnet format`, manual quickstart walkthrough).
- Three US1 test files, mirroring 002's pattern: `FinishMatchTests.cs` (successful finish),
  `FinishMatchRejectionTests.cs` (already-finished/nonexistent), and
  `FinishMatchSideEffectsTests.cs` (the FR-005/FR-006 cross-feature effects — explicitly noted
  in T004 that these exercise already-existing `StartMatch`/`UpdateScore` checks, so no change
  to those methods is expected to make them pass).
- T010 (Polish) calls out a bonus: `001-start-match/quickstart.md`'s own step 4 ("a finished
  match frees its slot"), previously a documented forward reference, becomes exercisable too —
  worth confirming manually, but explicitly out of scope to edit that file from this feature.
- MVP scope: the entire feature (T001-T007) — single user story, no smaller independently-
  shippable slice within it.
- Next suggested step: `/speckit-analyze`, then `/speckit-implement`.

## 2026-08-04 — Ran `/speckit-analyze` for 003-finish-match

- No hooks. Cleanest analysis of the three features so far: 1 LOW/non-issue finding (FR-003
  "no reopen operation" has no task — correctly, it's satisfied by omission, not testable
  behavior). 0 CRITICAL/HIGH. plan.md and tasks.md fully agreed this time (unlike 002, no
  file-count deviation to flag). 10/11 requirements (FR-003 excepted, by design) mapped to
  tasks — effectively 100% coverage of testable requirements.

## 2026-08-04 — Ran `/speckit-converge` for 003-finish-match (before implementing)

- No hooks. Verified directly: no `Finished` in `MatchStatus.cs`, no `FinishMatch` anywhere,
  no `FinishMatch*Tests.cs` files — nothing implemented yet, all 10 tasks unchecked. Outcome:
  **Converged** — every gap already tracked by tasks.md's own unchecked tasks, nothing new to
  append. Sanity-checked no regression: 30/30 tests still passing (001+002 unaffected).

## 2026-08-04 — Ran `/speckit-implement` for 003-finish-match (full TDD cycle, all 10 tasks)

- Checklist gate: 16/16 passing. No hooks.
- **T001**: added `Finished` to `MatchStatus` enum (`InProgress, Finished`).
- **T002-T004**: wrote `FinishMatchTests.cs`, `FinishMatchRejectionTests.cs`,
  `FinishMatchSideEffectsTests.cs` against a not-yet-existing `FinishMatch` — confirmed
  `CS1061` compile failures before writing any implementation.
- **T005-T006**: added `Match FinishMatch(int matchId)` to `IScoreboard.cs`; implemented it in
  `Scoreboard.cs` — resolve+confirm in-progress (throw the *same* `MatchNotFoundException` from
  002, no new type), set `Status = Finished`, persist, return. Did **not** touch
  `StartMatch`/`UpdateScore` as planned — their pre-existing checks activated correctly with
  zero code changes.
  - **T007**: 7/7 new tests pass; full suite 37/37 (30 prior + 7 new), zero regressions.
- **T008**: added a `finish <matchId>` command to `demo/ScoreboardCli/Program.cs`; extended
  `PrintWelcome`/`PrintHelp` with 5 new manual scenarios (15-19), including one explicitly
  demonstrating the location/time-reuse payoff (a second match starting successfully at the
  same venue/time a finished match used, contrasted with scenario 5's rejection while that
  match was still in-progress).
- **Verified live against real SQLite**: start → update → finish → get (data retained,
  Status=Finished) → finish again (rejected) → finish nonexistent (rejected) → update
  post-finish (rejected, score still 2-1) → start a second match, finish it, then successfully
  reuse its exact location+time for a third match. All outcomes matched spec exactly.
- **T009-T010**: `dotnet format --verify-no-changes` clean; manual walkthrough completed as
  part of the live CLI verification above (including the `001-start-match/quickstart.md` step
  4 bonus scenario, without editing that file, per the task's own instruction).
- All 10 tasks marked `[X]`. Final state: `dotnet build`/`dotnet test` clean (37/37). Nothing
  committed yet — pending the user's explicit go-ahead. Brief's first three of four required
  core operations (start, update score, finish) are now all implemented.

## 2026-08-04 — Ran `/speckit-specify` for 004-live-summary (scoped to live summary only)

- User's raw ask bundled two things: (1) the live summary itself — order in-progress matches
  by total score descending, tie-break by most-recently-started, track total score in the data
  model, kept in sync on score updates, plus the brief's exact worked example as the expected
  order; (2) a separate paginated "see all matches saved in db" browse feature (10/page,
  most-recent created-or-updated first), which the user's own wording called out as "separate
  from live summary."
- **Deliberately scoped this spec to (1) only** — created `specs/004-live-summary/spec.md`
  covering just the live summary, and explicitly excluded pagination via a flagged Assumptions
  bullet rather than silently dropping it or silently bundling it in. Reasoning: bundling both
  into one spec would violate constitution Principle III (Single-Concern Features — "never
  bundle multiple operations or concerns into a single spec"), `/speckit-specify` itself only
  creates one feature per invocation, and pagination isn't a numbered slot on CLAUDE.md's
  current Roadmap (which only has 004-007 mapped) — inserting it is a Roadmap-structure
  decision the user should make explicitly, not something to decide unilaterally mid-spec.
- Verified the worked example's arithmetic before locking it in as FR-006/Acceptance Scenario
  1: totals are Mexico 5, Spain 12, Germany 4, Uruguay 12, Argentina 4 — sorted desc with
  most-recently-started-first on ties (Uruguay #4 before Spain #2 at 12; Argentina #5 before
  Germany #3 at 4) produces exactly Uruguay, Spain, Mexico, Argentina, Germany — matches the
  brief exactly.
- Single P1 user story, 6 acceptance scenarios, 3 edge cases, 6 FRs, 4 measurable Success
  Criteria, zero `[NEEDS CLARIFICATION]` markers. Checklist
  (`specs/004-live-summary/checklists/requirements.md`): 16/16 pass.
- **Still needs a decision from the user**: how to handle the pagination request — options
  include (a) a new `/speckit-specify` run for it as its own feature (renumbering
  `005-match-history`/`006-scoreboard-api`/`007-scoreboard-frontend` up by one, or giving
  pagination a number after them), or (b) folding it into `005-match-history` if the user
  decides "browse all matches" and "match history" are actually the same concern after all.
  Not yet resolved — flagged in the completion report for the user to weigh in on.

## 2026-08-04 — User decided: fold pagination into 005-match-history

- Asked the user via AskUserQuestion; chose "fold into 005-match-history" over a new
  standalone spec — `GetHistory` becomes the one operation returning all matches (in-progress
  + finished), paginated 10/page, most-recent created-or-updated first.
- **Caught a real reconciliation need while updating CLAUDE.md**: the existing `GetHistory`
  rationale already specified an ordering — "ordered by start order (most recent first)" — which
  is subtly different from the new pagination request's "most recent match created or updated
  first" (a score update or finish should also bump a match to the front, not just its original
  start time). Updated CLAUDE.md's Roadmap table row and the "Extra operation" Confirmed
  Decisions bullet to state the reconciled ordering explicitly (superseding the old "start
  order" framing) and to note pagination (10/page) as part of `GetHistory`'s contract now.
  Cross-referenced `004-live-summary`'s spec (Assumptions section already explains why
  pagination was scoped out of that feature).
- No spec.md changes made for `005-match-history` yet — it hasn't had its own `/speckit-specify`
  run. This is purely a CLAUDE.md update to make the Roadmap/Confirmed Decisions accurate ahead
  of that.

## 2026-08-04 — Ran `/speckit-clarify` for 004-live-summary

- No hooks. Full ambiguity/coverage scan — every category Clear. Zero questions asked, same
  pattern as 002/003. One editorial fix made directly (not a formal Q&A): tightened FR-004 to
  say total score is correct "from the moment the match is created (0 at 0-0)," not just "after
  any score update" — a completeness gap, not a design ambiguity. Checklist unchanged, 16/16.

## 2026-08-04 — Ran `/speckit-plan` for 004-live-summary

- No hooks. Generated `plan.md`, `research.md`, `data-model.md`, `contracts/IScoreboard.md`,
  `quickstart.md`. **Key decisions**: `Match.TotalScore` is a get-only, expression-bodied
  computed property (`HomeTeam.Score + AwayTeam.Score`), not a persisted column — correct by
  construction, satisfies FR-004 with zero changes to `002-update-score`'s `UpdateScore`.
  Explicitly verified (not just assumed) that EF Core's model-building convention excludes a
  property with no setter and no backing field from the mapped model, based directly on the
  real bug from `001-start-match`'s persistence retrofit (where the *opposite* case — get-only
  properties WITH a settable path — needed fixing) — so no `[NotMapped]` attribute or migration
  needed. `GetSummary`'s tie-break reuses `Match.Id` (the monotonic sequence `001` committed to
  specifically for this future ordering rule) rather than inventing a new field.
  `GetSummary() : IEnumerable<Match>` returns the existing entity, no new DTO type.
- Gave the brief's worked example its own dedicated test file
  (`GetSummaryWorkedExampleTests.cs`), separate from ordinary ordering tests, so it stays
  trivially discoverable as the literal acceptance test CLAUDE.md commits to.
- Constitution Check: all 5 principles PASS — Principle III's pass explicitly notes pagination
  was already excluded and folded into `005-match-history` per the user's own decision.
- Next suggested step: `/speckit-tasks` for `004-live-summary`.

## 2026-08-04 — Ran `/speckit-tasks` for 004-live-summary

- No hooks. Generated `specs/004-live-summary/tasks.md` — 11 tasks (T001-T011) across
  Foundational (T001: add `Match.TotalScore`), User Story 1 (T002-T008: four test files +
  `GetSummary` implementation + verification — the feature's only story), and Polish
  (T009-T011: CLI demo `summary` command, `dotnet format`, manual quickstart walkthrough).
- Four US1 test files matching plan.md exactly: `GetSummaryOrderingTests.cs`,
  `GetSummaryLiveUpdateTests.cs`, `GetSummaryScopeTests.cs`, and — kept deliberately separate
  — `GetSummaryWorkedExampleTests.cs` for the brief's literal acceptance test, so it stays
  trivially discoverable rather than buried among ordinary ordering tests.
- MVP scope: the entire feature (T001-T008) — single user story, no smaller independently-
  shippable slice. Framed T008's checkpoint as completing "the brief's fourth and final
  required core operation."
- Next suggested step: `/speckit-analyze`, then `/speckit-implement`.

## 2026-08-04 — Ran `/speckit-converge` for 004-live-summary (before implementing)

- No hooks. Verified directly: no `TotalScore`/`GetSummary` anywhere, no `GetSummary*Tests.cs`
  files, all 11 tasks unchecked. Outcome: **Converged** — every gap already tracked, nothing
  new to append. No regression: 37/37 tests still passing.

## 2026-08-04 — Ran `/speckit-analyze` for 004-live-summary

- No hooks. 1 LOW/informational finding: flagged that research.md's claim "EF Core excludes
  `TotalScore` from the mapped model" is reasoned by analogy, not yet empirically confirmed —
  given the project's own precedent of an EF Core surprise in `001-start-match` (the *opposite*
  case), called this out as a "watch for this during T001/T007's actual build" note rather than
  a spec/plan defect. 0 CRITICAL/HIGH. 10/10 requirements mapped to tasks, 100% coverage.

## 2026-08-04 — Ran `/speckit-implement` for 004-live-summary (full TDD cycle, all 11 tasks)

- Checklist gate: 16/16 passing. No hooks.
- **T001**: added `public int TotalScore => HomeTeam.Score + AwayTeam.Score;` to `Match.cs`.
  The `/speckit-analyze` LOW-finding's "watch for this" check passed clean — `dotnet build`
  raised no pending-model-changes warning, confirming EF Core excluded the computed property
  from the mapped model exactly as research.md predicted.
- **T002-T005**: wrote `GetSummaryOrderingTests.cs`, `GetSummaryLiveUpdateTests.cs`,
  `GetSummaryScopeTests.cs`, `GetSummaryWorkedExampleTests.cs` against a not-yet-existing
  `GetSummary` — confirmed `CS1061` compile failures before writing any implementation.
- **T006-T007**: added `IEnumerable<Match> GetSummary()` to `IScoreboard.cs`; implemented it in
  `Scoreboard.cs` — filter to `InProgress`, `OrderByDescending(TotalScore).ThenByDescending(Id)`,
  under the existing lock, never throws.
  - **T008 — real Principle II cycle, not just a clean pass**: first test run failed 1/10.
    Root cause (reproduced before touching code): my own
    `GetSummary_UpdateThatDoesNotChangeTotal_DoesNotDisturbTieOrder` test tried to go from 2-2
    to 3-1 expecting the same total (4) — but `002-update-score`'s `UpdateScore` enforces
    **per-team** non-decrease, so decreasing away-score 2→1 while increasing home 2→3 is
    rejected regardless of the total staying flat. The test's premise was flawed, not the
    production code: the only way to resubmit an unchanged total under that rule is a genuine
    no-op (identical scores). Fixed the test to call `UpdateScore(first.Id, 2, 2)` again
    instead, re-ran the FULL suite per Principle II step 4: 47/47 green.
- **T009**: added a `summary` command to `demo/ScoreboardCli/Program.cs`; extended
  `PrintWelcome`/`PrintHelp` with 2 new manual scenarios (20-21), including the brief's exact
  worked example reproduced via the CLI itself.
- **Verified live against real SQLite**: ran the CLI through the brief's exact worked example
  (5 matches started, scores updated) → `summary` produced **exactly** Uruguay, Spain, Mexico,
  Argentina, Germany, matching the brief character-for-character — then finished Germany and
  confirmed it disappeared from a second `summary` call.
- **T010-T011**: `dotnet format --verify-no-changes` clean; manual walkthrough completed as
  part of the live CLI verification above.
- All 11 tasks marked `[X]`. Final state: `dotnet build`/`dotnet test` clean (47/47). Nothing
  committed yet — pending the user's explicit go-ahead. **All 4 of the brief's required core
  operations (start, update score, finish, live summary) are now fully implemented** — only the
  chosen extra feature (`005-match-history`, now including pagination) remains from Phase 1.

## 2026-08-04 — Checkpoint: saved session state to memory

- User asked to log this session's work (already fully logged above through
  `004-live-summary`'s implementation) and save a checkpoint to memory. Updated
  `foot-ball-score-board-1.md` (cross-session memory) accordingly — see that memory for the
  condensed state; this file remains the full, detailed record.

## 2026-08-04 — Ran `/speckit-specify` for 005-match-history

- No slash-command feature description this time — built the spec directly from CLAUDE.md's
  already-reconciled Confirmed Decisions (from the `004-live-summary` pagination-scoping
  decision earlier this session): `GetHistory()` returns every match ever started (in-progress
  and finished), paginated 10/page, ordered by most recently created-or-updated first.
- Created `specs/005-match-history/spec.md`, updated `.specify/feature.json`. Single P1 user
  story, 6 acceptance scenarios, 4 edge cases, 7 FRs, 4 measurable Success Criteria, zero
  `[NEEDS CLARIFICATION]` markers.
- **Key new design concept**: a tracked "last activity" marker on `Match`, separate from `Id`
  (which only reflects creation order) — a monotonic sequence, not wall-clock, consistent with
  `001`'s `Id` rationale and `004`'s tie-break reuse of it.
- **Explicitly flagged a scope-expectation difference from the last two features**: unlike
  `003`/`004` (where `StartMatch`/`UpdateScore` needed zero or near-zero changes because their
  checks were already written defensively/correctly), this feature genuinely requires editing
  all three existing mutating methods (`StartMatch`, `UpdateScore`, `FinishMatch`) to add the
  new tracking write — called this out now so `/speckit-plan` doesn't assume the same
  "free lunch" pattern repeats a third time.
- Checklist (`specs/005-match-history/checklists/requirements.md`): 16/16 pass.
- Next suggested step: `/speckit-clarify` or `/speckit-plan`.

## 2026-08-04 — Ran `/speckit-clarify` for 005-match-history

- User passed context: "pagination is done with page size but also page can be changed." No
  hooks. Checked this specific point against spec.md: already correctly resolved — page
  **size** fixed at 10 (Assumptions) vs. page **number** variable/caller-supplied (FR-003),
  kept distinct throughout. Ran the full taxonomy scan anyway; every category Clear. Zero
  questions asked, checklist unchanged (16/16), no spec edits needed.

## 2026-08-04 — Ran `/speckit-plan` for 005-match-history

- No hooks. Verified directly against current source (`Scoreboard.cs`) that none of
  `StartMatch`/`UpdateScore`/`FinishMatch` track any activity/recency concept today — confirming
  spec.md's own scope-expectation flag rather than assuming it. Generated `plan.md`,
  `research.md`, `data-model.md`, `contracts/IScoreboard.md`, `quickstart.md`.
- **Key decisions**: new `Match.ActivitySequence` (`int`, monotonic, `internal set`) — unlike
  `004`'s computed `TotalScore`, this genuinely needs a **new EF Core migration** since it has
  no other field to derive from. Reused the "monotonic sequence, not `DateTime`" pattern a
  second time (after `004`'s tie-break already reused it once from `001`'s original `Id`
  rationale). New `InvalidPageException` — neither existing exception type
  (`MatchNotFoundException`, `InvalidScoreException`) fits a page-number validation concern, so
  split rather than force-fit, consistent with `002`'s original exception-splitting precedent.
  `GetHistory` paginates in-memory over `repository.GetAll()` (`.OrderByDescending` +
  `.Skip`/`.Take`), mirroring `004`'s `GetSummary` pattern rather than adding a new
  paged-query method to `IMatchRepository` — explicitly deferred as premature optimization
  given no stated performance goal.
- Constitution Check: all 5 principles PASS. Noted this is the brief's chosen "additional
  operation," still Phase 1 (not Phase 2/3), and must land in its own distinct commit.
- Next suggested step: `/speckit-tasks` for `005-match-history`.

## 2026-08-04 — Ran `/speckit-tasks` for 005-match-history

- No hooks. Generated `specs/005-match-history/tasks.md` — 17 tasks (T001-T017) across
  Foundational (T001-T004: `ActivitySequence` field, `InvalidPageException`, EF Core mapping,
  new migration — pure scaffolding, no behavior), User Story 1 (T005-T014: three test files +
  a strictly sequential chain of edits to `Scoreboard.cs` — `StartMatch`→`UpdateScore`→
  `FinishMatch`→`GetHistory` itself, since each builds on the last — + verification), and
  Polish (T015-T017: CLI `history <page>` command, `dotnet format`, manual walkthrough
  including a persistence-across-restart check).
- Explicitly noted in the Foundational phase header why, unlike `003`/`004`, there's no
  "already-defensive code" to activate here — the `StartMatch`/`UpdateScore`/`FinishMatch`
  edits are genuine behavior changes, so they follow Test-First in Phase 3 rather than being
  scaffolded in Phase 2.
- Flagged this feature's dependency chain as more sequential than prior ones: T009-T013 all
  touch `Scoreboard.cs` and must run in strict order (activity-sequence helper → three call
  sites → `GetHistory` itself), unlike `001`-`004` where most implementation tasks only had a
  single interface→implementation pair.
- T017 (Polish) explicitly calls for confirming `ActivitySequence` persistence survives a CLI
  restart — not just an in-process check, since this field (unlike `004`'s computed
  `TotalScore`) is genuinely persisted via a new migration.
- MVP scope: the entire feature (T001-T014) — single user story, no smaller independently-
  shippable slice. Completing it finishes the brief's chosen extra feature.
- Next suggested step: `/speckit-analyze`, then `/speckit-implement`.

## 2026-08-04 — Ran `/speckit-converge` for 005-match-history (before implementing)

- No hooks. Verified directly: no `ActivitySequence`/`GetHistory`/`InvalidPageException`
  anywhere, no migration, no `GetHistory*Tests.cs` files, all 17 tasks unchecked. Outcome:
  **Converged** — every gap already tracked, nothing new to append. No regression: 47/47
  tests still passing.

## 2026-08-04 — Ran `/speckit-analyze` for 005-match-history — found a real bug before it shipped

- No hooks. Found **I1 (HIGH)**: tasks.md T009 described seeding `_nextActivitySequence` via
  `repository.GetAll().Max(ActivitySequence) + 1` "mirroring the existing `_nextId` pattern" —
  but verified directly against actual `Scoreboard.cs` that the real `_nextId` pattern is a
  `foreach`/`>=` loop, NOT LINQ `Max()`. `Max()` throws `InvalidOperationException` on an empty
  sequence — a fresh `Scoreboard`/database (the very first run) would crash immediately. This
  is the first finding across all 5 features' analyses that's a genuine correctness bug rather
  than a stylistic note or already-self-flagged item. 0 CRITICAL, otherwise clean — 100%
  requirement coverage (11/11).
- User asked me to suggest the exact remediation before applying it. Presented the corrected
  task text (foreach/`>=`, matching `_nextId` exactly) and the concrete C# snippet, and asked
  whether to proceed with the fix or adjust first.

## 2026-08-04 — Saved a new memory for the bug class, then applied the I1 remediation

- User asked to save the problem + suggested solution to memory before continuing. Created
  `feedback-monotonic-counter-seeding.md` (type: feedback): whenever a monotonic counter is
  seeded from existing repository data in this project, use the `foreach`/`>=` pattern, never
  LINQ `Max()` (throws on empty sequences) — includes the actual `_nextId` reference code.
  Added to `MEMORY.md` index.
- User then explicitly confirmed the fix approach (foreach/`>=`, not `Max()`, not
  `OrderByDescending().FirstOrDefault()`) and asked me to update T009's tasks.md description
  to match before implementing. Edited T009 in `specs/005-match-history/tasks.md` to describe
  the corrected `foreach`/`>=` seeding exactly, cross-referencing the new memory and the
  `/speckit-analyze` finding by ID.

## 2026-08-04 — Ran `/speckit-implement` for 005-match-history (full TDD cycle, all 17 tasks)

- Checklist gate: 16/16 passing. No hooks.
- **T001-T004**: added `Match.ActivitySequence`, `InvalidPageException`, mapped
  `ActivitySequence` with `ValueGeneratedNever()` in `ScoreboardDbContext`, generated the
  `AddActivitySequence` EF Core migration (confirmed no autoincrement annotation, same pattern
  as `Id`).
- **T005-T007**: wrote `GetHistoryPaginationTests.cs`, `GetHistoryOrderingTests.cs`,
  `GetHistoryScopeTests.cs` against a not-yet-existing `GetHistory` — confirmed `CS1061`
  compile failures before writing any implementation.
- **T008-T013**: added `GetHistory` to `IScoreboard.cs`; in `Scoreboard.cs`, added
  `_nextActivitySequence` seeded via the corrected `foreach`/`>=` loop (the I1 fix, applied as
  agreed — sits right alongside the existing `_nextId` loop in the constructor), then bumped it
  in `StartMatch`, `UpdateScore`, and `FinishMatch` (one line each), then implemented
  `GetHistory` itself (order by `ActivitySequence` desc, `.Skip`/`.Take` for pagination, throw
  `InvalidPageException` for `page < 1`).
  - **T014**: 14/14 new tests pass on the first run — the I1 fix meant no empty-repository
    crash to reproduce/debug this time; full suite 61/61, zero regressions.
- **T015**: added a `history <page>` command to `demo/ScoreboardCli/Program.cs`; extended
  `PrintWelcome`/`PrintHelp` with 4 new manual scenarios (22-25).
- **Verified live against real SQLite, including the persistence-across-restart check T017
  specifically calls for**: ran the CLI, started/updated/finished matches, confirmed `history`
  ordering was correct; then **restarted the CLI process** and confirmed a newly-started match
  still got the correct next `Id` and ranked correctly at the top of history — proving
  `_nextActivitySequence`'s seeding (the exact logic I1 flagged) works correctly across a real
  process restart, not just within a single run.
- **T016-T017**: `dotnet format --verify-no-changes` clean; manual walkthrough (including the
  restart check) completed as part of the live CLI verification above.
- All 17 tasks marked `[X]`. Final state: `dotnet build`/`dotnet test` clean (61/61). Nothing
  committed yet — pending the user's explicit go-ahead. **The brief's chosen extra feature
  (`GetHistory`, with pagination) is now implemented — all Phase 1 (library) work from the
  brief is complete**: 4 required core operations + 1 chosen extra feature, all 5 features
  fully implemented and tested.

## 2026-08-04 — Ran `/speckit-specify` for 006-scoreboard-api (Phase 2, beyond the brief)

- User input verbatim: "006-scoreboard-api. Create a minimal .net web api project that uses
  that library and provide apis for all methods match creation, score update etc. If any
  method is missing then ask. Add unit tests for api as well and test while completing it.
  Add a docker file and swagger as well. It should be possible to use swagger to test the api.
  Return appropriate status code and response where applicable."
- **Explicitly checked the user's "if any method is missing then ask" instruction before
  writing anything**: read `IScoreboard.cs` directly — exactly 6 methods (`StartMatch`,
  `GetMatch`, `UpdateScore`, `FinishMatch`, `GetSummary`, `GetHistory`), all cleanly mappable
  to one endpoint each. Nothing is missing — stated this explicitly in spec.md's Assumptions
  rather than silently proceeding, since the user asked to be told either way.
- Created `specs/006-scoreboard-api/spec.md` — 5 user stories (mirroring `001`-`005`'s own
  priority order: start+get, update score, finish, live summary, history), 11 FRs, 4 Success
  Criteria, zero `[NEEDS CLARIFICATION]` markers.
- **Key design calls, all documented as Assumptions rather than left implicit**: "minimal .NET
  Web API" read as "small, single-purpose, thin transport adapter" (constitution Principle IV)
  rather than a mandate for the specific ASP.NET Core Minimal-API-endpoints style over
  Controllers — deferred the exact style to `/speckit-plan`. Status-code granularity is bounded
  by what the library itself distinguishes: typed exceptions
  (`MatchNotFoundException`/`InvalidScoreException`/`InvalidPageException`) map to specific
  statuses, but `StartMatch`'s single non-throwing `null` can only map to one generic
  client-error response — the API can't invent finer-grained reasons the library doesn't
  expose. No auth, no health-check endpoint — not requested. Tests are integration tests
  against real HTTP endpoints (constitution Principle IV), not a re-derivation of `001`-`005`'s
  own business-logic tests.
- **Scope-structure decision, flagged not assumed silently**: kept this as ONE Spec-Kit feature
  with 5 internal user stories, rather than splitting into 5 separate specs mirroring
  `001`-`005`'s per-operation granularity — reasoned from CLAUDE.md's Roadmap allocating a
  single entry to the whole API phase. Noted a finer split is possible if the user wants it.
- Checklist (`specs/006-scoreboard-api/checklists/requirements.md`): 16/16 pass, with a
  documented "technical-flavor" caveat (status codes/endpoints inherently appear in an API
  spec) that isn't itself a defect.
- Next suggested step: `/speckit-clarify` or `/speckit-plan`.

## 2026-08-04 — Ran `/speckit-clarify` for 006-scoreboard-api

- No hooks. Full ambiguity/coverage scan — every category Clear. Specifically checked for
  things that would materially change architecture (API versioning, exact HTTP verbs/status
  codes, DI lifetime, Swagger environment-gating) — all correctly deferred as plan-level
  decisions already, not spec-level gaps. Zero questions asked, checklist unchanged (16/16).

## 2026-08-04 — Ran `/speckit-plan` for 006-scoreboard-api

- No hooks. Generated `plan.md`, `research.md`, `data-model.md`, `contracts/api.md`,
  `quickstart.md`. **Key decisions**: new sibling project `src/WorldCupScoreboard.Api/`
  (ASP.NET Core Minimal API, not Controllers — six thin endpoints don't need MVC's ceremony).
  `Swashbuckle.AspNetCore` chosen over the built-in `Microsoft.AspNetCore.OpenApi` specifically
  because the built-in package only generates the OpenAPI document, not a bundled UI, and
  spec.md's FR-009 requires an interactive, browsable UI. `Scoreboard` registered as a DI
  **singleton** (not scoped) — reasoned explicitly that a per-request instance would actually
  break correctness (it would re-scan `repository.GetAll()` and reseed the monotonic counters
  on every request), not just waste time. New, separate SQLite file (`scoreboard-api.db`) for
  the API rather than sharing the CLI demo's `scoreboard.db` — avoids file-lock contention
  between two independently-run processes. Full HTTP status-code mapping table worked out
  (research.md §6) using ASP.NET Core's built-in `ProblemDetails` for all error bodies, not a
  bespoke shape. Test project references `WorldCupScoreboard.Tests` directly to reuse the
  existing `InMemoryMatchRepository` fake rather than duplicating it. Noted the
  `WebApplicationFactory<Program>` + top-level-statements gotcha (needs an explicit `public
  partial class Program { }` line) proactively, before it could become an implement-time
  surprise.
- Constitution Check: all 5 principles PASS — Principle V's row explicitly notes it's satisfied
  "by spirit, not the literal CLI": Swagger UI is this phase's manual-verification surface,
  since Principle V's text specifically names `demo/ScoreboardCli`, a Phase 1 concept.
- Next suggested step: `/speckit-tasks` for `006-scoreboard-api`.

## 2026-08-04 — Ran `/speckit-tasks` for 006-scoreboard-api

- No hooks. Generated `specs/006-scoreboard-api/tasks.md` — 32 tasks (T001-T032) across Setup
  (T001-T006: two new projects, Swashbuckle/Mvc.Testing packages, Dockerfile/.dockerignore),
  Foundational (T007-T011: request DTOs, endpoint-less `Program.cs` with DI/Swagger/migration
  wired up, the `ScoreboardApiFactory` test helper, `appsettings.json`), five user-story phases
  (T012-T028, one per endpoint group, mirroring `001`-`005`'s priority order), and Polish
  (T029-T032: Swagger manual check, Docker build+run verification, format, full quickstart
  walkthrough).
- **Flagged a real structural difference from every prior feature**: unlike `001`-`005` where
  most user stories' implementation tasks were independently parallelizable, all five of this
  feature's endpoint-mapping tasks (T014-T015, T018, T021, T024, T027) edit the **same**
  `Program.cs` file — so while all five stories' *tests* can be written in parallel (T012,
  T013, T017, T020, T023, T026, once the Foundational test factory exists), their
  *implementations* are strictly sequential P1→P2→P3→P4→P5. Called this out explicitly in
  Dependencies & Execution Order rather than leaving it as an implicit same-file constraint the
  way prior features' occasional 2-3-task sequential chains were.
  T032 explicitly walks quickstart.md's steps against BOTH the local `dotnet run` instance and
  the Dockerized one.
- MVP scope: Setup + Foundational + User Story 1 (T001-T016) — starting/retrieving a match over
  HTTP, mirroring `001-start-match`'s own MVP framing.
- Next suggested step: `/speckit-analyze`, then `/speckit-implement`.

## 2026-08-04 — Ran `/speckit-converge` for 006-scoreboard-api (before implementing)

- No hooks. Verified directly: no `WorldCupScoreboard.Api`/`.Api.Tests` projects, no
  `Dockerfile`, all 32 tasks unchecked. Outcome: **Converged** — every gap already tracked,
  nothing new to append. No regression: 61/61 library tests still passing.

## 2026-08-04 — Ran `/speckit-analyze` for 006-scoreboard-api — found a real inconsistency

- No hooks. Looked harder than usual given this feature's size (32 tasks, two new projects,
  Docker). Found **I1 (MEDIUM)**: tasks.md T015 offered `` `TypedResults.NotFound()`/`Problem(...)` ``
  as interchangeable — but the parameterless `NotFound()` overload returns an empty body,
  directly violating FR-008 ("never a bare status code with no body") and contradicting
  research.md §6/contracts/api.md's own committed `ProblemDetails`-always decision. Also
  **E1 (LOW-MEDIUM)**: quickstart.md's literal `docker run` command creates a fresh container
  each time (no volume, no `--name`+`start` reuse), so the SQLite file doesn't persist across
  separate Docker invocations, unlike the local `dotnet run` path — not a spec contradiction,
  just an undocumented asymmetry worth one clarifying sentence. 0 CRITICAL, 100% requirement
  coverage (15/15). Offered to apply the I1 fix and asked whether to also add the E1 note.

## 2026-08-04 — User redefined the error-response contract: error_code/error_message + OneOf

- User instruction (not a formal clarify/analyze remediation, a new design requirement): every
  4xx response must carry `error_code` (e.g. `"match_not_found"`) and `error_message`
  properties, implemented using the `OneOf` package and discriminated unions — superseding the
  earlier `ProblemDetails` decision entirely (which also incidentally subsumes fixing I1 from
  the prior analyze pass, since the whole ambiguous-NotFound()-vs-Problem() question goes away
  once every rejection path routes through one shared `IApiError`-to-`ErrorResponse` mapping).
- **Updated every artifact touched by this change**, not just tasks.md:
  - `spec.md`: revised FR-008 to specify the exact `error_code`/`error_message` shape and added
    a concrete error-code table (`start_rejected`, `match_not_found`, `invalid_score`,
    `invalid_page`); broadened SC-002; added two Assumptions bullets (concrete codes are
    library-distinguishability-bounded same as before; the OneOf/discriminated-union
    *implementation* pattern is explicitly deferred to plan.md as the technical "how", spec.md
    only fixes the observable "what").
  - `research.md`: struck through the superseded `ProblemDetails` decision (§6) rather than
    silently deleting it — left visible as a record of what changed and why; added a new §8
    documenting the `OneOf`-per-endpoint design (which cases each of the 5 endpoints needs, a
    shared `IApiError` interface so the error→response mapping is written once) and explicit
    rationale for why `OneOf` beats `try`/`catch` here (compiler-enforced exhaustiveness).
  - `data-model.md`: added `ErrorResponse` (with `[JsonPropertyName]` snake_case mapping) and
    the `IApiError`/4-error-type table.
  - `contracts/api.md`: every endpoint's error case now cites its specific `error_code`; added
    an "Implementation shape" section describing the `OneOf` pattern.
  - `plan.md`: added `OneOf` to Primary Dependencies with rationale; added the 6 new
    `Contracts/` files to the Project Structure; updated the Post-Phase-1 re-check note.
  - `tasks.md`: fully regenerated (noted as a "Revision note" at the top, explaining why and
    that it also resolves I1) — added T003 (OneOf package) to Setup; added T010-T016 to
    Foundational (the `ErrorResponse` DTO, `IApiError` interface, 4 error types, and the shared
    mapping helper — explicitly the piece that makes the mapping "written once, not duplicated
    per endpoint"); rewrote every user story's implementation task to build the correct
    `OneOf<Match, ...>` shape for that endpoint's specific error cases; rewrote every test
    task's description to assert the correct `error_code` value. Renumbered sequentially
    throughout (T001-T040) since nothing had been implemented yet — no churn cost.
  - `quickstart.md`: every `curl` example's expected-response line now shows the literal
    `error_code`/`error_message` JSON body instead of a bare status code.
- Did not touch the checklist (`checklists/requirements.md`) — quick sanity check confirmed
  nothing flipped state (still 0 `[NEEDS CLARIFICATION]`, still fully testable).
- Next suggested step: `/speckit-implement` for `006-scoreboard-api`.

## 2026-08-04 — Re-ran `/speckit-plan` for 006-scoreboard-api — confirmed already up to date

- User re-invoked `/speckit-plan` after the error_code/OneOf revision. `setup-plan.sh`
  confirmed `plan.md` already exists (non-destructive — the script skips the template copy in
  that case). Rather than blindly regenerating and risking loss of the careful rationale
  written in the previous turn's manual revision, re-verified `plan.md` against `spec.md`
  line-by-line: FR-008's error-code table, the `OneOf`/`ErrorResponse` design, the Constitution
  Check, and the Project Structure's new `Contracts/` files all already line up correctly — no
  drift found. No hooks. No files changed this turn.
- Reported this honestly rather than performing redundant work for its own sake.

## 2026-08-04 — Re-ran `/speckit-tasks` for 006-scoreboard-api — found and fixed one real gap

- User re-invoked `/speckit-tasks`. `tasks.md` already existed (40 tasks, T001-T040, already
  regenerated for the error_code/OneOf design in an earlier turn). Verified it against
  plan.md/spec.md rather than blindly regenerating.
- **Found one real, if minor, inconsistency this time**: `plan.md`'s Project Structure tree
  listed 8 `Contracts/` files but was missing `ApiErrorExtensions.cs` — the shared
  `ToHttpResult` error-mapping helper that `tasks.md`'s T016 creates and that research.md §8
  explicitly describes. Fixed by adding it to plan.md's file tree with the same one-line
  rationale research.md already gives (the mapping is written once, not duplicated per
  endpoint).
- Otherwise `tasks.md` itself needed no changes: checklist format correct throughout
  (checkbox/ID/`[P]`/`[Story]`), all 8 new Contracts/ DTOs plus the shared helper (T016)
  present, every endpoint task already using the `OneOf`/`ToHttpResult` pattern, task count
  and phase structure consistent with spec.md's 5 user stories.
- No hooks. Reported the one real fix rather than claiming a full clean pass or performing
  redundant full regeneration.

## 2026-08-04 — Re-ran `/speckit-converge` and `/speckit-analyze` for 006-scoreboard-api

- User re-ran both gates a second time after the error_code/OneOf revision and the
  `plan.md` Contracts/ file-list fix above, as the DoD requires clean converge+analyze before
  `/speckit-implement`. Both passed cleanly against the revised artifacts, clearing the way to
  start implementation.

## 2026-08-04 — Ran `/speckit-implement` for 006-scoreboard-api (full TDD cycle, all 40 tasks)

Session picked up with Setup (T001-T007) and Foundational (T008-T019) already built in a prior
context window — verified, not re-done: `dotnet build` succeeded, and a live smoke test
(`dotnet run` + `curl` against `/swagger/index.html` and `/swagger/v1/swagger.json`, both
`200`) confirmed the Foundational checkpoint's own definition ("API builds and starts with zero
endpoints, Swagger UI reachable, shared error helper exists, test factory works").

Then implemented all 5 user stories strictly in priority order, each following the same
TDD loop — write the endpoint's test file first, run it and confirm it fails (404/empty body,
since the route doesn't exist), implement the endpoint in `Program.cs`, re-run to confirm
green, then run the full two-project test suite to confirm no regression:

- **US1** (T020-T024): `StartMatchEndpointTests.cs` (201 body, 400 `start_rejected`) and
  `GetMatchEndpointTests.cs` (200 body, 404 `match_not_found`) written and confirmed failing
  (4/4 red — two as body-shape errors since no route existed to even 404 sanely, two as literal
  wrong-status-code assertions). Implemented `POST /matches` / `GET /matches/{id}` using
  `OneOf<Match, StartRejectedError>` / `OneOf<Match, MatchNotFoundError>`. All 65 tests green
  (61 library + 4 new).
- **US2** (T025-T027): `UpdateScoreEndpointTests.cs` (200 valid update, 404 unknown match, 400
  `invalid_score` for a negative score) — 3/3 red, then implemented `PUT
  /matches/{id}/score` catching `MatchNotFoundException`/`InvalidScoreException` from the
  library and mapping each to its `OneOf` case. 68/68 green.
- **US3** (T028-T030): `FinishMatchEndpointTests.cs` (200 finish, 404 for an
  already-finished match, 404 for an unknown match) — 3/3 red, then implemented `POST
  /matches/{id}/finish`. 71/71 green.
- **US4** (T031-T033): `GetSummaryEndpointTests.cs` — one test asserting `200` + empty array
  with genuinely no matches started (used a fresh `ScoreboardApiFactory` per test rather than
  the shared class fixture, since this endpoint's "empty" case needed real isolation), and one
  replaying the brief's exact worked example (Mexico/Canada → Spain/Brazil → Germany/France →
  Uruguay/Italy → Argentina/Australia) over real HTTP and asserting the exact expected order —
  2/2 red, then implemented `GET /matches/summary` as a plain `Results.Ok(...)`, no `OneOf`
  needed (this endpoint has no failure case). 73/73 green, confirming the brief's ordering
  guarantee survives the transport layer, not just the library.
- **US5** (T034-T036): `GetHistoryEndpointTests.cs` (12 matches started, page 1 returns 10,
  page 2 returns 2; an out-of-range page returns `200` + empty array, not an error; page 0
  returns `400` `invalid_page`) — 3/3 red, then implemented `GET /matches/history` catching
  `InvalidPageException`. **76/76 tests green** — the first fully-green run across every
  endpoint, as T036 itself calls for.

**Polish (T037-T040)**:
- T037: live Swagger check — parsed `/swagger/v1/swagger.json` and confirmed all 6 endpoints
  registered; sampled a live 404 and 400 response and confirmed both show
  `error_code`/`error_message` correctly.
- T038: Docker Desktop wasn't running on this machine. Asked the user how to proceed rather
  than silently skipping or guessing (options: start Docker and wait, or defer T038 and
  continue polish) — user chose to start Docker. `docker build` succeeded; a container run
  confirmed it migrates its own SQLite DB on first start (a `StartMatch` call succeeded) and
  Swagger is reachable on the mapped port; container removed after.
- T039: `dotnet format` reported no changes needed for either new project — already clean.
- T040: walked all 7 `quickstart.md` steps against the local `dotnet run` instance first — all
  matched exactly. Then repeated against the Docker container and got what looked like
  **stale data from the local run** appearing in "the container's" responses — investigated
  with `lsof -i :8080` and `ps aux` before trusting the result, and found the real cause: an
  earlier `dotnet run &` job's `kill %1` had only killed the wrapper process, leaving its child
  apphost orphaned and still bound to the port, so the "Docker" requests were actually being
  served by leftover host processes on ports 5299 and 8080, not the container. Killed the
  actual apphost PIDs (not just the job), confirmed both ports clear via `lsof`, and re-ran the
  full Docker walkthrough cleanly — all 7 steps matched the documented expected responses, with
  `lsof` confirming port 8080 was genuinely bound to `com.docker` this time. Saved this as a new
  feedback memory ([[feedback-dotnet-run-leftover-process]]) since it's a generalizable gotcha,
  not project-specific.

All 40 tasks in `specs/006-scoreboard-api/tasks.md` marked `[X]` (including T001-T019, which
had been implemented in the prior context window but never marked). Final `dotnet
build`/`dotnet test`: 76/76 passing, zero failures. Confirmed no stray Docker containers or
orphaned host processes left running, and no uncommitted SQLite files. Reported one
implementation-time deviation not previously surfaced to the user: `Program.cs` constructs
`ScoreboardDbContext`/`SqliteMatchRepository`/`Scoreboard` directly at startup rather than via a
DI-scope factory, avoiding a disposed-DbContext bug the literal task wording would have
produced. Nothing committed — reiterated the standing "don't commit without being asked"
instruction and asked whether/how to split the commit(s) for 006.

Separately, after this implementation pass, `Program.cs` was modified outside this session
(by the user or a linter) to add a default CORS policy (`AllowAnyOrigin().AllowAnyHeader()
.AllowAnyMethod()`, applied via `app.UseCors()`) — not part of any `006` task, observed as a
subsequent change to the file.

## 2026-08-04 — Checkpoint: saved session state to memory

- Updated [[foot-ball-score-board-1]] (the main execution checkpoint) to reflect `006`'s full
  completion: 76/76 tests, all 40 tasks done, the OneOf/error-shape design, the proactively
  caught DI-lifetime bug, and Swagger/Docker verification — superseding the prior version's
  "Phase 1 only, next step is README/AI.md" framing.
- Saved a new memory, [[feedback-dotnet-run-leftover-process]], documenting the orphaned
  apphost gotcha from T040 above — generalizes beyond this project to any `dotnet run
  &`-then-`kill`-based verification workflow.
- Updated `MEMORY.md`'s index to point at both.
- Outstanding/next unchanged in substance: `README.md`/`AI.md` still not started (now needs to
  cover `006`'s API-layer decisions too, not just the library), nothing committed yet across
  001-006, `007-scoreboard-frontend` not started.

## 2026-08-04 — Ran `/speckit-specify` for 007-scoreboard-frontend — found pre-existing artifacts from a parallel session

- User invoked `/speckit-specify 007-scoreboard-frontend` with a fresh description (Angular
  frontend, Material white/blue theme, left side nav for Summary/History/Matches, country-vs-
  country cards with flags and a "VS" separator, dropdown-driven match start, live refresh on
  tab switch, Material error modals for backend rejections).
- Found `specs/007-scoreboard-frontend/{spec.md,plan.md,tasks.md}` **already existed**, written
  minutes earlier by what looks like a separate, parallel Claude Code session on this same
  repo — more detailed than the just-typed prompt (Playwright E2E suite as its own User Story
  5, explicit hex palette, explicit ports 5000/4200, and the existing spec's own "Input" field
  ends with "For anything missing, ask me" — text not present in this session's literal
  invocation).
- Asked the user how to reconcile rather than guessing (three options: keep existing as
  authoritative, overwrite from scratch with only the just-typed prompt, or merge). **User
  chose: keep existing, review only.**
- Reviewed the existing `spec.md` against the just-typed prompt line-by-line — it already
  covers every point in the new prompt (Angular+Material, white/blue palette, 3-tab side nav,
  country-vs-country cards with flag/name/score and a "VS" badge, flag dropdowns for
  match-start, auto-refresh on tab switch, Material error-modal handling of backend
  rejections). No contradictions found.
- Created `checklists/requirements.md` (didn't exist yet) and validated the existing spec
  against it rather than treating "keep existing" as skip-validation: passed on all items
  except one partial (`~`) — SC-002/SC-003 name specific tools (Playwright, Docker Compose)
  rather than staying fully technology-agnostic, left as-is since both tools were explicitly
  user-requested, not AI-invented. Flagged two other items in Notes without editing the spec
  (per "review only"): the Playwright user story isn't mentioned in this session's own prompt
  text (kept, not removed, since it came from the user's own fuller prompt in the other
  session); User Story 3's score-update/finish-match scenarios aren't backed by their own
  top-level FR (low-impact gap, acceptance scenarios are unambiguous on their own).
- Updated `.specify/feature.json` to point at `specs/007-scoreboard-frontend` so downstream
  commands (`/speckit-clarify`, `/speckit-plan`, etc.) resolve correctly — it had been left
  pointing at `006-scoreboard-api` from the prior feature.
- No files in `src/`/`tests/` touched this turn — spec-only. Did not implement anything.
- Next suggested step: `/speckit-clarify` (to resolve the two flagged gaps above, if desired)
  or straight to `/speckit-plan`/`/speckit-tasks` review, since `plan.md`/`tasks.md` already
  exist too and haven't yet been checked against this same "keep vs. reconcile" question.

## 2026-08-04 — Ran `/speckit-clarify` for 007-scoreboard-frontend — no real ambiguity, just confirmation + a documentation gap

- User input: "do the playwright work. Regarding User Story 3, I don't understand clarify and
  ask question if needed."
- Ran the prerequisites check (no hooks). Re-read the full spec looking for genuine
  ambiguities beyond the two items flagged in the prior `/speckit-specify` review pass.
  Concluded neither item actually needed a decision, so asked zero formal multiple-choice/
  short-answer questions — explained both directly instead, per "ask a question if needed"
  (not needed here):
  - **Playwright**: user's "do the playwright work" directly confirms User Story 5 / FR-009
    stays in scope — recorded as resolved, no spec content changed (it was already there).
  - **User Story 3 gap**: explained to the user that this wasn't an ambiguity requiring a
    decision — the acceptance scenarios (score update, finish match) were already clear; the
    only issue was a missing top-level FR bullet mirroring them (FR-001 through FR-009 covered
    every other user story but not US3). Added **FR-010** ("System MUST allow updating a
    match's home/away scores and marking a match as finished directly from the Matches tab's
    active-match view") to close the gap.
- Added a `## Clarifications` / `### Session 2026-08-04` section to `spec.md` recording both
  resolutions as Q/A pairs, even though neither was a formal multiple-choice question loop —
  kept for traceability consistent with how prior features' clarify sessions are recorded.
- Re-validated `checklists/requirements.md`: no checkbox changed state (still 15/16 passing,
  1 partial `~` on SC-002/SC-003's tool-specific wording, left as-is since both tools are
  user-mandated, not AI-invented) — the Playwright and US3 items were Notes, not failing
  checkboxes, so their resolution updates the Notes text, not the pass count. Marked both as
  RESOLVED in the Notes section.
- 0 formal questions asked (both items resolved directly per explicit user instruction/
  explanation, no ambiguity survived scrutiny). No other candidate ambiguities found worth
  raising — full-spec re-read didn't surface anything new since the prior review pass already
  covered content quality, requirement completeness, and feature readiness thoroughly.
- Next suggested step: `/speckit-plan` (or a review pass of the already-existing `plan.md` /
  `tasks.md`, same "keep vs. reconcile" question as `spec.md` got, since those haven't been
  checked against this session's two resolutions yet — FR-010 in particular may need a
  corresponding task).

## 2026-08-04 — Ran `/speckit-plan` for 007-scoreboard-frontend — filled the actual missing gates/artifacts

- `setup-plan.sh` reported `plan.md` already exists (non-destructive, skipped template copy —
  same behavior as `006`'s re-run). Unlike the `spec.md` situation, though, the existing
  `plan.md`/`tasks.md` pair used a shorter, non-standard structure that never had `research.md`,
  `data-model.md`, `contracts/`, or `quickstart.md` generated, and `plan.md` itself was missing
  the template's required Technical Context / Constitution Check / Project Structure /
  Complexity Tracking sections entirely. Since the user invoked `/speckit-plan` plainly this
  time (no "keep existing, review only" instruction like the `spec.md` reconciliation), treated
  this as "fill the actual gates the command requires," not "leave everything untouched":
  **preserved** the existing plan's Architecture Overview / Key Technical Decisions /
  Verification Plan content verbatim (it was accurate), and **added** what was missing.
- **`research.md`** (new): 8 decisions — Angular 18+ standalone components; Angular Material +
  the already-fixed white/blue palette; Karma/Jasmine for component tests (chosen over Jest to
  avoid extra build tooling, alongside the already-planned Playwright e2e suite, to satisfy
  Constitution Principle IV's "frontend via component/e2e tests"); backend integration over
  plain REST + the CORS policy already present in `Program.cs` (observed, not re-added); a
  single functional `HttpInterceptorFn` + shared `MatDialog` for error handling (mirrors `006`'s
  own "one shared mapping, not duplicated per call site" pattern); a static bundled
  `CountryOption[]` list with `flagcdn.com` flags instead of a live countries API; Nginx-served
  multi-stage Docker build + root `docker-compose.yml`; Playwright E2E scope confirmation.
- **`data-model.md`** (new): `MatchViewModel`/`CountryOption`/`ApiError` — frontend view models
  mirroring `006`'s already-fixed JSON contract exactly, not a new source of truth; one derived
  `isInProgress` field; an explicit state-transition diagram noting the frontend never
  optimistically updates before the API confirms success.
- **`contracts/frontend-api-usage.md`** (new): maps each Angular feature/component to the
  specific `006` endpoint it calls and which user story that serves, plus the single
  `ScoreboardService` interface every component goes through (no direct `HttpClient` calls per
  component) — referencing `006`'s existing contract rather than duplicating request/response
  shapes, per the plan template's own guidance.
- **`quickstart.md`** (new): full manual walkthrough (9 steps) covering every acceptance
  scenario in `spec.md`, both local (`ng serve` + `dotnet run`) and via `docker compose up`,
  plus the `npx playwright test` command.
- **`plan.md`** restructured: added Technical Context (Karma/Jasmine + Playwright under
  Testing; explicit Constraint that the frontend never touches the DB/library directly);
  Constitution Check table (all 5 principles assessed — Principle V explicitly N/A for this
  phase per the constitution's own text, all others PASS, no Complexity Tracking entries
  needed); Project Structure (Option 2: web app, `src/WorldCupScoreboard.Frontend/` sibling to
  the existing `006` API project, matching this repo's existing `src/` convention rather than a
  separate top-level `frontend/` dir). Left the pre-existing architecture sections untouched
  underneath.
- **Not done this turn**: `tasks.md` was NOT touched — it still reflects the pre-restructure
  plan (10 broad tasks, no Karma/Jasmine component-test tasks, no explicit FR-010 task, no
  reference to the new `contracts/frontend-api-usage.md`/`data-model.md`). Flagged to the user
  as needing a `/speckit-tasks` pass before `/speckit-implement`, rather than silently treating
  the stale `tasks.md` as still authoritative.
- No hooks. No `src/`/`tests/` files touched — planning artifacts only.

## 2026-08-04 — Ran `/speckit-clarify` for 007-scoreboard-frontend (round 2) — added success-confirmation modal requirement

- User input: for the success scenario of match creating, update and finish, there should be a
  modal/pop-up notifying the user it succeeded, based on the API's expected status code.
- This was a genuine new requirement, not currently in `spec.md` — one real UX decision buried
  in it: whether the success confirmation should be a blocking `MatDialog` (like the existing
  error path) or a lighter, auto-dismissing `MatSnackBar`/toast. These have materially
  different UX implications (interrupts the flow after every successful action vs. non-
  blocking), so asked it as a single multiple-choice question rather than guessing.
  - **Recommended** snackbar/toast (standard Material pattern: dialogs for things needing a
    decision, snackbars for status confirmations; keeps a "sleek" dashboard from feeling
    heavy-handed after every action).
  - **User chose: Modal dialog** — same `MatDialog` family as the error path, for visual
    consistency across both outcomes.
- Integrated the answer into `spec.md`:
  - Added a `Clarifications` bullet recording the Q/A.
  - Added **FR-011**: Material modal dialog confirming success on `201`/`200` for
    start/update/finish, using `MatDialog` (not a snackbar), keyed off the actual returned
    status code.
  - Updated User Story 2's Acceptance Scenario 1 (start match) and User Story 3's Acceptance
    Scenarios 1-2 (update score, finish match) to explicitly mention the success modal
    appearing on the relevant status code.
- Re-validated `checklists/requirements.md`: no checkbox changed state (this is new scope, not
  a fix to something previously failing) — updated Notes to mark this RESOLVED.
- 1 question asked and answered (this session); combined with the 0 from the prior `/speckit-
  clarify` round for `007`, still well under the 5-question cap.
- **Not done this turn, flagged for the next `/speckit-tasks`/`/speckit-plan` pass**: `plan.md`
  (research.md §5 in particular, which currently only designs the error-path
  `HttpInterceptorFn`/`MatDialog`) and `tasks.md` don't yet reflect FR-011's success-modal
  requirement — both will need a follow-up reconciliation pass, on top of the `tasks.md`
  regeneration already flagged as outstanding from the `/speckit-plan` re-run.

## 2026-08-04 — Re-ran `/speckit-plan` for 007-scoreboard-frontend — folded in FR-011 (success modal)

- `setup-plan.sh` confirmed `plan.md` already exists (non-destructive). Rather than a fresh
  Phase 0/1 pass, reconciled the FR-011 (success confirmation modal, added via the second
  `/speckit-clarify` round) into every artifact the first `/speckit-plan` pass had already
  produced, since it introduced a new design decision (research.md) and a new component
  (plan.md/contracts.md/quickstart.md) that none of them yet reflected.
- **`research.md`**: added **§5a** — success confirmation opens a dedicated
  `SuccessDialogComponent` via `MatDialog`, triggered from the three `ScoreboardService` call
  sites (`startMatch`/`updateScore`/`finishMatch`), not from the `HttpInterceptorFn` — the
  interceptor only sees non-2xx responses and has no natural hook for action-specific success
  messages ("Match started" vs "Score updated" vs "Match finished"), unlike the error path's
  one generic `error_message` string. Considered and rejected: extending the interceptor to
  handle 2xx too (breaks its "one generic mapping" simplicity); a single dialog component with
  a success/error variant flag (rejected for simpler isolated Karma/Jasmine testing).
- **`plan.md`**: updated the note header, Technical Context (Primary Dependencies/Testing/
  Scale-Scope now mention `SuccessDialogComponent`), Constitution Check's Principle I row,
  Project Structure tree (added `success-dialog/` component + `success-dialog.service.ts`),
  and Key Technical Decisions (new point 3a for the success dialog, mirroring point 3's error
  dialog).
- **`contracts/frontend-api-usage.md`**: start/update/finish rows now note "success modal:
  FR-011"; added an explicit scoping note that `getMatch`/`getSummary`/`getHistory` (read-only
  fetches) do NOT trigger a success modal — FR-011 only covers the three mutating actions the
  user explicitly named, not every successful API call (avoids a noisy toast/modal on every
  tab switch, which the user never asked for).
- **`quickstart.md`**: steps 2, 5, and 7 (start/update/finish) now explicitly call out the
  success confirmation modal appearing on `201`/`200` before the resulting state change.
- No hooks. No `src/`/`tests/` files touched — still planning artifacts only.
- `tasks.md` still not reconciled — now needs to account for both the original plan
  restructure AND FR-011 in one `/speckit-tasks` pass.

## 2026-08-04 — Ran `/speckit-tasks` for 007-scoreboard-frontend — full regeneration (51 tasks)

- Replaced the earlier 10-broad-task version (written in a parallel session before `plan.md`
  was brought into the standard structure) with a full regeneration against the now-current
  `plan.md`/`research.md`/`data-model.md`/`contracts/frontend-api-usage.md`, organized by all
  5 user stories in priority order, mirroring `006-scoreboard-api`'s own task-generation
  conventions (checkpoints, sequential same-file dependencies called out explicitly, [P]
  markers, test-first ordering).
- **Tests included** (Karma/Jasmine component tests + Playwright E2E), even though the
  template treats tests as optional — justified explicitly in the file's own header: mandatory
  here per Constitution Principle I (Test-First, NON-NEGOTIABLE) and Principle IV ("frontend
  via component/e2e tests").
- **Phase 1 Setup (T001-T004)**: Angular workspace, Material + theme, Playwright tooling
  install, lint config. Deliberately did NOT include the Dockerfile/docker-compose.yml here
  (moved to US5's own phase instead — Docker Compose is literally what US5's acceptance
  scenario is about, so it belongs in that story's implementation, not generic Setup).
- **Phase 2 Foundational (T005-T013)**: models (`MatchViewModel`/`CountryOption`/`ApiError`),
  `FlagService`, `ScoreboardService` (test-first via `HttpTestingController`), `app.config.ts`
  wiring, `SidenavComponent`/`AppComponent` shell with 3 route stubs. Deliberately did NOT put
  the error/success dialog components here even though they're "shared" — they're each owned
  by the story that introduces them (US2 for success, US4 for error) since neither is a
  blocking prerequisite for US1's own acceptance scenarios.
- **Phases 3-7 (T014-T048)**: one phase per user story, test-then-implementation per story.
  Notable sequencing decisions:
  - US2 (T025-T031) introduces `SuccessDialogComponent`/`SuccessDialogService` (FR-011) on its
    first use (start match); US3 (T032-T036) reuses the same service for update/finish rather
    than rebuilding it — mirrors research.md §5a's "one shared success-dialog mechanism, not
    duplicated per action" decision.
  - US4 (T037-T043) owns the error interceptor/dialog entirely, deferred from Foundational
    since no earlier story's acceptance scenarios depend on error-handling being wired.
  - US5 (T044-T048) has **no new test-writing tasks** — its own "independent test" *is*
    running the Playwright suite already written across T018/T027/T034/T039 against the
    Dockerized stack, called out explicitly in the phase so it isn't mistaken for a gap.
- **Phase 8 Polish (T049-T051)**: lint, full `quickstart.md` walkthrough, and an explicit
  backend-regression check (`dotnet test` still green) — since this feature's CORS/contract
  assumptions touch the existing `006` API, even though no `006` code changes are planned.
- **51 tasks total**: Setup 4, Foundational 9, US1 11, US2 7, US3 5, US4 7, US5 5, Polish 3.
  Verified: all IDs T001-T051 unique and sequential, every checklist line matches the required
  `- [ ] T### [P?] [Story?] Description` format (spot-checked via grep, zero mismatches), no
  duplicate task numbers.
- No hooks. Suggested MVP scope: Setup + Foundational + US1 (T001-T024) — matches `006`'s own
  "single user story = MVP" framing.

## 2026-08-04 — Ran `/speckit-converge` for 007-scoreboard-frontend — found real implementation, real bugs

- Discovered `src/WorldCupScoreboard.Frontend/` and a root `docker-compose.yml` already exist
  with substantial real implementation (Angular workspace, Material theme, all 3 feature
  components, error dialog/interceptor, one combined Playwright spec file) — built outside
  this session, never checked against `spec.md`/`plan.md`/`tasks.md` until now.
- Read through every component/service/model file and the Playwright spec before writing any
  finding — no guessing. Found 8 gaps, 2 of them CRITICAL:
  - **F1 (CRITICAL, contradicts)**: `match.model.ts`'s `Match` interface is flat
    (`homeTeam: string`, `homeScore: number`) but `006-scoreboard-api`'s real JSON response is
    nested (`homeTeam: {name, score}`) — confirmed against the actual response bodies captured
    during `006`'s own implementation session. Every component reading `match.homeTeam` as a
    string is reading the wrong shape.
  - **F2 (CRITICAL, contradicts)**: `match-row.component.ts`'s status badge does
    `(match.status || '').toLowerCase()` — but `status` is the numeric enum `0`/`1`, not a
    string. `(1).toLowerCase()` throws a runtime `TypeError`, so viewing History with any
    finished match crashes the page. Same root cause as F1 (wrong assumed shape).
  - **F3 (HIGH, missing)**: FR-011's success confirmation modal doesn't exist anywhere —
    `MatchesComponent`'s start/update/finish success callbacks show nothing.
  - **F4 (HIGH, missing)**: zero component/service tests exist beyond the CLI-scaffolded
    `app.component.spec.ts` — a direct violation of Constitution Principle I (Test-First,
    NON-NEGOTIABLE).
  - **F5 (MEDIUM, partial)**: the one Playwright spec file covers most happy/error paths but
    never asserts the success modal, only replays 2 of the brief's 5 matches (not the full
    worked example), and never asserts History's 10-per-page pagination.
  - **F6 (MEDIUM, unrequested)**: `FlagService` (planned in plan.md's Project Structure)
    doesn't exist — flag logic lives as a plain function in `country.model.ts` instead;
    functionally fine but plan.md now describes something that isn't there.
  - **F7 (MEDIUM, missing)**: no `.dockerignore` for the frontend build context.
  - **F8 (LOW, missing)**: no Angular ESLint configured, `ng lint` not runnable.
- Appended `## Phase 9: Convergence` to `tasks.md` (T052-T059, append-only per the command's
  contract — no existing task text touched). T052/T053 (the two CRITICAL model/status bugs)
  ordered first per the severity-ordering rule.
- Outcome: **tasks_appended**, not converged. Recommended `/speckit-analyze` next (to check
  spec/plan/tasks internal consistency) and then `/speckit-implement` to actually work through
  T052-T059 before this feature can be considered done — the two CRITICAL bugs in particular
  mean the app likely doesn't work correctly against the real API yet, despite looking
  complete at a glance.

## 2026-08-04 — Fixed the two CRITICAL convergence bugs (F1/F2) in 007-scoreboard-frontend

- **F1 fix**: `core/models/match.model.ts` — replaced the flat `homeTeam: string`/
  `homeScore: number` shape with a nested `Team { name, score }` + `MatchStatus` numeric enum
  (`InProgress = 0`, `Finished = 1`), matching `006-scoreboard-api`'s real JSON contract
  exactly. Updated every consumer: `match-row.component.ts` (template bindings + the
  `app-country-card` inputs), `matches.component.ts` (active-match template bindings,
  `scoreMap` population from `m.homeTeam.score`/`m.awayTeam.score`). `StartMatchRequest`/
  `UpdateScoreRequest` (outbound request DTOs) were correctly flat already — no change needed
  there, only the inbound `Match` response shape was wrong.
- **F2 fix**: `match-row.component.ts` — replaced
  `[ngClass]="(match.status || '').toLowerCase()"` / `match.status === 'Finished'` (string
  comparison against a numeric field — `(1).toLowerCase()` threw a runtime `TypeError` for any
  Finished match) with a proper `isFinished` getter comparing against `MatchStatus.Finished`.
- **Verification, not just a build check**: `npx tsc --noEmit` clean, `ng build --configuration
  production` clean (pre-existing bundle-size warnings only, unrelated). Then ran the fix
  against genuinely live services — found the API (`:5000`) and frontend dev server (`:4200`)
  already running (leftover from the parallel session that built this feature), confirmed via
  `lsof`/`ps` they were real and responsive rather than assuming. Started a real match via
  `curl`, confirmed the actual response body is nested (`"homeTeam":{"name":...,"score":...}`,
  `"status":0`/`1`) exactly as the fix now assumes. Finished it, confirmed it appears in
  `/matches/history` with `status: 1`. Loaded `http://localhost:4200/history` in a headless
  Playwright browser (ad hoc script, not part of the committed suite) with `pageerror`/console
  listeners attached: **zero JS errors**, the "FINISHED" badge rendered, and the match's real
  name rendered correctly (not `[object Object]`/`undefined`) — the exact crash condition F2
  described, now passing. Spot-checked Summary and Matches tabs too: zero errors, correctly
  excluding the now-finished match from the "in progress" views.
- Marked T052/T053 `[X]` in `tasks.md`. T054-T059 (success modal, missing tests, e2e gaps,
  `FlagService` drift, `.dockerignore`, lint config) remain open — not addressed in this pass,
  scoped explicitly to "fix the CRITICAL issues" per the user's request.

## 2026-08-04 — Fixed T054-T059 in 007-scoreboard-frontend, found and fixed a real bug along the way

- **T054 (FR-011 success modal)**: created `SuccessDialogComponent` + `SuccessDialogService`
  (mirrors `ErrorDialogComponent`'s structure/styling, green checkmark instead of red error
  icon), wired into `MatchesComponent`'s start/update/finish success handlers with
  action-specific messages ("Match started successfully.", "Score updated successfully.",
  "Match finished successfully."). Verified live via a headless Playwright script against the
  real running services: all three modals fire correctly, zero JS errors.
- **T057 (FlagService)**: chose to actually build it (not just document the function-based
  approach) — extracted `getCountryFlagUrl` out of `country.model.ts` into a proper
  `FlagService` (`getFlagUrl(countryName)`), `CountryCardComponent` now injects it via DI
  instead of importing a bare function. Keeps `country.model.ts` pure data.
- **T058**: added `.dockerignore` for the frontend build context.
- **T059**: ran `ng add @angular-eslint/schematics`. The auto-generated recommended preset
  (`prefer-inject`, `prefer-control-flow`, `no-inferrable-types`, etc.) would have demanded
  rewriting nearly every existing component's established style (constructor DI, `*ngIf`/
  `*ngFor`, explicit type annotations) just to satisfy a newly-added linter — turned those
  specific stylistic-preference rules off rather than doing a mass rewrite out of scope for
  "make `ng lint` runnable." Fixed the small number of genuine issues instead: an unused
  `PageEvent` import, two `label`-without-`for` accessibility violations in `MatchesComponent`.
  `ng lint` now passes cleanly.
- **T055 (missing tests)**: this also surfaced that `ng test` was **completely broken** before
  this fix — the CLI-scaffolded default `app.component.spec.ts` was stale (asserted an
  `<h1>Hello,...` that no longer exists) and any Material component test crashed with
  `NG05105` (missing animations provider). Fixed both, then wrote 12 new spec files covering
  every component/service left untested: `ScoreboardService`, `FlagService`,
  `ErrorDialogService`, `SuccessDialogService`, `errorInterceptor`, `CountryCardComponent`,
  `MatchRowComponent` (including a dedicated "renders a Finished match without throwing" test —
  directly guards against F2 regressing), `ErrorDialogComponent`, `SuccessDialogComponent`,
  `SidenavComponent`, `SummaryComponent`, `HistoryComponent`, `MatchesComponent`. **42/42
  passing**, `ng lint` clean throughout.
- **T056 (extend Playwright suite) — found a real, second bug while doing this**: extending the
  e2e suite to add success-modal assertions and replay the brief's full 5-match example
  surfaced that **`MatchesComponent`'s start form never varies `location`/`scheduledAt` across
  submissions** — both were computed once in `ngOnInit()` and never refreshed or reset, so
  starting a second match (with different teams but the same stale location+time) collided
  with `006`'s own "same location + same time" in-progress uniqueness rule and got silently
  rejected. Reproduced by elimination: ruled out DB pollution, stale dev-server HMR state (both
  the API and `ng serve` were fully restarted with a clean DB and it still reproduced), and
  isolated it down to network-response logging showing an actual `400 start_rejected` on the
  *second* start attempt. Root-caused it to the stale form defaults, not a test artifact.
  **Fix**: `location` is now required (was optional with a stale default) and blank by default;
  after every successful start, the form resets `location` to blank and `scheduledAt` to a
  freshly computed "now" — makes every subsequent match genuinely distinct by default. Updated
  the e2e helper to fill a team-derived location per start (`"{home} vs {away} Stadium"`) and
  the existing `matches.component.spec.ts` test to patch a location value too (since it's now
  required).
  - Also found and fixed a **second-order test-data bug** in my own new pagination test: its
    12-pair fixture reused team names still in-progress from the worked-example test (which
    never finishes its matches), and the 18-country bundled list isn't large enough for 12
    disjoint pairs anyway. Fixed by cycling a small pool of genuinely free countries, finishing
    each match immediately so its teams are available again next round — reaches 10+ history
    entries without exhausting distinct names or colliding with anything.
  - Also hit a whitespace-normalization mismatch in Playwright's `toHaveText` exact-string
    comparison (rendering-order artifact, not a real bug) — fixed by comparing normalized
    `.innerText()` strings directly instead.
  - **All 4 Playwright tests pass** on a genuinely fresh run (backend DB wiped, both dev
    servers fully restarted) — verified twice in a row for reproducibility, not just once.
- Final verification sweep: `tsc --noEmit` clean, `ng lint` clean, `ng test` 42/42, `ng build
  --configuration production` clean (pre-existing bundle-size warnings only), `npx playwright
  test` 4/4, and `dotnet test` (the `006`/library suite) still 76/76 — confirmed no backend
  regression from any of this frontend work.
- Marked T054-T059 `[X]` in `tasks.md` (T052/T053 were already marked in the prior turn).
- **Noted but not fixed** (out of scope for "T054-T059"): `src/WorldCupScoreboard.Api/
  scoreboard-api.db*` are tracked in git despite `.gitignore` listing `scoreboard.db*` (a
  different, root-level filename) — the actual app-generated file at
  `src/WorldCupScoreboard.Api/scoreboard-api.db` was apparently committed by the parallel
  session and isn't covered by the ignore pattern. Restored it to its committed state via
  `git checkout` after this session's dev-server churn rather than leaving it modified, but did
  not restructure `.gitignore` or untrack it — flagging for a future cleanup pass.
- Killed all dev-server processes started during this session's verification (API on :5000,
  `ng serve` on :4200) — nothing left running.

## 2026-08-04 — Ran `/speckit-analyze` for 007-scoreboard-frontend, then applied D1/D2/F1-F3 remediation

- Read spec.md/plan.md/tasks.md/constitution.md fresh (not from memory) and found 9 findings:
  2 CRITICAL (Constitution Principle I gaps in tasks.md's own text), 3 MEDIUM inconsistencies,
  3 coverage gaps (1 MEDIUM — a genuinely unimplemented edge case, 2 LOW — documentation only),
  1 LOW ambiguity. 100% FR/SC-to-task coverage otherwise; zero unmapped tasks.
- Also noted (outside `/speckit-analyze`'s formal scope, since it only checks the 3 markdown
  artifacts' text, not code-vs-checkbox state): `tasks.md` still shows T001-T051 as unchecked
  even though that code already exists and was verified working — flagged as a `/speckit-
  converge` follow-up, not a formal analyze finding.
- User approved applying the top remediation (D1, D2, F1-F3). Applied all five directly to the
  existing artifacts, editing task descriptions in place rather than renumbering — tasks.md
  already has extensive `Depends on T0XX` cross-references and T052-T059 are already marked
  `[X]` (and already cited by number in this very log), so inserting new IDs and shifting
  everything after them would have broken both. Chose in-place expansion instead:
  - **D1**: rewrote T008 into a combined test-then-implement task for `FlagService` (asserts
    known-country and fallback-URL behavior before implementing), closing the "production code
    without a preceding failing test" gap without touching any other task's numbering.
  - **D2**: `ErrorDialogService` was named in plan.md's Project Structure but had zero task
    anywhere in tasks.md. Expanded T037 to add a preceding test for it, and T040 to actually
    create it (noting explicitly that the interceptor calls the service, not `MatDialog`
    directly) — same pattern as D1, no renumbering.
  - **F1**: spec.md's Key Entities still described `MatchViewModel` with flat
    `HomeTeam/HomeScore` fields — a leftover from before the F1 model-mismatch bug (fixed
    earlier this session) was corrected. Updated it to describe the real nested
    HomeTeam/AwayTeam-with-Name-and-Score shape, matching data-model.md and the actual code.
  - **F2**: plan.md §3a said "`ScoreboardService.startMatch`/... each open a shared
    `SuccessDialogComponent`" — misattributed the dialog-opening call to the service itself.
    Reworded to attribute it correctly to `MatchesComponent`'s calls into the service. Traced
    the same ambiguous phrasing back to its source in research.md §5a ("Triggered from the
    three `ScoreboardService` call sites") and fixed it there too, since plan.md's wording had
    inherited the ambiguity from research.md rather than introducing it fresh.
  - **F3**: tasks.md's own Playwright tasks (T018/T027/T034/T039) each invented a different
    hypothetical file name (`summary-history.spec.ts`, `matches.spec.ts` ×2,
    `error-handling.spec.ts`) contradicting plan.md's stated single-file design AND the actual
    repo (one combined `e2e/scoreboard.spec.ts`). Updated all four tasks plus the "Within Each
    User Story" dependency note and the "Parallel Example" section to consistently reference
    the one real file.
- Verified no stale references remain (`grep` for the three invented filenames returns zero
  hits; all Playwright task mentions now point at `e2e/scoreboard.spec.ts`, 8 occurrences).
- No files in `src/`/`tests/` touched — this was a documentation-consistency pass only, no code
  changes (D1/D2's underlying test coverage already exists in the real codebase from the
  earlier T055 fix; this pass only brought tasks.md's own *instructions* into alignment with
  what already happened, and fixed spec.md/plan.md/research.md's remaining prose drift).

## 2026-08-04 — Ran `/speckit-converge` for 007-scoreboard-frontend, then reconciled all checkboxes

- Systematically re-verified all 51 pre-existing tasks (T001-T051) against the actual codebase
  — file-existence checks per task, plus re-confirming `ng test`/`ng lint`/`dotnet test` results
  already established earlier this session. Found **zero gaps** — every task's underlying file
  and behavior genuinely exists and works, including the `ErrorDialogService` added during the
  `/speckit-analyze` remediation (D2) and the `FlagService` test-first restructuring (D1).
- **Caught a real remaining gap before declaring victory**: T047/T048 specifically require
  verification against the *Dockerized* stack, and this session had only ever run the
  Playwright suite against local `dotnet run`/`ng serve` dev servers — never actually against
  `docker compose up`. Rather than assume equivalence, actually ran it: `docker compose down`
  (clean slate) → `docker compose up --build -d` (both images built fresh, both containers
  started) → confirmed `:5000/swagger` and `:5000/matches/summary` and `:4200/` all reachable
  → ran the full Playwright suite against that live Dockerized stack: **4/4 passed**. Then
  `docker compose down` to clean up.
- Outcome: **Converged — zero actionable findings.** No new tasks appended (correctly — the
  command's contract is to leave `tasks.md` untouched on a clean pass, not to flip checkboxes).
- **Explicitly clarified to the user, then acted on their actual intent**: `/speckit-converge`'s
  real contract has no mechanism to check off existing tasks even on a fully clean pass — it
  only appends new tasks for gaps. Since the user's request was specifically "reconcile the
  checkboxes" (not "run converge" for its own sake), and this session now has concrete,
  verified evidence for every one of T001-T051 (including the just-completed Docker run),
  directly marked all of them `[X]` as a separate, explicit action. `tasks.md` now has **all
  59 tasks (T001-T059) checked**, 0 remaining unchecked.
- No source code changes this turn — verification and bookkeeping only.

## 2026-08-04 — Addressed E1: implemented RxJS cancellation for rapid tab switching

- spec.md's Edge Case 3 ("rapid switching between History and Summary... RxJS state
  management/HTTP cancellation handles stale requests cleanly without race conditions") had
  been unimplemented since the feature was first built — `SummaryComponent`/`HistoryComponent`
  used plain `.subscribe()` calls with no cancellation, so a stale, out-of-order response could
  silently overwrite newer state (or a request could keep running uselessly after the user
  navigated away).
- **Fix**: in both components, replaced the direct `service.method().subscribe(...)` call with
  a `Subject` trigger piped through `switchMap` (cancels the previous in-flight request the
  moment a new one is triggered) and `takeUntilDestroyed(this.destroyRef)` (cancels anything
  still in flight when the component itself is destroyed, e.g. navigating to a different
  route). Public API (`loadSummary()`, `loadHistory(page)`, `goToPage(page)`, `matches`,
  `currentPage`, `loading`) is unchanged, so no template or caller changes were needed.
  `HistoryComponent`'s `currentPage` is only updated inside the inner pipe (after that specific
  page's response actually arrives), so a cancelled page's stale response can never
  incorrectly claim to be the current page either.
- **Test-first**: added one test per component using manually-controlled `Subject`s to
  simulate a real race (older request resolves *after* a newer one) — proves the newer
  response wins and the stale one is ignored, not just a smoke test. 44/44 Karma/Jasmine tests
  pass (42 previous + 2 new).
- **Verification beyond unit tests**: `tsc --noEmit`, `ng lint`, `ng build --configuration
  production` all clean; full Playwright suite (4/4) re-run against fresh local dev servers —
  no regression. Additionally wrote an ad hoc headless-browser script that rapidly clicks
  History↔Summary 15 times in a row against the real running app and asserted zero JS errors
  and a correctly-settled final state — directly exercises the actual user scenario the edge
  case describes, not just the isolated unit-test race simulation.
- Not tracked as a `tasks.md` checkbox (E1 was a `/speckit-analyze` finding, never appended as
  a task, unlike D1/D2/F1-F3 which were direct doc edits) — logged here instead for the record.

## 2026-08-04 — Addressed E2/E3/B1 (remaining /speckit-analyze findings)

Documentation-only edits — no code changes, since the underlying behavior for E2/E3 already
exists and works (verified in earlier passes); these just close the traceability/wording gaps
the analyze report flagged.

- **E2**: `tasks.md` T038 now explicitly names spec.md Edge Case 1 ("backend unreachable") and
  describes the expected behavior (a connection-failure message, not a raw/blank error, when
  `status: 0`) instead of leaving it implicit.
- **E3**: `tasks.md` T008 and T014 now explicitly cite spec.md Edge Case 2 (unknown/custom
  country → generic fallback flag icon, not a broken image).
- **B1**: `plan.md`'s Performance Goals replaced the vague "no perceptible extra client-side
  delay" with a concrete, measurable target — "rendered within 100ms of the response arriving
  (client-side rendering overhead only, excluding network/backend latency)".
- All 59 tasks remain `[X]` in `tasks.md`; this pass only added citations/wording, no new
  tasks and no checkbox-state changes.

This closes out every finding from the `/speckit-analyze` report (D1/D2/F1-F3 addressed
earlier, E1 addressed as its own implementation fix, E2/E3/B1 now closed) — 007's planning
artifacts are fully internally consistent as of this pass.

## 2026-08-04 — Re-ran `/speckit-plan` for 007-scoreboard-frontend — documented the E1 fix

- `setup-plan.sh` confirmed `plan.md` already exists (non-destructive). Rather than a fresh
  Phase 0/1 pass, checked whether the RxJS-cancellation fix (from addressing `/speckit-analyze`
  finding E1) had been documented anywhere in the planning artifacts — it hadn't (`grep` for
  `switchMap`/`takeUntilDestroyed`/`cancellation` across research.md/plan.md/data-model.md/
  contracts/ returned zero hits), even though it's now a real architectural decision in the
  codebase. Design-doc drift left over from fixing a bug directly rather than through a plan
  update.
- **`research.md`**: added §9 — the `Subject` → `switchMap` → `takeUntilDestroyed` decision,
  rationale (spec.md Edge Case 3, found unimplemented via `/speckit-analyze` finding E1), and
  alternatives considered (manual `Subscription` tracking, debouncing — both rejected).
- **`plan.md`**: added Key Technical Decisions point 6 (Request Cancellation) referencing
  research.md §9.
- **`quickstart.md`**: step 4 (Live refresh on tab switch) now also calls out the rapid
  History↔Summary click-through check and what it should never do (console error, stuck-on-
  stale-data), referencing research.md §9 — reuses the existing step rather than renumbering
  the rest of the walkthrough for one additional sentence.
- No `src/`/`tests/` changes — this pass only closed a documentation gap for already-shipped,
  already-tested code.

## 2026-08-04 — Re-ran `/speckit-tasks` for 007-scoreboard-frontend — synced with the plan update

- `setup-tasks.sh` confirmed `tasks.md` already exists. Since all 59 tasks were already `[X]`
  and the underlying work is done, this was a targeted sync (matching T021/T022's own
  established pattern from the `/speckit-analyze` D1/D2 fixes) rather than a full regeneration
  — updating task *descriptions* to reflect the `switchMap`/`takeUntilDestroyed` decision now
  documented in research.md §9/plan.md point 6, not adding new tasks (nothing is missing).
- **T016/T017** (Summary/History component tests): now explicitly mention the cancellation
  assertion each spec file actually contains (added during the E1 fix) — a stale request
  resolving after a newer one must not overwrite state.
- **T021/T022** (Summary/History implementation): now describe the actual `Subject` →
  `switchMap` → `takeUntilDestroyed` pipeline instead of the original "fetch-on-init"/
  "fetch-on-page-change" wording that predated the E1 fix and no longer matched the real
  implementation.
- No changes needed to T018 (Playwright worked-example test — unaffected by this fix), T024
  (already generically references T014-T017), or any other task.
- No renumbering, no new tasks, no checkbox-state changes — descriptions only, closing the
  same kind of plan-vs-tasks drift `/speckit-analyze` was designed to catch, pre-empted this
  time by going straight from a plan update to a tasks sync.

## 2026-08-04 — Ran a fresh `/speckit-analyze` pass, then applied F1 remediation

- Re-read spec.md/plan.md/tasks.md fully fresh (not from memory) after the plan/tasks sync.
  Found 2 new findings, both documentation-consistency only (no functional defects):
  - **F1 (MEDIUM)**: plan.md point 3 and research.md §5 both described the error interceptor
    as opening `ErrorDialogComponent` "via `MatDialog`" directly — the exact same misattribution
    pattern as the F2 finding fixed in the *previous* analyze pass (which corrected the
    success-dialog side), just never itself caught on the mirror-image error-dialog side.
    tasks.md's own T040 already had it right ("the interceptor calls the service, not
    `MatDialog` directly") — only the design-doc prose lagged behind.
  - **F2 (LOW)**: spec.md/data-model.md/tasks.md call the frontend models `MatchViewModel`/
    `TeamViewModel`, but the actual TypeScript interfaces are named `Match`/`Team` — pure
    terminology drift, no functional impact. Left open (not remediated this turn — only F1 was
    approved).
- **Applied F1**: reworded plan.md point 3 ("Calls `ErrorDialogService.openError(...)`, which
  opens `MatDialog`... the interceptor never calls `MatDialog` directly, mirroring how the
  success path (point 3a) also opens its dialog through a dedicated service") and research.md
  §5 (retitled from "`HttpInterceptorFn` + `MatDialog`" to "`HttpInterceptorFn` +
  `ErrorDialogService`", same correction in the decision text). Verified no other stray
  "opens/triggers ... via MatDialog" phrasing remained misattributed — the two remaining hits
  (plan.md point 3a, research.md §5a) are the success-dialog side, already correct from the
  prior pass.
- No code changes — documentation-only, closing the second half of a symmetric gap (error side)
  whose success-side twin was fixed earlier. `tasks.md` untouched (all 59 tasks remain `[X]`).

## 2026-08-04 — Applied F2 remediation: renamed doc references from MatchViewModel/TeamViewModel to Match/Team

- Chose the lower-effort, lower-risk option offered in the analyze report: updated the
  *documentation* to match the actual code's interface names, rather than renaming the
  TypeScript interfaces themselves (which would have touched many files for a purely cosmetic
  concern).
- **`data-model.md`**: heading `## MatchViewModel` → `## Match`, clarified it's the frontend
  TypeScript interface (in `core/models/match.model.ts`), `TeamViewModel` → `Team` in the
  field-type column.
- **`spec.md`**: Key Entities bullet renamed `MatchViewModel` → `Match`, references the nested
  `Team` object by name.
- **`tasks.md`**: T005 now says "Create the `Match`/`Team` interfaces" (was
  `MatchViewModel`/`TeamViewModel`); T014 now says "given a `Team` input" (was
  `TeamViewModel`).
- **`contracts/frontend-api-usage.md`**: the `ScoreboardService` interface code snippet's
  return types changed from `Observable<MatchViewModel>`/`MatchViewModel[]` to
  `Observable<Match>`/`Match[]`, matching the actual service signature.
- Verified zero remaining `MatchViewModel`/`TeamViewModel` references anywhere under
  `specs/007-scoreboard-frontend/`. No code changes; `tasks.md` still has all 59 tasks `[X]`.
- This closes both findings from the latest `/speckit-analyze` pass (F1 and F2) — `007`'s
  planning artifacts are now fully consistent with each other and with the actual codebase.

## 2026-08-04 — Ran `/speckit-implement` for 007-scoreboard-frontend — final completion validation

- Checklist status: `checklists/requirements.md` — 15/15 items complete, ✓ PASS. Proceeded
  automatically (no incomplete checklists to confirm past).
- Ignore files verified, none needed creating: frontend has its own `.gitignore`
  (Angular CLI default, covers `node_modules`/`dist`), `src/WorldCupScoreboard.Frontend/
  .dockerignore` and root `.dockerignore` both already cover the right patterns,
  `eslint.config.js` already has a real rules/ignores config from the T059 fix.
- All 59 tasks (T001-T059) in `tasks.md` already marked `[X]` — nothing left to implement;
  this run was a final validation pass, not new implementation work, given the extensive
  iterative convergence/analyze/remediation cycle already completed across this session.
- Ran the full verification sweep one more time from a clean state, since documentation had
  changed since the last full sweep (F1/F2 fixes) even though no code had: `tsc --noEmit`
  (app + spec configs) clean, `ng lint` clean, `ng test` 44/44, `dotnet test` 15/15 (API
  integration suite) — all green, no regression.
- **007-scoreboard-frontend is complete**: spec.md/plan.md/tasks.md are internally consistent
  (per the latest `/speckit-analyze`), the implementation converges with all three artifacts
  (per the latest `/speckit-converge`), and every automated check passes. Nothing has been
  committed yet — still pending explicit user go-ahead per standing instructions.

## 2026-08-04 — Updated cross-session memory to reflect 007's completion

- **`foot-ball-score-board-1`** (main checkpoint, in `~/.claude/projects/.../memory/`):
  rewritten to reflect all 3 Roadmap phases now done (001-007), not just 001-006. Recorded:
  the unusual parallel-session origin of `007`'s pre-existing code, the full reconciliation
  cycle (plan → clarify → plan → tasks → converge → analyze → remediate → converge →
  edge-case fix → plan → tasks sync → converge → analyze again → remediate), the 2 CRITICAL
  bugs `/speckit-converge` caught (flat-vs-nested `Match` model, string-vs-numeric status
  comparison), the `location`/`scheduledAt` uniqueness bug found via extending the Playwright
  suite, the final architecture (Angular 18 + Material, `switchMap`/`takeUntilDestroyed` for
  request cancellation), and updated test counts (76/76 backend, 44/44 + 4/4 frontend).
  Updated "Outstanding / next" to point at `README.md`/`AI.md` as the only remaining
  Roadmap-adjacent work, now that all 7 specs are done.
- **`feedback-dotnet-run-leftover-process`**: extended to explicitly cover `ng serve` (the
  same orphaned-child-process pattern as `dotnet run`, hit again this session while chasing
  down what turned out to be an unrelated form-defaults bug).
- **New memory: `feedback-speckit-docs-drift-after-direct-fix`**: captures the pattern that
  recurred three times on `007` alone — fixing something directly in code (bugs, edge cases,
  even doc-consistency fixes) reliably leaves `plan.md`/`research.md` silently out of date
  until a *later*, independent pipeline pass (re-reading fresh, not from memory) catches it.
  Includes the concrete lesson that a second `/speckit-analyze` pass on the *same* category of
  issue (dialog-opening attribution) still found a mirror-image instance the first pass missed.
- **`MEMORY.md`** index updated to reflect all of the above.

## 2026-08-04 — Wrote README.md

- Verified every factual claim against the actual code before writing, rather than from
  memory: re-read `IScoreboard.cs`/`Scoreboard.cs` in full, confirmed `dotnet test`'s exact
  current count (76 = 61 + 15), confirmed `Match.cs`/`Team.cs` constructors for the usage
  example's accuracy, confirmed the demo CLI's actual command set, confirmed
  `docker-compose.yml`/`Dockerfile` ports, confirmed `specs/006-scoreboard-api/contracts/
  api.md`'s exact error-code table, confirmed `GetSummaryWorkedExampleTests.cs` is the correct
  file to cite for the worked example.
- Structure: title/kata origin → **prominent, early** Java/Maven→.NET deviation section (own
  H2, not buried) → a 3-phase overview table → Phase 1 (what it does, usage example replaying
  the brief's exact worked example, Assumptions, Ordering rationale, Throwing-vs-non-throwing
  convention, the chosen extra feature `GetHistory` with explicit "why this one" rationale,
  Trade-offs) → build/test → CLI demo → Phase 2 (API: endpoints, `OneOf` error shape, Swagger,
  Docker) explicitly labelled "beyond the brief" → Phase 3 (frontend) same labelling → Docker
  Compose → repository layout.
- Caught and fixed one accuracy gap before finalizing: the usage example used
  `DbContextOptionsBuilder` without importing `Microsoft.EntityFrameworkCore`.
- Verified no unexplained TODOs/placeholders (`grep` clean), all referenced file paths exist
  (`GetSummaryWorkedExampleTests.cs`, `contracts/api.md`, the `.sln`), and all 6 code blocks in
  the file are properly paired (12 fence markers).
- Covers every item in CLAUDE.md's README.md acceptance checklist: basic usage, the deviation
  (prominent), all 6 assumption bullets, ordering rationale, all 3 named trade-off categories,
  the extra-feature rationale, build/test instructions referencing the worked-example test, and
  the CLI demo run command.

## 2026-08-04 — Expanded README.md: process, exercise brief, per-project run instructions, real test coverage

- Read the actual brief PDF (`~/Downloads/Coding Exercise version 2.01.pdf`) directly rather
  than paraphrasing from CLAUDE.md's own condensed summary, to quote/summarize it accurately in
  its own new section.
- **New "Process" section**: explains GitHub Spec-Kit was used for the full SDLC
  (`/speckit-constitution` → ... → `/speckit-implement`), what `.specify/memory/constitution.md`
  and `specs/*/` actually contain, and that `/speckit-analyze`/`/speckit-converge` were run
  repeatedly (not once) to catch drift — foreshadowing the Testing section's mention of the 2
  bugs `/speckit-converge` caught.
- **New "The coding exercise brief" section**: summarizes exactly what the PDF asked for (simple
  Java/Maven library, 4 required ops + 1 chosen op with justification + its own commit, the
  worked example, README/AI.md deliverables), then immediately explains the chosen extra
  feature (`GetHistory`) and why, right under that same heading — per explicit request, so the
  "what was asked" and "what was chosen and why" read together.
- Reinforced "library is the actual deliverable; API and frontend are additional" in the
  opening paragraph, not just implied by section ordering.
- **Real, measured test coverage, not estimated**: ran `dotnet test --collect:"XPlat Code
  Coverage"` and parsed the Cobertura XML per-class to separate business logic from EF Core's
  auto-generated `Migrations/`/persistence classes (deliberately unit-test-exempt per
  Constitution Principle IV) — **85.5%** line coverage on the library's actual business logic,
  **99.3%** on the API layer. Ran `ng test --code-coverage` for the frontend — **94.6%** line
  coverage. Documented *why* persistence classes read as uncovered (by design, verified at a
  different layer — CLI demo, API startup, Docker/Playwright — not silently skipped) rather
  than just quoting a lower blended number without context.
- **New "Testing" section**: a table of all 4 test folders/locations with file and test counts
  (verified via `ls`/`find`, not guessed), a "Methodology" subsection explicitly naming
  Constitution Principles I (Test-First/TDD) and II (Verify-Plan-Implement-Verify for bugs)
  with two concrete real examples of each, and the Coverage numbers above.
- **Run instructions restructured**: "Run each part separately" (library build/test, CLI demo,
  API standalone, frontend standalone with its dependency on the API noted, frontend tests
  standalone) now clearly separated from "Run everything together" (Docker Compose) — previously
  these were mixed into Phase-numbered sections without a clean separate/together distinction.
- Cleaned up the coverage-tool artifacts generated while gathering these numbers
  (`tests/*/TestResults/`, frontend `coverage/`) — none left in `git status`.
- Verified: no TODOs/placeholders, all referenced file paths exist
  (`GetSummaryWorkedExampleTests.cs`, `contracts/api.md`, `specs/005-match-history/spec.md`),
  all 9 code blocks properly paired, no duplicate headings.

## 2026-08-04 — Removed the dedicated Java/Maven→.NET deviation section from README.md

- Per explicit user instruction: removed the "## ⚠️ Deviation from the brief: .NET instead of
  Java/Maven" heading and everything under it. The opening paragraph now just states the
  library "is implemented in .NET 9 (C#) with xUnit" as a plain fact, no longer as its own
  called-out, justified deviation.
- **Flagged, not silently done**: this directly reverses one of CLAUDE.md's own standing
  "Confirmed decisions" — "Explicitly and prominently documents the Java/Maven → .NET
  deviation, with rationale (not buried in a footnote)" is listed there as a required
  acceptance-criteria item for README.md. Complied with the direct instruction since it was
  clear and immediate, but noted the tension back to the user rather than treating it as
  already-reconciled with CLAUDE.md.
- Fixed a dangling cross-reference this left behind: "The coding exercise brief" section's
  quoted summary said "(see the deviation above)" — updated to state the .NET substitution
  inline instead, since there's no longer a "deviation" section to point at.
- Verified no other stray mentions of "deviation"/"Java/Maven" remain, code block count
  unchanged (9, still paired), heading structure otherwise intact.

## 2026-08-04 — Expanded the Docker Compose section in README.md

- Per user request, expanded "Run everything together" into a fuller walkthrough rather than
  just the bare `docker compose up --build` command: what the two services in
  `docker-compose.yml` actually are (build contexts, ports, the frontend's `depends_on` the
  API), what to expect in the logs before it's ready, both services' URLs, a background-mode
  variant (`-d` + `docker compose ps`/`logs -f`), and how to stop it (`docker compose down`).
- Added an honest note about the API's SQLite database being ephemeral (no volume mounted in
  `docker-compose.yml`) — persists across `stop`/`start` but resets on `down` + fresh `up` or
  `--build` — and why that's fine (no durable-storage requirement, and it keeps every Compose
  run a clean baseline for the Playwright suite).
- Verified: 11 code blocks still properly paired, no dangling references, no TODOs/placeholders.

## 2026-08-04 — Fixed real UI formatting bugs: dialog headers and the Start Match button

- User reported the success/error modal's icon+text formatting was wrong, and the "Start
  Match" button too. Rather than guess, started the real app and took Playwright screenshots
  of both dialogs and the button to see the actual rendering.
- **Confirmed two real, distinct bugs**:
  1. **Dialog headers** (`SuccessDialogComponent`, `ErrorDialogComponent`): the icon appeared
     to float almost outside the dialog's rounded edge, with a large disconnected-looking gap
     before the title text. Root-caused via computed-style inspection: Angular Material's own
     `.mdc-dialog__title` class (applied automatically to any `h2[mat-dialog-title]`) bakes in
     `padding: 0 24px 9px` — designed for a title with no leading icon. That 24px left padding
     was stacking with our own flex `gap: 12px`, producing a ~36px visual gap instead of the
     intended 12px. Separately, the success dialog never had a `.custom-success-dialog-panel`
     surface-padding rule at all (unlike the error dialog's own `.custom-error-dialog-panel`
     rule) — a copy-paste gap from when the success dialog was added later.
     - **Fix**: reset `padding: 0` on both dialogs' title `h2` (with a comment explaining why,
       so it doesn't get "fixed" back by a future edit), and added the missing
       `.custom-success-dialog-panel` surface rule in `styles.scss`, unifying both dialogs'
       panel padding to `20px 24px` for proper breathing room from the rounded corner.
  2. **"Start Match" button**: the icon and text sat cramped together with almost no gap and
     slight vertical misalignment. Root cause: the button's `*ngIf="!startingMatch"` branch
     wrapped the icon+text in an extra `<span>` that was NOT itself a flex container (unlike
     Material's own button label span, which is), so the icon and a single literal space
     character were the only separation — breaking out of the flex/gap styling the rest of the
     button relies on. Confirmed via computed-style inspection that "Update Score"/"Finish
     Match" (which have no such extra wrapping span) were already correctly aligned — this bug
     was specific to Start Match's own markup, not a systemic pattern.
     - **Fix**: gave that span a `btn-label` class (`display: inline-flex; align-items: center;
       gap: 6px`) and split the icon and text into separate inline elements instead of a
       literal space.
  - Also did a broader visual pass over Summary/History/Matches (full-page screenshots) and
    checked the Refresh/Previous Page/Next Page buttons' markup — all already correctly
    formatted (direct children of the button, not wrapped in an extra non-flex span) — no
    further changes made there, staying scoped to what was actually reported broken.
- **Caught and fixed a self-inflicted syntax error while writing the fix**: an unescaped
  backtick inside a CSS comment (explaining the `gap` property) prematurely closed the
  surrounding JS template literal, breaking both dialog components. Caught immediately via
  `tsc --noEmit`/`ng lint` before it ever reached a browser.
- **Verified the actual fix visually**, not just "code looks right" — fresh Playwright
  screenshots of both dialogs and a zoomed-in crop of the button after the fix, confirmed
  clean alignment in all three.
- **Full regression sweep**: `tsc --noEmit` clean, `ng lint` clean, `ng test` 44/44, `ng build`
  clean, `dotnet test` 15/15, and the full Playwright suite (4/4) re-run against a genuinely
  fresh backend — nothing broken by the fix.
- Cleaned up all dev-server processes and restored the tracked SQLite db files to their
  committed state afterward.
