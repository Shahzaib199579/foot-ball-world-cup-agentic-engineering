using Microsoft.AspNetCore.Http;

namespace WorldCupScoreboard.Api.Contracts;

public static class ApiErrorExtensions
{
    public static IResult ToHttpResult(this IApiError error, int statusCode)
    {
        var body = new ErrorResponse(error.ErrorCode, error.ErrorMessage);
        return TypedResults.Json(body, statusCode: statusCode);
    }
}
