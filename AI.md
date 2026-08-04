# AI.md

How AI was used to build this repository — Claude Code, driving GitHub Spec-Kit's full
spec-driven SDLC pipeline (`/speckit-constitution` → `/speckit-specify` → `/speckit-clarify` →
`/speckit-plan` → `/speckit-tasks` → `/speckit-analyze` → `/speckit-converge` →
`/speckit-implement`) across all 7 specs (`001-start-match` through `007-scoreboard-frontend`).

Every stage of every spec was actually run through this pipeline, not written by hand and
retrofitted with spec-kit artifacts afterward — `specs/*/spec.md`/`plan.md`/`tasks.md` are the
real design documents that preceded and drove each implementation, not documentation generated
after the fact.

**Full prompt history**: [`PROMPT-HISTORY.md`](PROMPT-HISTORY.md) has every key verbatim prompt
that drove this repository, grouped by spec/stage — this document only summarizes.

## How AI was used, stage by stage

### `/speckit-constitution`

Filled `.specify/memory/constitution.md`'s 5 principles from the user's own verbatim text
(Test-First; Verify-Plan-Implement-Verify; Single-Concern Features; Layered
Architecture/Library-First, later amended twice — once to add the `IMatchRepository`
persistence abstraction, once via a separate amendment adding Principle V, Runnable Local
Verification). AI's role here was mechanical: apply the user's exact wording, respect an
explicit "don't touch anything else" instruction literally (leaving template placeholders like
`[PROJECT_NAME]` and `[GOVERNANCE_RULES]` deliberately unfilled), and maintain the Sync Impact
Report tracking what changed and why.

### `/speckit-specify`

For each spec, translated a short user description (sometimes a single sentence, sometimes a
paragraph) into a full spec — user stories, acceptance scenarios, functional requirements,
measurable success criteria, and an Assumptions section documenting every default chosen
without asking. Where a description was genuinely ambiguous, raised up to 3
`[NEEDS CLARIFICATION]` markers rather than guessing silently; most specs (`002`-`005`) needed
zero, since strong precedent already existed from earlier specs. `007` was the exception: its
`spec.md` was discovered *already written* by a separate, parallel Claude Code session working
on the same repo — that spec was reviewed and reconciled against the pipeline rather than
regenerated from scratch.

### `/speckit-clarify`

Ran a structured ambiguity/coverage scan against each spec before planning. Most specs came
back fully clear (zero questions asked) because earlier specs had already established strong
conventions (throwing behavior, exception types, ordering rules) that later specs could just
reuse. Where genuine ambiguity existed — `001`'s scheduling/identity/failure-signaling
questions, `007`'s success-notification UX (modal vs. toast) — asked one focused,
multiple-choice question at a time with a recommended default, rather than batching everything
up front.

### `/speckit-plan`

Generated `research.md` (decisions + rationale + alternatives considered), `data-model.md`,
`contracts/`, and `quickstart.md` for each spec, re-checking the constitution both before and
after design. Consistently verified claims against actual source code rather than trusting
memory — e.g. before `003-finish-match`'s plan, re-read `Scoreboard.cs` directly to confirm
`StartMatch`/`UpdateScore` really did already have defensive `Status != InProgress` checks
before claiming they needed no changes. `007` needed `/speckit-plan` re-run **four separate
times** across the session — once to bring a pre-existing, non-standard `plan.md` into the
template's required structure, once to fold in a new clarification (FR-011, the success modal),
and twice more to catch design decisions (a request-cancellation pattern) that had been
implemented directly in code without the plan being updated first.

### `/speckit-tasks`

Generated the actual `tasks.md` checklist each `/speckit-implement` pass followed — organized
by user story, test-first ordering enforced explicitly. Task counts scaled with genuine
complexity: `003-finish-match` needed only 10 tasks (most of its behavior was already
defensively implemented by earlier specs), `007-scoreboard-frontend` needed 51 initially, then
grew to 59 after a `/speckit-converge` pass appended 8 more for gaps found in the
already-existing frontend code.

### `/speckit-analyze`

