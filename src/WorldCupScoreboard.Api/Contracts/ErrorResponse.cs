using System.Text.Json.Serialization;

namespace WorldCupScoreboard.Api.Contracts;

public record ErrorResponse(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("error_message")] string ErrorMessage);
