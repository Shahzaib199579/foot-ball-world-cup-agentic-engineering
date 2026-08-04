namespace WorldCupScoreboard.Api.Contracts;

public class InvalidPageError : IApiError
{
    public int Page { get; }

    public InvalidPageError(int page)
    {
        Page = page;
    }

    public string ErrorCode => "invalid_page";

    public string ErrorMessage => $"Page {Page} is invalid — page numbers must be 1 or greater.";
}
