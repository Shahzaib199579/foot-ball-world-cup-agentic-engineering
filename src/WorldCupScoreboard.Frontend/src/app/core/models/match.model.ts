export enum MatchStatus {
  InProgress = 0,
  Finished = 1
}

export interface Team {
  name: string;
  score: number;
}

// Mirrors 006-scoreboard-api's actual JSON response shape: homeTeam/awayTeam are nested
// { name, score } objects, and status is the numeric MatchStatus enum, not a string.
export interface Match {
  id: number;
  homeTeam: Team;
  awayTeam: Team;
  status: MatchStatus;
  scheduledAt?: string;
  location?: string;
  activitySequence?: number;
  totalScore?: number;
}

export interface StartMatchRequest {
  homeTeam: string;
  awayTeam: string;
  scheduledAt?: string;
  location?: string;
}

export interface UpdateScoreRequest {
  homeScore: number;
  awayScore: number;
}
