namespace WorldCupScoreboard.Exceptions;

public class InvalidPageException : Exception
{
    public int Page { get; }

    public InvalidPageException(int page)
        : base($"Page {page} is invalid — page numbers must be 1 or greater.")
    {
        Page = page;
    }
}
