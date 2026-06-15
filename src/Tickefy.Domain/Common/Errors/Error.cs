namespace Tickefy.Domain.Common.Errors;

public class Error(string message)
{
    public string Message { get; } = message;
}
