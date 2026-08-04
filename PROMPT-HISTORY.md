# Prompt History

Verbatim user prompts that drove this repository's implementation, in chronological order,
pulled from the session's full working log (`chat-history.md`, gitignored) and the raw
transcripts referenced there. This is the detailed backing material for `AI.md`'s "How AI was
used" summary — that document intentionally stays short; this file has the actual asks.

Entries are grouped by the spec or stage they produced. Where the original prompt is available
verbatim, it's quoted directly; where only a close paraphrase survived, that's noted.

## Bootstrapping CLAUDE.md and the Spec-Kit scaffold

**Fixing guessed Spec-Kit command names/paths to match what was actually installed:**

> Update CLAUDE.md to match the Spec-Kit setup that was actually installed (via `specify init
> --here --integration claude --script sh --force`), instead of the command names/paths
> originally guessed before setup ran:
>
> 1. In the "## Confirmed decisions" section, the "Process" bullet currently says the pipeline
>    is invoked as: /constitution → /specify → /clarify → /plan → /tasks → /implement, with the
>    constitution artifact at `memory/constitution.md`. Replace with the actual installed names:
>    /speckit-constitution → /speckit-specify → /speckit-clarify → /speckit-plan →
>    /speckit-tasks → /speckit-implement. These are installed as Claude Code skills under
>    `.claude/skills/speckit-*`, not native slash commands — note that. Also fix the
>    constitution artifact path to `.specify/memory/constitution.md` (not root-level `memory/`).
>
> 2. In the "## Chat history capture mechanism" section, update the checkpoint references
>    (currently /specify, /clarify, /plan, /tasks, /implement) to the same /speckit-* names.
>
> 3. Leave "## Repo layout (target state)" as-is — it already correctly shows `.specify/` for
>    the scaffolding.
>
> 4. Once /speckit-specify actually runs and creates a spec folder, verify its real path (e.g.
>    root-level `specs/<NNN-name>/` vs nested under `.specify/`) and correct CLAUDE.md if it
>    differs from what's written.
>
> Don't change anything else in CLAUDE.md — only the command-name and path references described
> above, to match the real installed tool. Log this correction as a new entry in
> chat-history.md.

**Decomposing Phase 1 into 5 specs, adding the Roadmap section:**

> Update CLAUDE.md to reflect a finer-grained Spec-Kit decomposition: instead of one "Phase 1:
> core library" spec, each of the brief's 5 operations becomes its own Spec-Kit feature, built
> and merged sequentially: specs/001-start-match, specs/002-update-score, specs/003-finish-match,
> specs/004-live-summary, specs/005-match-history (the chosen extra feature — must land in its
> own distinct commit per the brief), specs/006-scoreboard-api (Phase 2, beyond the brief),
> specs/007-scoreboard-frontend (Phase 3, beyond the brief).
>
> 1. In "## Roadmap", replace the single Phase-1 row with 5 sub-rows, keep Phase 2/3 as single
>    rows for now.
> 2. In "## Repo layout (target state)", update the specs/ line to show the multiple numbered
>    folders instead of one.
> 3. In "## Working conventions", add a line noting one Spec-Kit feature maps to one reviewable
>    commit, and that this satisfies the brief's "distinct commit for the extra feature"
>    requirement naturally via spec 005.
>
> Do not change anything else. Log this restructuring in chat-history.md.

(No "## Roadmap" section existed yet when this ran — flagged via AskUserQuestion; the user
chose to have it created from scratch rather than pause.)

## `/speckit-constitution` — filling the 5 principles

**Principles I-IV (verbatim):**

