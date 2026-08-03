<!--
Sync Impact Report
Version change: [CONSTITUTION_VERSION] → [CONSTITUTION_VERSION] (unchanged — deferred, see below)
Modified principles:
  - [PRINCIPLE_1_NAME] → I. Test-First (NON-NEGOTIABLE) (filled)
  - [PRINCIPLE_2_NAME] → II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE) (filled)
  - [PRINCIPLE_3_NAME] → III. Single-Concern Features (filled)
  - [PRINCIPLE_4_NAME] → IV. Layered Architecture / Library-First (filled)
Removed sections:
  - [PRINCIPLE_5_NAME] / [PRINCIPLE_5_DESCRIPTION] slot (only 4 principles supplied by the user;
    template had 5 — per skill instructions, respecting the specified count)
Added sections:
  - V. Runnable Local Verification (CLI Demo) — added in a later amendment, per explicit user
    instruction to touch nothing else (other 4 principles, other sections, version/date
    metadata all still deferred exactly as below)
Amended principles (later amendments, extending rather than replacing):
  - IV. Layered Architecture / Library-First — extended to require persistence be abstracted
    behind `IMatchRepository` (business logic never depends on EF Core/SQLite directly; unit
    tests use a fake/in-memory `IMatchRepository`), per explicit user instruction. Principles
    I, II, III, and V untouched by this amendment.
Deferred (left as unresolved template placeholders, per explicit instruction to touch nothing
beyond filling in the principles requested):
  - TODO([PROJECT_NAME]): document title not yet set
  - TODO([SECTION_2_NAME] / [SECTION_2_CONTENT]): additional-constraints section not yet defined
  - TODO([SECTION_3_NAME] / [SECTION_3_CONTENT]): workflow/review section not yet defined
  - TODO([GOVERNANCE_RULES]): amendment/compliance procedure not yet defined
  - TODO([CONSTITUTION_VERSION] / [RATIFICATION_DATE] / [LAST_AMENDED_DATE]): versioning and
    ratification metadata not yet set — establish on the next amendment that completes the
    remaining sections above
-->

# [PROJECT_NAME] Constitution

## Core Principles

### I. Test-First (NON-NEGOTIABLE)
TDD is mandatory for every operation: write a failing test that specifies the expected
behavior first, confirm it fails for the right reason, then write the minimum code to make
it pass (Red-Green-Refactor). No production code is written without a preceding failing
test. Every one of the 4 core operations plus the chosen extra operation MUST have direct
test coverage, including the brief's worked example as a literal acceptance test.

### II. Verify-Plan-Implement-Verify (NON-NEGOTIABLE)
On any test failure or bug: (1) reproduce it and identify root cause before touching code —
never guess-fix; (2) state the intended fix in one sentence before implementing it; (3)
implement the minimal fix; (4) re-run the FULL test suite, not just the failing test, to
confirm the fix and rule out regressions. A fix is not done until step 4 passes clean.

### III. Single-Concern Features
Each Spec-Kit feature is scoped to one independently testable, independently shippable unit
of behavior. Never bundle multiple operations or concerns into a single spec — a feature is
too large if it can't be fully specified, planned, and verified without touching unrelated
capabilities. This applies to every phase (library, API, frontend), not just the current
001-007 breakdown.

### IV. Layered Architecture / Library-First
All business logic and validation rules live ONLY in the scoreboard library (Phase 1). The
API (Phase 2) is a thin transport adapter with no business logic of its own. The frontend
(Phase 3) is a thin presentation layer that only calls the API — no direct library access,
no duplicated business logic. Each layer MUST be independently testable: library via unit
tests, API via integration tests against real HTTP endpoints, frontend via component/e2e
tests. Specific tech choices (API framework, Angular vs React) are NOT decided here — those
are decided per-phase in that phase's own plan.md when its /speckit-plan runs.

Persistence is abstracted behind a repository interface (`IMatchRepository`); `Scoreboard`'s
business logic depends only on this abstraction, never on the concrete storage technology
(Entity Framework Core / SQLite) directly. This keeps Principle I (Test-First) practical:
unit tests exercise business logic against a fake/in-memory `IMatchRepository`.

### V. Runnable Local Verification (CLI Demo)
Every implemented feature must be exercisable manually, not only through
automated tests. A thin console demo project (demo/ScoreboardCli) wraps the
current state of IScoreboard and is updated alongside each feature to
demonstrate it — e.g. start matches, update scores, finish matches, print
the live summary, and (once built) the match history. It has zero business
logic of its own (per Principle IV) — it only calls the library and prints
results. A feature's implementation is not considered done until this demo
is updated to cover it, in the same commit as the feature. This is distinct
from the Phase 2 API and Phase 3 frontend in the roadmap — it is a
local-only, no-network tool that exists from Phase 1 onward.

## [SECTION_2_NAME]
<!-- Example: Additional Constraints, Security Requirements, Performance Standards, etc. -->

[SECTION_2_CONTENT]
<!-- Example: Technology stack requirements, compliance standards, deployment policies, etc. -->

## [SECTION_3_NAME]
<!-- Example: Development Workflow, Review Process, Quality Gates, etc. -->

[SECTION_3_CONTENT]
<!-- Example: Code review requirements, testing gates, deployment approval process, etc. -->

## Governance
<!-- Example: Constitution supersedes all other practices; Amendments require documentation, approval, migration plan -->

[GOVERNANCE_RULES]
<!-- Example: All PRs/reviews must verify compliance; Complexity must be justified; Use [GUIDANCE_FILE] for runtime development guidance -->

**Version**: [CONSTITUTION_VERSION] | **Ratified**: [RATIFICATION_DATE] | **Last Amended**: [LAST_AMENDED_DATE]
<!-- Example: Version: 2.1.1 | Ratified: 2025-06-13 | Last Amended: 2025-07-16 -->
