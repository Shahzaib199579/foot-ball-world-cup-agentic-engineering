# Contract: IScoreboard (this feature's slice)

Public library contract addition for `005-match-history`. Only documents what this feature
adds to `IScoreboard` — `StartMatch`/`GetMatch` (`001`), `UpdateScore` (`002`), `FinishMatch`
(`003`), and `GetSummary` (`004`) are unchanged in *signature*, though `StartMatch`/
`UpdateScore`/`FinishMatch`'s internal behavior each gains one new side effect (bumping
`Match.ActivitySequence` — see those methods' updated behavior notes below).

## `GetHistory`

```csharp
IEnumerable<Match> GetHistory(int page);
```

**Preconditions checked:**

- `page >= 1`; otherwise throws `InvalidPageException`.

**Postconditions:**

- Returns up to 10 matches — **both in-progress and finished**, no exclusion — ranked by most
  recent activity (creation, score update, or finish) descending, for the requested page.
- Page 1 returns ranks 1-10 (most recent first), page 2 returns ranks 11-20, and so on.
- A page beyond the available data returns an empty sequence (never `null`, never throws for
  this case — only `page < 1` throws).
- Never modifies any match's data — purely a read.

## Updated behavior of existing methods (side effect only, no signature change)

- `StartMatch`: on success, the newly created `Match` is assigned the next
  `ActivitySequence` value, ranking it as the most recent activity at that moment.
- `UpdateScore`: on success, the updated `Match`'s `ActivitySequence` is bumped to the next
  value, ranking it as the most recent activity, ahead of matches whose only activity was an
  earlier creation or update.
- `FinishMatch`: on success, the finished `Match`'s `ActivitySequence` is bumped the same way.

## Notes

- Safe to call concurrently with every other operation; `Scoreboard` serializes access
  internally via the existing coarse lock.
- This is the project's chosen "additional operation" (per the brief) — no further methods are
  planned for `IScoreboard` beyond this in Phase 1. `006-scoreboard-api`/`007-scoreboard-frontend`
  are separate phases with their own contracts (HTTP endpoints, UI), not additions to this
  interface.