Read-only cross-consistency checks across `spec.md`/`plan.md`/`tasks.md` and the constitution.
Found real, actionable issues on several passes — not just clean reports every time: a genuine
seeding-pattern bug in `005-match-history` (caught before any code existed — see below), a
`plan.md`/`tasks.md` file-count mismatch on `002`, a missing task for an `ErrorDialogService`
that `plan.md` named but no `007` task ever created, and — twice — a success/error-dialog
attribution error that got fixed on one side and then found again, unfixed, on the
mirror-image side in a *second* `/speckit-analyze` pass later in the session.

### `/speckit-converge`

Read-only assessment of the actual codebase against spec/plan/tasks intent — appends new tasks
for any gap found, touches no other files. Reported "Converged" (zero gaps) on nearly every
run for `001`-`006`, since those specs were implemented directly through the pipeline. The one
major exception was `007`: converging against code that had been written by a separate,
un-pipelined session surfaced 2 CRITICAL bugs before they'd have shipped (a flat-vs-nested
`Match` model mismatch, and a numeric status field compared as a string).

### `/speckit-implement`

Executed each `tasks.md` in order, test-first: write the failing test, confirm it fails for the
right reason (a compile error for statically-typed C#, a missing route for the API, a missing
component for the frontend), then write the minimum implementation to pass it, then re-run the
*full* suite before moving on. This is where most of the real engineering happened — via genuine
`dotnet build`/`dotnet test`/`ng test`/Playwright runs against real SQLite, a real running API,
and a real browser, not just read-through code review.

## Artifacts that guided implementation

| Spec | Constitution | Spec | Plan | Tasks |
|---|---|---|---|---|
| Governing (all specs) | [`.specify/memory/constitution.md`](.specify/memory/constitution.md) | — | — | — |
| `001-start-match` | ↑ | [`specs/001-start-match/spec.md`](specs/001-start-match/spec.md) | [`plan.md`](specs/001-start-match/plan.md) | [`tasks.md`](specs/001-start-match/tasks.md) |
| `002-update-score` | ↑ | [`specs/002-update-score/spec.md`](specs/002-update-score/spec.md) | [`plan.md`](specs/002-update-score/plan.md) | [`tasks.md`](specs/002-update-score/tasks.md) |
| `003-finish-match` | ↑ | [`specs/003-finish-match/spec.md`](specs/003-finish-match/spec.md) | [`plan.md`](specs/003-finish-match/plan.md) | [`tasks.md`](specs/003-finish-match/tasks.md) |
| `004-live-summary` | ↑ | [`specs/004-live-summary/spec.md`](specs/004-live-summary/spec.md) | [`plan.md`](specs/004-live-summary/plan.md) | [`tasks.md`](specs/004-live-summary/tasks.md) |
| `005-match-history` | ↑ | [`specs/005-match-history/spec.md`](specs/005-match-history/spec.md) | [`plan.md`](specs/005-match-history/plan.md) | [`tasks.md`](specs/005-match-history/tasks.md) |
| `006-scoreboard-api` | ↑ | [`specs/006-scoreboard-api/spec.md`](specs/006-scoreboard-api/spec.md) | [`plan.md`](specs/006-scoreboard-api/plan.md) | [`tasks.md`](specs/006-scoreboard-api/tasks.md) |
| `007-scoreboard-frontend` | ↑ | [`specs/007-scoreboard-frontend/spec.md`](specs/007-scoreboard-frontend/spec.md) | [`plan.md`](specs/007-scoreboard-frontend/plan.md) | [`tasks.md`](specs/007-scoreboard-frontend/tasks.md) |

Each spec folder also has its own `research.md` (Phase 0 decisions + rationale + alternatives
considered), `data-model.md`, `contracts/`, and `quickstart.md` (manual validation walkthrough)
— all Phase 0/1 outputs of that spec's own `/speckit-plan` run, referenced throughout
implementation but omitted from the table above for brevity.

## Consistency with README.md

This document and `README.md` describe the same underlying facts and should not be read as
contradicting each other:

- Both agree the library (`src/WorldCupScoreboard/`) is the actual deliverable the brief asked
  for, and that the API (`006`) and frontend (`007`) are additional, clearly-labelled scope
  expansions.
- Both agree on the chosen extra feature (`GetHistory`, paginated match history) and its
  rationale.
- `README.md`'s "Process" section states GitHub Spec-Kit was used for the full SDLC; this
  document is the detailed version of that same claim.
