namespace WorldCupScoreboard.Api.Contracts;

public interface IApiError
{
    string ErrorCode { get; }

    string ErrorMessage { get; }
}
