namespace Tickefy.Application.Auth.Common
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
