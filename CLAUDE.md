# Live Football World Cup Scoreboard — Project Context

## What this repo is

A take-home coding exercise (Sportradar's "Live Football World Cup Scoreboard" kata). Full brief:
`~/Downloads/Coding Exercise version 2.01.pdf`. Condensed requirements below so you don't need to
re-read the PDF every session.

**Deliverables required by the brief:**
1. A scoreboard library supporting multiple simultaneous matches, implementing:
   - Start a new match
   - Update the score
   - Finish a match
   - Get a summary of matches **in progress**, ordered by total score descending, then by
     most-recently-started first on ties
   - Exactly one additional operation of our own choice (see "Confirmed decisions" below)
2. `README.md` — assumptions, reasoning, trade-offs, and why the extra feature was chosen
3. `AI.md` — how AI tools were used, prompt history, and any artifacts that guided the implementation
4. A git repo formatted as if submitting for code review to teammates — clean, logical commit
   history. **The extra feature must land in its own distinct commit.**
5. Worked example from the brief (must produce this exact order) — treat as an acceptance test:
   ```
   Mexico 0–Canada 5, Spain 10–Brazil 2, Germany 2–France 2, Uruguay 6–Italy 6, Argentina 3–Australia 1
   → summary order: Uruguay 6–Italy 6, Spain 10–Brazil 2, Mexico 0–Canada 5, Argentina 3–Australia 1, Germany 2–France 2
   ```

## Roadmap

Each row is its own Spec-Kit feature (`specs/<NNN-name>/`), built and merged sequentially.

| Spec | Phase | Covers |
| --- | --- | --- |
| `specs/001-start-match` | Phase 1 (brief) | Start a new match — establishes `Match`, `MatchStatus`, `IScoreboard` skeleton |
| `specs/002-update-score` | Phase 1 (brief) | Update the score |
| `specs/003-finish-match` | Phase 1 (brief) | Finish a match |
| `specs/004-live-summary` | Phase 1 (brief) | Get summary of in-progress matches, ordering rule + the brief's worked example as an acceptance test |
| `specs/005-match-history` | Phase 1 (brief) | The chosen extra feature (`GetHistory`) — must land in its own distinct commit per the brief |
| `specs/006-scoreboard-api` | Phase 2 (beyond the brief) | REST API |
| `specs/007-scoreboard-frontend` | Phase 3 (beyond the brief) | Angular/React client |

## Confirmed decisions (do not re-litigate these without flagging it to the user first)

- **Stack deviation**: the brief explicitly asks for a Java/Maven package. We are deliberately using
  **.NET 9 (C#) + xUnit** instead. This MUST be called out explicitly and early in README.md as an
  intentional deviation, with rationale — not silently substituted.
- **Extra operation**: `GetHistory()` — returns every match ever started (in-progress *and*
  finished), each tagged with status, ordered by start order (most recent first). Rationale: fits a
  "Data & Odds Platform" — historical results have standalone value beyond the live board. Document
  this choice and rationale in README.md, and land it in its own commit.
- **Ordering/tie-break**: use a monotonic in-memory sequence counter for "start order," not
  `DateTime`/wall-clock — avoids tie-break ambiguity from timestamp resolution and keeps tests
  deterministic.
- **Validation rules** (document as explicit assumptions in README.md):
  - Team names non-null/non-empty.
  - A team cannot be in more than one in-progress match at a time.
  - Scores are non-negative integers, supplied as absolute values (not deltas), per the brief's
    example. Not enforcing monotonic non-decrease — keeps the library simple, as the brief itself
    frames these as open judgment calls.
  - Operating on a non-existent or already-finished match throws.
- **Concurrency**: coarse-grained internal locking for thread-safety, documented as "simple and
  correct, not optimized for throughput."
- **Process**: using GitHub Spec-Kit for the full SDLC pipeline —
  `/speckit-constitution` → `/speckit-specify` → `/speckit-clarify` → `/speckit-plan` →
  `/speckit-tasks` → `/speckit-implement`. These are installed as Claude Code skills under
  `.claude/skills/speckit-*`, not native slash commands. Each stage's artifact
  (`.specify/memory/constitution.md`, `specs/*/spec.md`, `plan.md`, `tasks.md`) is committed as its
  own commit and doubles as part of the documented reasoning trail for `AI.md`.
- **Chat history**: maintain `chat-history.md` at the repo root as a running log, updated
  continuously (don't defer writing it to the end — you'll forget details). It is gitignored and
  freely editable/deletable while work is in progress — see "Chat history capture mechanism" below
  for why. `AI.md` should directly summarize/embed the key prompt history at the end, once
  finalized.

## Chat history capture mechanism

Two layers — don't rely on memory/summarization alone to reconstruct prompt history at the end:

1. **Ground truth (automatic)**: every session's full transcript is already persisted as JSONL at
   `~/.claude/projects/<encoded-repo-path>/<session-id>.jsonl`. This survives context compaction —
   it's the raw source of truth if anything needs to be recovered later. No setup required.
2. **Repo-visible export (manual, at checkpoints)**: run the `/export` slash command at the end of
   each spec-kit stage (after `/speckit-specify`, `/speckit-clarify`, `/speckit-plan`,
   `/speckit-tasks`, each `/speckit-implement` slice) and save the output under
   `chat-history/<stage-name>.md`.

**Important — this stays uncommitted until the end, on purpose.** `chat-history.md` and everything
under `chat-history/` are gitignored while work is in progress. They are a free-editing scratch
area — add, revise, or delete entries as needed, while iterating. Only once the
author is satisfied does a **single final commit** add the finished, reviewed version to the repo
(or just the finished `AI.md` with hand-picked excerpts, if the raw exports aren't meant to ship at
all). Do NOT commit `chat-history.md` incrementally per stage — an incremental commit history would
itself become a permanent record of anything edited out later, defeating the point of keeping this
editable. Hand-pick the key prompts/decisions from the final `chat-history/` exports into `AI.md`'s
own prompt-history section — the brief wants prompt history *embedded in* AI.md, not merely linked.

## Acceptance criteria — verify before considering docs done

**README.md** must satisfy all of:
- [ ] States what the library does and how a caller would use it (basic usage example)
- [ ] Explicitly and prominently documents the Java/Maven → .NET deviation, with rationale (not
      buried in a footnote)
- [ ] Documents assumptions: team-name validation, absolute (not delta) score updates, non-negative
      score validation, one-in-progress-match-per-team rule, behavior on already-finished/nonexistent
      matches, thread-safety approach
- [ ] Documents reasoning for the ordering implementation (monotonic sequence counter vs wall-clock)
- [ ] Documents trade-offs made (e.g. simplicity vs strict validation, coarse locking vs throughput,
      in-memory-only vs persistence)
- [ ] Documents the chosen extra feature (`GetHistory`) and explicit rationale for choosing it
- [ ] States how to build/test (`dotnet build`, `dotnet test`) and references the test that encodes
      the brief's worked example
- [ ] No unexplained TODOs or placeholder text

**AI.md** must satisfy all of:
- [ ] Summarizes how AI was used, stage by stage (constitution/specify/clarify/plan/tasks/implement)
- [ ] Embeds actual key prompt history directly (not just a pointer) — pulled from the `/export`
      checkpoints in `chat-history/`
- [ ] Lists every artifact that guided implementation with its path (`memory/constitution.md`,
      `specs/*/spec.md`, `plan.md`, `tasks.md`)
- [ ] Calls out at least one instance where an AI suggestion was overridden or changed, with the
      reasoning why — demonstrates judgment, not blind acceptance
- [ ] Consistent with README.md — no contradicting claims about decisions made

## Repo layout (target state)

```
/
├── README.md / AI.md / chat-history.md
├── WorldCupScoreboard.sln
├── src/WorldCupScoreboard/            (net9.0 class library: Match, MatchStatus, IScoreboard,
│                                        Scoreboard, Exceptions/)
├── tests/WorldCupScoreboard.Tests/    (xUnit — one test class per operation + a dedicated test
│                                        encoding the brief's worked example + history tests)
├── .github/workflows/dotnet.yml       (build+test on push)
├── .specify/                          (spec-kit scaffolding)
└── specs/                              (one numbered folder per Spec-Kit feature, see Roadmap)
    ├── 001-start-match/                (spec.md, plan.md, tasks.md)
    ├── 002-update-score/
    ├── 003-finish-match/
    ├── 004-live-summary/
    ├── 005-match-history/
    ├── 006-scoreboard-api/
    └── 007-scoreboard-frontend/
```

## Working conventions

- Conventional commit messages (`feat:`, `docs:`, `chore:`, `ci:`). Small, logical, reviewable
  commits — this repo is meant to read like a real PR to teammates.
- One Spec-Kit feature (see Roadmap) maps to one reviewable commit, or a couple of small ones —
  this is how the brief's "distinct commit for the extra feature" requirement is satisfied
  naturally by spec `005-match-history`, with no special-casing needed.
- Review every spec-kit/AI-generated file before accepting it into a commit — don't rubber-stamp
  `/implement` output.
- `dotnet build` and `dotnet test` must be clean before any commit that touches `src/` or `tests/`.