> ### Test-First (NON-NEGOTIABLE)
> TDD is mandatory for every operation: write a failing test that specifies the expected
> behavior first, confirm it fails for the right reason, then write the minimum code to make it
> pass (Red-Green-Refactor). No production code is written without a preceding failing test.
> Every one of the 4 core operations plus the chosen extra operation must have direct test
> coverage, including the brief's worked example as a literal acceptance test.
>
> ### Verify-Plan-Implement-Verify (NON-NEGOTIABLE)
> On any test failure or bug: (1) reproduce it and identify root cause before touching code —
> never guess-fix; (2) state the intended fix in one sentence before implementing it; (3)
> implement the minimal fix; (4) re-run the FULL test suite, not just the failing test, to
> confirm the fix and rule out regressions. A fix is not done until step 4 passes clean.
>
> ### Single-Concern Features
> Each Spec-Kit feature is scoped to one independently testable, independently shippable unit of
> behavior. Never bundle multiple operations or concerns into a single spec... This applies to
> every phase (library, API, frontend), not just the current 001-007 breakdown.
>
> ### Layered Architecture / Library-First
> All business logic and validation rules live ONLY in the scoreboard library (Phase 1). The API
> (Phase 2) is a thin transport adapter with no business logic of its own. The frontend (Phase 3)
> is a thin presentation layer that only calls the API... Each layer must be independently
> testable... Do not put specific tech choices (API framework, Angular vs React) in the
> constitution — those are decided per-phase in that phase's own plan.md.
>
> Do not remove or alter any placeholder sections beyond filling in these four principles. Log
> this in chat-history.md.

**Principle V, added later, verbatim:**

> Add a fifth principle to the project constitution:
>
> ### V. Runnable Local Verification (CLI Demo)
> Every implemented feature must be exercisable manually, not only through automated tests. A
> thin console demo project (demo/ScoreboardCli) wraps the current state of IScoreboard and is
> updated alongside each feature to demonstrate it... It has zero business logic of its own (per
> Principle IV) — it only calls the library and prints results. A feature's implementation is
> not considered done until this demo is updated to cover it, in the same commit as the feature.
>
> Do not touch the other 4 principles or any other section.

**Principle IV extended with the persistence abstraction, verbatim:**

> Amend constitution Principle IV (Layered Architecture / Library-First) — do not create a new
> principle, extend this one. Add: "Persistence is abstracted behind a repository interface
> (IMatchRepository); Scoreboard's business logic depends only on this abstraction, never on the
> concrete storage technology (Entity Framework Core / SQLite) directly. This keeps Principle I
> (Test-First) practical: unit tests exercise business logic against a fake/in-memory
> IMatchRepository." Update the Sync Impact Report accordingly. Do not touch Principles I, II,
> III, or V.
>
> [Also: update CLAUDE.md's Confirmed Decisions with the Persistence bullet, add the
> Persistence/ folder to the repo layout, note that unit tests use a fake IMatchRepository.]

This same prompt is what put the already-implemented `001-start-match` (built against a plain
`Dictionary<int, Match>`) out of compliance with the constitution it had just satisfied —
resolved by a follow-up plan/tasks retrofit (see below).

**Other direct CLAUDE.md edits, verbatim:**

> Update CLAUDE.md: 1. In "## Repo layout (target state)", add a new entry: demo/ScoreboardCli/
> (net9.0 console app...). 2. In "## Working conventions", add: "each feature's commit(s)
> include updating demo/ScoreboardCli to exercise the new operation..." 3. In the README.md
> acceptance criteria checklist, add a checkbox: "States how to run the CLI demo locally...".
> Log this in chat-history.md.

> Add a "Definition of Done" subsection to CLAUDE.md's "## Working conventions" section... A
> feature is not done, and should not be committed, until all of these pass in order: 1.
> /speckit-converge reports nothing left to build... 2. /speckit-analyze reports... internally
> consistent... 3. dotnet build and dotnet test both succeed with zero failures. 4. The feature
> can be run and observed manually via dotnet run --project demo/ScoreboardCli... 5. Every item
> in the feature's tasks.md is checked off. Only after all 5 pass does the feature get
> committed.

## `001-start-match`

**Opening description** (paraphrased from the session; not preserved verbatim): Match/Team
classes, score 0-0, date/time + location tracking, "can't start a match at the same location and
same time."

**Persistence retrofit request** (after the Principle IV amendment above), verbatim:

> [Amend plan.md (only, not tasks.md yet) to incorporate the Persistence decision made after
> 001-start-match was already implemented: replace the plain Dictionary<int, Match> design with
> IMatchRepository, an EF Core DbContext, and a SQLite implementation under
> src/WorldCupScoreboard/Persistence/ and Persistence/Migrations/, plus a fake in-memory
> IMatchRepository for unit tests; Scoreboard must depend only on IMatchRepository, never EF Core
> directly, per constitution Principle IV.]

