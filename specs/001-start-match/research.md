# Research: Start New Match

Phase 0 output for `specs/001-start-match`. No open `NEEDS CLARIFICATION` markers remained in the
spec's Technical Context (all resolved during `/speckit-clarify` or already settled by CLAUDE.md's
project-wide Confirmed Decisions). The items below are design/pattern decisions needed to move
from spec to data model, not unresolved requirements.

## 1. Match ID generation strategy

- **Decision**: Reuse a monotonic in-memory integer sequence counter (incremented under the same
  coarse lock used for mutations) as the `Match.Id`.
- **Rationale**: CLAUDE.md already commits to a monotonic in-memory sequence counter for
  "start order" tie-breaking in the live-summary feature (004), specifically to avoid
  timestamp-resolution ambiguity and keep tests deterministic. Reusing the same counter as the
  match identity avoids introducing a second identity mechanism (e.g., `Guid`) and gives IDs a
  natural, deterministic, testable ordering for free.
- **Alternatives considered**:
  - `Guid`: rejected — opaque, non-deterministic in test assertions, no natural ordering, and adds
    a concept CLAUDE.md's ordering decision already ruled out for the same reasons.
  - Composite key (team names + scheduled date/time): rejected — this is exactly the identity the
    spec's clarification session explicitly moved away from (it breaks down for repeat fixtures
    and for historical records in spec 005).

## 2. Failure-signaling shape for `StartMatch` (FR-008)

- **Decision**: `StartMatch` returns `Match?` — `null` on any rejection (FR-004, FR-005, FR-006),
  the created `Match` on success. No exception is thrown for a rejected start. `GetMatch` follows
  the same style: returns `Match?`, `null` when the ID doesn't exist.
- **Rationale**: The spec's clarification session settled on a non-throwing result. A nullable
  return is the simplest shape that satisfies this — no new public `Result<T>`-style wrapper type
  is needed for a single yes/no outcome. Matches CLAUDE.md's general "simplicity" trade-off
  preference and avoids introducing an abstraction ahead of a second use case that would justify
  it.
- **Alternatives considered**:
  - `bool TryStartMatch(..., out Match? match)`: idiomatic .NET `TryXxx` pattern, but adds an
    `out` parameter for no real benefit over a nullable return when there's only one output value.
  - Custom `Result<Match>` type carrying a failure reason: rejected for now — the spec does not
    require exposing *why* a start failed, only that it did; introducing this ahead of an actual
    need would be speculative.

## 3. `IScoreboard` interface growth strategy across specs

- **Decision**: `IScoreboard` exposes only `StartMatch` and `GetMatch` in this feature. Specs
  002-005 each add their own method(s) to the same interface when implemented, not before.
- **Rationale**: Constitution Principle III (Single-Concern Features) and Principle I (Test-First)
  together rule out pre-declaring methods for capabilities that don't exist yet — an untested stub
  method (e.g., a placeholder `UpdateScore` that throws `NotImplementedException`) would be
  production code with no preceding failing test, and would couple this spec to unrelated
  capabilities.
- **Alternatives considered**:
  - Declare all 5 core operations now, implement bodies incrementally: rejected — violates
    Test-First (adds untested surface) and Single-Concern Features (couples specs together).

## 4. Concurrency primitive

- **Decision**: A single private lock object on the `Scoreboard` implementation guards every
  mutating operation (currently just `StartMatch`); reads (`GetMatch`) also take the lock for a
  consistent snapshot.
- **Rationale**: Restates CLAUDE.md's existing decision concretely for this feature: coarse-grained
  locking, documented as "simple and correct, not optimized for throughput." No per-match or
  reader/writer locking is introduced, since no performance requirement exists to justify it.
- **Alternatives considered**: `ConcurrentDictionary` / lock-free structures — rejected as
  premature optimization for a kata with no stated performance goals; a single lock is simpler to
  reason about and verify correct.
