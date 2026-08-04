using Microsoft.EntityFrameworkCore;
using OneOf;
using WorldCupScoreboard;
using WorldCupScoreboard.Api.Contracts;
using WorldCupScoreboard.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Scoreboard")
    ?? "Data Source=scoreboard-api.db";

var dbContextOptions = new DbContextOptionsBuilder<ScoreboardDbContext>()
    .UseSqlite(connectionString)
    .Options;
var dbContext = new ScoreboardDbContext(dbContextOptions);
dbContext.Database.Migrate();

var scoreboard = new Scoreboard(new SqliteMatchRepository(dbContext));

builder.Services.AddSingleton<IScoreboard>(scoreboard);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/matches", (StartMatchRequest request, IScoreboard scoreboard) =>
{
    OneOf<Match, StartRejectedError> result = scoreboard.StartMatch(
        request.HomeTeam, request.AwayTeam, request.ScheduledAt, request.Location)
        is { } match
        ? match
        : new StartRejectedError();

    return result.Match(
        match => Results.Created($"/matches/{match.Id}", match),
        error => error.ToHttpResult(StatusCodes.Status400BadRequest));
});

app.MapGet("/matches/{id:int}", (int id, IScoreboard scoreboard) =>
{
    OneOf<Match, MatchNotFoundError> result = scoreboard.GetMatch(id)
        is { } match
        ? match
        : new MatchNotFoundError(id);

    return result.Match(
        match => Results.Ok(match),
        error => error.ToHttpResult(StatusCodes.Status404NotFound));
});

app.MapPut("/matches/{id:int}/score", (int id, UpdateScoreRequest request, IScoreboard scoreboard) =>
{
    OneOf<Match, MatchNotFoundError, InvalidScoreError> result;
    try
    {
        result = scoreboard.UpdateScore(id, request.HomeScore, request.AwayScore);
    }
    catch (WorldCupScoreboard.Exceptions.MatchNotFoundException)
    {
        result = new MatchNotFoundError(id);
    }
    catch (WorldCupScoreboard.Exceptions.InvalidScoreException ex)
    {
        result = new InvalidScoreError(ex.TeamName, ex.AttemptedScore, ex.CurrentScore);
    }

    return result.Match(
        match => Results.Ok(match),
        notFound => notFound.ToHttpResult(StatusCodes.Status404NotFound),
        invalidScore => invalidScore.ToHttpResult(StatusCodes.Status400BadRequest));
});

app.MapPost("/matches/{id:int}/finish", (int id, IScoreboard scoreboard) =>
{
    OneOf<Match, MatchNotFoundError> result;
    try
    {
        result = scoreboard.FinishMatch(id);
    }
    catch (WorldCupScoreboard.Exceptions.MatchNotFoundException)
    {
        result = new MatchNotFoundError(id);
    }

    return result.Match(
        match => Results.Ok(match),
        error => error.ToHttpResult(StatusCodes.Status404NotFound));
});

app.MapGet("/matches/summary", (IScoreboard scoreboard) => Results.Ok(scoreboard.GetSummary()));

app.MapGet("/matches/history", (int page, IScoreboard scoreboard) =>
{
    OneOf<Match[], InvalidPageError> result;
    try
    {
        result = scoreboard.GetHistory(page).ToArray();
    }
    catch (WorldCupScoreboard.Exceptions.InvalidPageException)
    {
        result = new InvalidPageError(page);
    }

    return result.Match(
        matches => Results.Ok(matches),
        error => error.ToHttpResult(StatusCodes.Status400BadRequest));
});

app.Run();

public partial class Program
{
}