Clarify-stage choices (via AskUserQuestion, recommended options accepted where noted):
future-scheduling supported (not recommended — user's explicit choice); exact date/time equality
for the conflict rule (recommended, accepted); in-progress-only conflict scope (recommended,
accepted); system-generated unique match ID (recommended, accepted); non-throwing rejection
result for `StartMatch`/`GetMatch` (recommended, accepted).

## `002-update-score`

**Opening description, verbatim:**

> 002-update-score. It should be possible to update the score for each team in the match. Score
> can only go up and never down.

This one sentence directly contradicted CLAUDE.md's own then-current text ("not enforcing
monotonic non-decrease — keeps the library simple"). The spec followed the user's explicit
instruction; CLAUDE.md was updated only after separate confirmation.

**Broadening FR-002 to reject non-numeric input, verbatim (direct edit, no formal `/speckit-*`
invocation):**

> [Score updates must also reject letters and special characters, not just negative numbers.]

## `003-finish-match`

**Opening description, verbatim:**

> 003-finish-match. A match's data if marked finished still exists in db. A finish match's
> status can't be changed to in-progress. One a match is marked finished then score can't be
> updated.

## `004-live-summary` / `005-match-history`

**Opening description** (paraphrased): live summary ordered by total score descending, tie-break
by most-recently-started; separately, a paginated "see all matches saved in db" browse feature
(10/page, most-recent created-or-updated first) — explicitly called out by the user as "separate
from live summary."

The pagination half was deliberately not bundled into `004-live-summary`'s spec (would violate
Single-Concern Features). Resolved via AskUserQuestion: user chose to fold it into
`005-match-history` rather than create a new standalone spec.

**`005`'s clarify-stage context, verbatim:**

> pagination is done with page size but also page can be changed.

(Already correctly resolved in the spec — page size fixed at 10, page number caller-supplied.)

## `006-scoreboard-api`

**Opening description, verbatim:**

> 006-scoreboard-api. Create a minimal .net web api project that uses that library and provide
> apis for all methods match creation, score update etc. If any method is missing then ask. Add
> unit tests for api as well and test while completing it. Add a docker file and swagger as well.
> It should be possible to use swagger to test the api. Return appropriate status code and
> response where applicable.

**Redefining the error-response contract mid-stream, verbatim:**

> for any response that should be 4xx, there should be a property e.g error_code =
> "match_not_found" etc. and a property for a message error_message. Use One of package and
> discriminated union to handle those cases.

This single instruction superseded the earlier `ProblemDetails`-based design and triggered a
full rewrite of `spec.md`, `research.md`, `data-model.md`, `contracts/api.md`, `plan.md`, and a
complete `tasks.md` regeneration.

## `007-scoreboard-frontend`

**Opening description, verbatim (the fullest single-sentence spec description of the whole
session):**

> 007-scoreboard-frontend. Create a separate angular application that will call the api for all
> the features implemented. Both services could be run through docker compose file. For front
> end, use material design theme, while and blue color pallet. The Dashboard should look
> professional and sleek. It should have a left side nav for History and Summary. For both, each
> country should shown as a separate card against each other in single row with "VS" in between.
> Each card would have the flag of country and their name then after a space the score. When we
> need to start a new match, then through side nav, there should be a separate tab for matches
> and inside that we would select country with flag and name through a drop down for left
> country and for right and a button to start the match. Then we can update the score there and
> finish the match. If we switch to History or summary then latest match we just started, or
> score updated or finished should be loaded. If we receive an error from the backend for cases
> like assigning same country to different matches then a modal or pop up should appear showing
> error and errors should be handled and shown in professional manner.

**Discovering pre-existing artifacts from a parallel session, and asking how to proceed:**

> specs/007-scoreboard-frontend already has spec.md/plan.md/tasks.md from what looks like a
> parallel session (more detail than your latest prompt: Playwright E2E, specific hex colors,
> ports 5000/4200). How should I proceed with this /speckit-specify run?

User chose: **keep existing, review only** — the reason later reconciliation passes for `007`
looked different from every other spec's from-scratch pipeline run.

