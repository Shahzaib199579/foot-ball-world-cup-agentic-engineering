namespace WorldCupScoreboard.Api.Contracts;

public record StartMatchRequest(string HomeTeam, string AwayTeam, DateTime ScheduledAt, string Location);
