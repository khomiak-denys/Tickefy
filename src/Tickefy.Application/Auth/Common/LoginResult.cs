namespace Tickefy.Application.Auth.Сommon
{
    public record LoginResult
    (
        Guid Id,
        string FirstName,
        string LastName,
        string Login,
        string Token
    );
}