**Clarify round 1, verbatim:**

> do the playwright work. Regarding User Story 3, I don't understand clarify and ask question if
> needed.

**Clarify round 2 — the success-confirmation modal requirement, verbatim:**

> for the success scenario of match creating, update and finish etc. there should be modal/pop
> up as well to notify that it has been done based on the expected status code from the api.

Presented as a multiple-choice question (modal vs. snackbar/toast); the recommended option was
snackbar, but the **user chose modal dialog** — same `MatDialog` family as the existing error
path, for visual consistency.

**Directly requesting the RxJS-cancellation edge case be implemented, verbatim:**

> Address E1: implement the RxJS cancellation for rapid tab switching.

## Post-implementation: README.md and AI.md

**README.md's required content checklist, verbatim:**

> README.md must answer the brief's exact ask (assumptions, reasoning, trade-offs made) and
> satisfy every item in CLAUDE.md's "Acceptance criteria" checklist for README.md. Concretely,
> cover: 1. What the library does + a basic usage example. 2. The .NET-over-Java/Maven
> deviation, prominently, with rationale — not buried. 3. Assumptions: team-name validation,
> one-in-progress-match-per-team rule, absolute (not delta) score updates, non-negative + 
> monotonic-non-decrease score validation, behavior on nonexistent/already-finished matches,
> thread-safety via coarse locking. 4. Ordering rationale: monotonic sequence counters (Id,
> ActivitySequence) vs wall-clock/DateTime, and why. 5. Trade-offs: simplicity vs strict
> validation, coarse locking vs throughput, SQLite/EF Core persistence vs an in-memory-only
> design. 6. The chosen extra feature (GetHistory, paginated match history) and rationale. 7.
> The throwing-vs-non-throwing convention split and why. 8. How to build/test, referencing the
> worked-example test. 9. How to run the CLI demo. 10. Document the API and frontend as explicit,
> deliberate scope expansions beyond the brief. No unexplained TODOs or placeholders.

**Follow-up expansions, verbatim:**

> Read Me also should explain about the repo, that spec driven development/speckit was used, The
> main thing is the library project and api and front-end are additional things, Explain that is
> a separate heading what the features that exercise document "Coding Exercise version 2.01.pdf"
> in Download wanted and the main additional feature of Match History I chose and why. It should
> also show how to run each project separately and together. It should mention the test folders
> as well and what test coverage we got and what test methodology we followed.

> Remove '## ⚠️ Deviation from the brief: .NET instead of Java/Maven' and anything under it. Just
> mention that .Net is used.

> The Read me should also tell about docker compose and how to use it to bring up both
> applications.

**AI.md's required content checklist, verbatim:**

> AI.md must satisfy every item in CLAUDE.md's AI.md acceptance criteria: 1. Summarize how AI was
> used, stage by stage (constitution/specify/clarify/plan/tasks/implement), across all 7 specs.
> 2. Embed actual key prompt history directly, pulled from chat-history.md — not just a pointer
> to it. 3. List every artifact that guided implementation with its path. 4. Call out at least
> one instance where an AI suggestion was overridden or changed, with reasoning — strong
> candidates already in chat-history.md: the 005 seeding bug (I1, .Max() crash on empty repo,
> caught by /speckit-analyze before any code existed), the 006 DI-lifetime bug (scoped DbContext
> disposed inside a singleton, caught before shipping), and 007's 3 real bugs (flat vs nested
> Match model, status-enum-as-string crash, location/scheduledAt collision found via Playwright
> testing) found reconciling a parallel session's pre-existing frontend code against the Spec-Kit
> pipeline. 5. Must stay consistent with README.md — no contradicting claims.

**Bug-fix requests, verbatim:**

> Make the UI better and more professional. Also, the modal that opens when you approve or
> error, the formatting in it for icon or text isn't correct. Same for 'Start Match' button.
> Please make improvements but make sure to not break anything.

**Trimming AI.md down, verbatim (the request that produced this file):**

> Make AI.md shorter. Don't give "Where AI suggestions were overridden or changed" and "Key
> Prompt History". Add whole prompt history as a separate md file and refer to it in the AI.md.
